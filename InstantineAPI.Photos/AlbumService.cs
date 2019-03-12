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
    public class AlbumService : IAlbumService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;
        private readonly IGuid _guid;

        public AlbumService(IUnitOfWork unitOfWork, 
                            IClock clock,
                            IGuid guid)
        {
            _unitOfWork = unitOfWork;
            _clock = clock;
            _guid = guid;
        }

        public async Task<bool> AddAdmin(string albumId, User newAdmin, User requestor)
        {
            var album = await GetAlbum(albumId);
            if(album?.Creator != requestor)
            {
                return false;
            }
            album.AlbumAdmins = album.AlbumAdmins ?? new List<AlbumAdmin>();
            if(!album.AlbumAdmins.Any(x => x.Admin == newAdmin))
            {
                var albumAdmin = new AlbumAdmin
                {
                    Album = album,
                    Admin = newAdmin
                };
                album.AlbumAdmins.Add(albumAdmin);
            }
            await _unitOfWork.Albums.Update(album);
            return true;
        }

        public async Task<bool> AddFollower(string albumId, User follower, User requestor)
        {
            var album = await GetAlbum(albumId);
            if (album?.Creator != requestor)
            {
                return false;
            }
            album.AlbumFollowers = album.AlbumFollowers ?? new List<AlbumFollower>();
            if (!album.AlbumFollowers.Any(x => x.Follower == follower))
            {
                var albumFollower = new AlbumFollower
                {
                    Album = album,
                    Follower = follower
                };
                album.AlbumFollowers.Add(albumFollower);
            }
            await _unitOfWork.Albums.Update(album);
            return true;
        }

        public async Task<Album> CreateAlbum(User creator, string name)
        {
            if (await _unitOfWork.Albums.Any(x => x.Name == name))
            {
                return null;
            }
            var album = new Album
            {
                Name = name,
                AlbumId = _guid.NewGuid().ToString(),
                Creator = creator,
                CreationDate = _clock.UtcNow
            };

            await _unitOfWork.Albums.Add(album);
            return album;
        }

        public async Task<IEnumerable<User>> GetAdmins(string albumId)
        {
            var album = await GetAlbum(albumId);
            return album?.AlbumAdmins.Select(x => x.Admin);
        }

        public Task<Album> GetAlbum(string albumId)
        {
            return _unitOfWork.Albums.GetFirst(x => x.AlbumId == albumId);
        }

        public async Task<IEnumerable<Album>> GetAlbums(User user)
        {
            var albums = await _unitOfWork.Albums.GetAll(); 
            return albums.Where(x => x.AlbumAdmins.Any(a => a.Admin == user) || x.AlbumFollowers.Any(a => a.Follower == user) || x.Creator == user);
        }

        public async Task<IEnumerable<User>> GetFollowers(string albumId)
        {
            var album = await GetAlbum(albumId);
            return album?.AlbumFollowers.Select(x => x.Follower);
        }

        public async Task<bool> RemoveAdmin(string albumId, User newAdmin, User requestor)
        {
            var album = await GetAlbum(albumId);
            if (album?.Creator == requestor || (newAdmin == requestor && album?.Creator != requestor))
            {
                var albumAdmin = album.AlbumAdmins.FirstOrDefault(a => a.Admin == newAdmin);
                if (albumAdmin != null)
                {
                    album.AlbumAdmins.Remove(albumAdmin);
                }
                await _unitOfWork.Albums.Update(album);
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveFollower(string albumId, User follower, User requestor)
        {
            var album = await GetAlbum(albumId);
            if (album?.Creator == requestor || (follower == requestor && album?.Creator != requestor))
            {
                var albumFollower = album.AlbumFollowers.FirstOrDefault(a => a.Follower == follower);
                if (albumFollower != null)
                {
                    album.AlbumFollowers.Remove(albumFollower);
                }

                await _unitOfWork.Albums.Update(album);
                return true;
            }
            return false;
        }
    }
}
