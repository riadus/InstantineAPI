using System.Collections.Generic;
using System.Threading.Tasks;
using InstantineAPI.Data;

namespace InstantineAPI.Core.Photos
{
    public interface IAlbumService
    {
        Task<Album> CreateAlbum(User creator, string name);
        Task<IEnumerable<Album>> GetAlbums(User user);
        Task<Album> GetAlbum(string albumId);

        Task<bool> AddFollower(string albumId, User follower, User requestor);
        Task<bool> RemoveFollower(string albumId, User follower, User requestor);

        Task<bool> AddAdmin(string albumId, User newAdmin, User requestor);
        Task<bool> RemoveAdmin(string albumId, User newAdmin, User requestor);

        Task<IEnumerable<User>> GetFollowers(string albumId);
        Task<IEnumerable<User>> GetAdmins(string albumId);
    }
}
