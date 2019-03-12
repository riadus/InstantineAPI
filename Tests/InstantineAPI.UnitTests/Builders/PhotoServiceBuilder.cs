using System;
using FakeItEasy;
using InstantineAPI.Core.Database;
using InstantineAPI.Core.Photos;
using InstantineAPI.Domain;
using InstantineAPI.Photos;

namespace InstantineAPI.UnitTests.Builders
{
    public class PhotoServiceBuilder
    {
        private IFtpService _ftpService = A.Dummy<IFtpService>();
        private IUnitOfWork _unitOfWork = A.Dummy<IUnitOfWork>();
        private IAlbumService _albumService = A.Dummy<IAlbumService>();
        private DateTime _datetime = DateTime.UtcNow;
        private Guid _guid = Guid.NewGuid();
        private IPermissionsService _permissionsService = A.Dummy<IPermissionsService>();

        public IPhotoService Build()
        {
            return new PhotoService(_ftpService, _unitOfWork, _albumService, new Clock(() => _datetime), new GuidGenerator(() => _guid), _permissionsService);
        }

        public PhotoServiceBuilder WithFtpService(IFtpService ftpService)
        {
            _ftpService = ftpService;
            return this;
        }

        public PhotoServiceBuilder WithUnitOfWork(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            return this;
        }

        public PhotoServiceBuilder WithAlbumService(IAlbumService albumService)
        {
            _albumService = albumService;
            return this;
        }

        public PhotoServiceBuilder WithPermissionsService(IPermissionsService permissionsService)
        {
            _permissionsService = permissionsService;
            return this;
        }

        public PhotoServiceBuilder WithDateTime(DateTime datetime)
        {
            _datetime = datetime;
            return this;
        }

        public PhotoServiceBuilder WithGuid(Guid guid)
        {
            _guid = guid;
            return this;
        }
    }
}
