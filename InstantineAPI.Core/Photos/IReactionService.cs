using System.Threading.Tasks;
using InstantineAPI.Data;

namespace InstantineAPI.Core.Photos
{
    public interface IReactionService
    {
        Task<Like> LikePhoto(string photoId, string albumId, User requestor);
        Task<bool> UnlikePhoto(string photoId, string albumId, User requestor);
        Task<Comment> AddComment(string photoId, string albumId, string message, User requestor);
        Task<bool> DeleteComment(string photoId, string albumId, string commentId, User requestor);
    }
}
