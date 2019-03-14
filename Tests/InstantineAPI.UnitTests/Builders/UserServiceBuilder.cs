using System;
using FakeItEasy;
using InstantineAPI.Core;
using InstantineAPI.Core.Database;
using InstantineAPI.Core.Domain;
using InstantineAPI.Domain;

namespace InstantineAPI.UnitTests.Builders
{
    public class UserServiceBuilder
    {
        private IUnitOfWork _unitOfWork;
        private IEmailService _emailService = A.Dummy<IEmailService>();
        private IPasswordService _passwordService = A.Dummy<IPasswordService>();
        private DateTime _utcNow = DateTime.UtcNow;
        private Guid _guid = Guid.NewGuid();

        public IUserService Build()
        {
            _unitOfWork = _unitOfWork ?? new UnitOfWorkBuilder().Build();
            return new UserService(_unitOfWork, _emailService, new Clock(() => _utcNow), new GuidGenerator(() => _guid), A.Fake<IConstants>(), _passwordService, A.Fake<IEncryptionService>());
        }

        public UserServiceBuilder WithUnitOfWork(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            return this;
        }

        public UserServiceBuilder WithEmailService(IEmailService emailService)
        {
            _emailService = emailService;
            return this;
        }

        public UserServiceBuilder WithPasswordService(IPasswordService passwordService)
        {
            _passwordService = passwordService;
            return this;
        }

        public UserServiceBuilder WithDateTime(DateTime dateTime)
        {
            _utcNow = dateTime;
            return this;
        }

        public UserServiceBuilder WithGuid(Guid guid)
        {
            _guid = guid;
            return this;
        }
    }
}
