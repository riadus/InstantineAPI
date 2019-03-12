using System;
using System.Threading.Tasks;
using InstantineAPI.Core.Photos;
using InstantineAPI.Data;

namespace InstantineAPI.UnitTests.Mock
{
    public abstract class AllSamePermissionsService : IPermissionsService
    {
        private readonly bool _permission;

        protected AllSamePermissionsService(bool permission)
        {
            _permission = permission;
        }

        public Task<bool> CanAddPicture(User user, string albumId)
        {
            return Task.FromResult(_permission);
        }

        public Task<bool> CanComment(User user, string albumId)
        {
            return Task.FromResult(_permission);
        }

        public Task<bool> CanDeleteComment(User user, Comment comment, string albumId)
        {
            return Task.FromResult(_permission);
        }

        public Task<bool> CanDeletePicture(User user, string albumId)
        {
            return Task.FromResult(_permission);
        }

        public Task<bool> CanLike(User user, string albumId)
        {
            return Task.FromResult(_permission);
        }

        public Task<bool> CanSeePicture(User user, string albumId)
        {
            return Task.FromResult(_permission);
        }

        public Task<bool> CanUnLike(User user, Like like, string albumId)
        {
            return Task.FromResult(_permission);
        }
    }
}
