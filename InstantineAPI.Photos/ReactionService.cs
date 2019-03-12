using System.Linq;
using System.Threading.Tasks;
using InstantineAPI.Core.Database;
using InstantineAPI.Core.Domain;
using InstantineAPI.Core.Photos;
using InstantineAPI.Data;

namespace InstantineAPI.Photos
{
    public class ReactionService : IReactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        private readonly IPermissionsService _permissionsService;
        private readonly IClock _clock;
        private readonly IGuid _guid;
        private readonly IUserService _userService;

        public ReactionService(IUnitOfWork unitOfWork,
                               IPhotoService photoService,
                               IPermissionsService permissionsService,
                               IClock clock,
                               IGuid guid,
                               IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _permissionsService = permissionsService;
            _clock = clock;
            _guid = guid;
            _userService = userService;
        }

        public async Task<Comment> AddComment(string photoId, string albumId, string message, User requestor)
        {
            if(!await _permissionsService.CanComment(requestor, albumId))
            {
                return null;
            }
            var photo = await _photoService.GetPhoto(photoId);
            var comment = new Comment
            {
                ReactionId = _guid.NewGuid().ToString(),
                Text = message,
                Reactor = requestor,
                ReactionDate = _clock.UtcNow
            };
            var photoComment = new PhotoComment
            {
                Photo = photo,
                Comment = comment
            };
            photo.PhotoComments.Add(photoComment);
            await _unitOfWork.Photos.Update(photo);
            return comment;
        }

        public async Task<bool> DeleteComment(string photoId, string albumId, string commentId, User requestor)
        {
            var photo = await _photoService.GetPhoto(photoId);
            var comment = await _unitOfWork.Comments.GetFirst(x => x.ReactionId == commentId);
            if (!await _permissionsService.CanDeleteComment(requestor, comment, albumId))
            {
                return false;
            }
            var photoComment = photo.PhotoComments.FirstOrDefault(x => x.Comment == comment);
            photo.PhotoComments.Remove(photoComment);
            await _unitOfWork.Comments.Delete(comment);
            await _unitOfWork.Photos.Update(photo);
            return true;
        }

        public async Task<Like> LikePhoto(string photoId, string albumId, User requestor)
        {
            if (!await _permissionsService.CanLike(requestor, albumId))
            {
                return null;
            }
            var photo = await _photoService.GetPhoto(photoId);
            var like = new Like
            {
                ReactionId = _guid.NewGuid().ToString(),
                Reactor = requestor,
                ReactionDate = _clock.UtcNow
            };
            var photoLike = new PhotoLike
            {
                Photo = photo,
                Like = like
            };
            photo.PhotoLikes.Add(photoLike);
            await _unitOfWork.Photos.Update(photo);
            return like;
        }

        public async Task<bool> UnlikePhoto(string photoId, string albumId, User requestor)
        {
            var photo = await _photoService.GetPhoto(photoId);
            var like = await _unitOfWork.Likes.GetFirst(x => x.Reactor == requestor);
            if (!await _permissionsService.CanUnLike(requestor, like, albumId))
            {
                return false;
            }
            var photoLike = photo.PhotoLikes.FirstOrDefault(x => x.Like == like);
            photo.PhotoLikes.Remove(photoLike);
            await _unitOfWork.Likes.Delete(like);
            await _unitOfWork.Photos.Update(photo);
            return true;
        }
    }
}
