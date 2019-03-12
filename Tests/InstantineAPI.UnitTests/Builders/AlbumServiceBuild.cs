using System;
using InstantineAPI.Core.Database;
using InstantineAPI.Core.Photos;
using InstantineAPI.Domain;
using InstantineAPI.Photos;

namespace InstantineAPI.UnitTests.Builders
{
    public class AlbumServiceBuild
    {
        private IUnitOfWork _unitOfWork;
        private DateTime _utcNow;
        private Guid _guid;

        public IAlbumService Build()
        {
            _unitOfWork = _unitOfWork ?? new UnitOfWorkBuilder().Build();
            return new AlbumService(_unitOfWork, new Clock(() => _utcNow), new GuidGenerator(() => _guid));
        }

        public AlbumServiceBuild WithUnitOfWork(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            return this;
        }

        public AlbumServiceBuild WithDateTime(DateTime dateTime)
        {
            _utcNow = dateTime;
            return this;
        }

        public AlbumServiceBuild WithGuid(Guid guid)
        {
            _guid = guid;
            return this;
        }
    }
}
