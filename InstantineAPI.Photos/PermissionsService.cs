using System;
using System.Linq;
using System.Threading.Tasks;
using InstantineAPI.Core.Database;
using InstantineAPI.Core.Photos;
using InstantineAPI.Data;

namespace InstantineAPI.Photos
{
    public class PermissionsService : IPermissionsService
    {
        private IUnitOfWork _unitOfWork;

        public PermissionsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Album> GetAlbum(string albumId)
        {
            return _unitOfWork.Albums.GetFirst(x => x.AlbumId == albumId);
        }

        public async Task<bool> CanSeePicture(User user, string albumId)
        {
            var album = await GetAlbum(albumId);
            if (album == null)
            {
                return false;
            }
            return album.AlbumAdmins.Any(a => a.Admin == user) || album.AlbumFollowers.Any(a => a.Follower == user) || album.Creator == user;
        }

        public async Task<bool> CanAddPicture(User user, string albumId)
        {
            var album = await GetAlbum(albumId);
            if (album == null)
            {
                return false;
            }
            return album.AlbumAdmins.Any(a => a.Admin == user) || album.Creator == user;
        }

        public async Task<bool> CanDeletePicture(User user, string albumId)
        {
            var album = await GetAlbum(albumId);
            if (album == null)
            {
                return false;
            }
            return album.AlbumAdmins.Any(a => a.Admin == user) || album.Creator == user;
        }

        public async Task<bool> CanComment(User user, string albumId)
        {
            var album = await GetAlbum(albumId);
            if (album == null)
            {
                return false;
            }
            return album.AlbumAdmins.Any(a => a.Admin == user) || album.AlbumFollowers.Any(a => a.Follower == user) || album.Creator == user;
        }

        public async Task<bool> CanLike(User user, string albumId)
        {
            var album = await GetAlbum(albumId);
            if (album == null)
            {
                return false;
            }
            return album.AlbumAdmins.Any(a => a.Admin == user) || album.AlbumFollowers.Any(a => a.Follower == user) || album.Creator == user;
        }

        public async Task<bool> CanDeleteComment(User user, Comment comment, string albumId)
        {
            var album = await GetAlbum(albumId);
            return album.Creator == user || comment.Reactor == user;
        }

        public async Task<bool> CanUnLike(User user, Like like, string albumId)
        {
            var album = await GetAlbum(albumId);
            return album.Creator == user || like.Reactor == user;
        }
    }
}
