using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstantineAPI.Core.Database;
using InstantineAPI.Core.Domain;
using InstantineAPI.Core.Photos;
using InstantineAPI.Data;

namespace InstantineAPI.Photos
{
    public class PhotoService : IPhotoService
    {
        private readonly IFtpService _ftpService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAlbumService _albumService;
        private readonly IClock _clock;
        private readonly IGuid _guid;
        private readonly IPermissionsService _permissionsService;

        public PhotoService(IFtpService ftpService, 
                            IUnitOfWork unitOfWork, 
                            IAlbumService albumService, 
                            IClock clock,
                            IGuid guid,
                            IPermissionsService permissionsService)
        {
            _ftpService = ftpService;
            _unitOfWork = unitOfWork;
            _albumService = albumService;
            _clock = clock;
            _guid = guid;
            _permissionsService = permissionsService;
        }

        public async Task<bool> DeletePhoto(string photoId, string albumId, User requestor)
        {
            if (!await _permissionsService.CanDeletePicture(requestor, albumId))
            {
                return false;
            }
            var photo = await _unitOfWork.Photos.GetFirst(x => x.PhotoId == photoId);
            if(photo == null)
            {
                return false;
            }
            await _ftpService.DeletePhoto(photo);
            await _unitOfWork.Photos.Delete(photo);
            return true;
        }

        public async Task<byte[]> GetPicture(string photoId, string albumId, User requestor)
        {
            var album = await _albumService.GetAlbum(albumId);
            if (!await _permissionsService.CanSeePicture(requestor, albumId))
            {
                return null;
            }
            var photo = album.Photos.FirstOrDefault(x => x.PhotoId == photoId);
            if(photo == null)
            {
                return null;
            }
            return await _ftpService.GetPhoto(photo);
        }

        public async Task<Photo> StorePhoto(byte[] image, User author, string albumId)
        {
            if (!await _permissionsService.CanAddPicture(author, albumId))
            {
                return null;
            }
            var id = _guid.NewGuid().ToString();
            var photo = new Photo
            {
                Author = author,
                PhotoComments = new List<PhotoComment>(),
                PhotoLikes = new List<PhotoLike>(),
                Link = $"{author.LastName}/{author.FirstName}/{id}",
                PhotoId = id,
                TakeDate = _clock.UtcNow
            };
            var album = await _albumService.GetAlbum(albumId);
            album.Photos.Add(photo);
            await _unitOfWork.Albums.Update(album);

            await _ftpService.StorePhotoOnFtp(image, photo);
            return photo;
        }

        public Task<Photo> GetPhoto(string photoId)
        {
            return _unitOfWork.Photos.GetFirst(x => x.PhotoId == photoId);
        }

        public async Task<IEnumerable<Photo>> GetPhotos(string albumId, User requestor)
        {
            if (!await _permissionsService.CanSeePicture(requestor, albumId))
            {
                return null;
            }
            var album = await _albumService.GetAlbum(albumId);
            return album.Photos;
        }
    }
}
