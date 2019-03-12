using System;
using FakeItEasy;
using InstantineAPI.Core.Database;
using InstantineAPI.Core.Domain;
using InstantineAPI.Core.Photos;
using InstantineAPI.Domain;
using InstantineAPI.Photos;

namespace InstantineAPI.UnitTests.Builders
{
    public class ReactionServiceBuilder
    {
        private IUnitOfWork _unitOfWork;
        private IPhotoService _photoService = A.Dummy<IPhotoService>();
        private IPermissionsService _permissionService = A.Dummy<IPermissionsService>();
        private DateTime _dateTime = DateTime.UtcNow;
        private Guid _guid = Guid.NewGuid();
        private IUserService _userService = A.Dummy<IUserService>();

        public IReactionService Build()
        {
            _unitOfWork = _unitOfWork ?? new UnitOfWorkBuilder().Build();
            return new ReactionService(_unitOfWork, _photoService, _permissionService, new Clock(() => _dateTime), new GuidGenerator(() => _guid), _userService);
        }

        public ReactionServiceBuilder WithUnitOfWork(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            return this;
        }

        public ReactionServiceBuilder WithPhotoService(IPhotoService photoService)
        {
            _photoService = photoService;
            return this;
        }

        public ReactionServiceBuilder WithPermissionsService(IPermissionsService permissionsService)
        {
            _permissionService = permissionsService;
            return this;
        }

        public ReactionServiceBuilder WithDateTime(DateTime dateTime)
        {
            _dateTime = dateTime;
            return this;
        }

        public ReactionServiceBuilder WithGuid(Guid guid)
        {
            _guid = guid;
            return this;
        }

        public ReactionServiceBuilder WithUserService(IUserService userService)
        {
            _userService = userService;
            return this;
        }
    }
}
