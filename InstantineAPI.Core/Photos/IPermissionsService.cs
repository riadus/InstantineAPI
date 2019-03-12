using System.Threading.Tasks;
using InstantineAPI.Data;

namespace InstantineAPI.Core.Photos
{
    public interface IPermissionsService
    {
        Task<bool> CanSeePicture(User user, string albumId);
        Task<bool> CanAddPicture(User user, string albumId);
        Task<bool> CanDeletePicture(User user, string albumId);
        Task<bool> CanComment(User user, string albumId);
        Task<bool> CanDeleteComment(User user, Comment comment, string albumId);
        Task<bool> CanLike(User user, string albumId);
        Task<bool> CanUnLike(User user, Like like, string albumId);
    }
}
