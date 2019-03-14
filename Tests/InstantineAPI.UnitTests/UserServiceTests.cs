using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using InstantineAPI.Core.Domain;
using InstantineAPI.Data;
using InstantineAPI.UnitTests.Builders;
using Xunit;

namespace InstantineAPI.UnitTests
{
    public class UserServiceTests
    {
        private User GetUser() => new User { Email = "mail@mail.com", FirstName = "John", LastName = "Doe" };

        [Fact]
        public async Task User_Should_Subscribe()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();

            var passwordService = A.Fake<IPasswordService>();
            A.CallTo(() => passwordService.GenerateRandomPassword()).Returns(("hash", "salt"));

            var dateTime = DateTime.UtcNow;
            var userService = new UserServiceBuilder().WithUnitOfWork(unitOfWork)
                            .WithDateTime(dateTime)
                            .WithPasswordService(passwordService)
                            .Build();

            var user = GetUser();

            await userService.RegisterMembers(new List<User> { user });
            var savedUser = await unitOfWork.Users.GetFirst(x => x.Email == user.Email);
            savedUser.Should().NotBeNull();
            savedUser.FirstName.Should().Be(user.FirstName);
            savedUser.LastName.Should().Be(user.LastName);
            savedUser.Password.Should().Be("hash");
            savedUser.PasswordSalt.Should().Be("salt");
            savedUser.CreationDate.Should().Be(dateTime);
        }

        [Fact]
        public async Task User_Should_Not_Subscribe_With_Same_Email()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();

            var passwordService = A.Fake<IPasswordService>();
            A.CallTo(() => passwordService.GenerateRandomPassword()).Returns(("hash", "salt"));

            var userService = new UserServiceBuilder().WithUnitOfWork(unitOfWork)
                            .WithPasswordService(passwordService)
                            .Build();

            var user = new User { Email = "mail@mail.com", FirstName = "John", LastName = "Doe" };
            var user2 = new User { Email = "mail@mail.com", FirstName = "Jane", LastName = "Doe" };
            var users = await unitOfWork.Users.GetAll();
            users.Count().Should().Be(0);
            await userService.RegisterMembers(new List<User> { user });
            users = await unitOfWork.Users.GetAll();
            users.Count().Should().Be(1);
            await userService.RegisterMembers(new List<User> { user2 });
            users = await unitOfWork.Users.GetAll();
            users.Count().Should().Be(1);
        }

        [Fact]
        public async Task UserService_Should_Return_ById()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();

            var userService = new UserServiceBuilder().WithUnitOfWork(unitOfWork)
                            .Build();
            var user = GetUser();
            user.Password = "code";
            user.UserId = "userId";

            await unitOfWork.Users.Add(user);


            var savedUser = await userService.GetUserFromId("userId");
            savedUser.Should().NotBeNull();
            savedUser.Password.Should().Be("code");
            savedUser.UserId.Should().Be("userId");
        }

        [Fact]
        public async Task UserService_Should_Return_ByEmail()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();

            var userService = new UserServiceBuilder().WithUnitOfWork(unitOfWork)
                            .Build();
            var user = GetUser();
            user.Password = "code";
            user.UserId = "userId";

            await unitOfWork.Users.Add(user);

            var savedUser = await userService.GetUserFromEmail(user.Email);
            savedUser.Should().NotBeNull();
            savedUser.Password.Should().Be("code");
            savedUser.UserId.Should().Be("userId");
        }

        [Fact]
        public async Task UserService_Should_Send_Email_To_New_Users()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var emailService = A.Fake<IEmailService>();

            var userService = new UserServiceBuilder().WithUnitOfWork(unitOfWork)
                            .WithEmailService(emailService)
                            .Build();

            var user = GetUser();
            user.Password = "pwd";
            user.InvitationSent = false;
            await unitOfWork.Users.Add(user);

            await userService.SendEmailToUsers();
            A.CallTo(() => emailService.SendAccountCreationEmail(user, "pwd")).MustHaveHappened();
        }

        [Fact]
        public async Task UserService_Should_Not_Send_Email_To_Invited_Users()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var emailService = A.Fake<IEmailService>();

            var userService = new UserServiceBuilder().WithUnitOfWork(unitOfWork)
                            .WithEmailService(emailService)
                            .Build();

            var user = GetUser();
            user.InvitationSent = true;
            user.Password = "pwd";
            await unitOfWork.Users.Add(user);

            await userService.SendEmailToUsers();
            A.CallTo(() => emailService.SendAccountCreationEmail(user, "pwd")).MustNotHaveHappened();
        }

        [Fact]
        public async Task UserService_Should_Not_ReSend_Email_To_New_Users()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var emailService = A.Fake<IEmailService>();

            var userService = new UserServiceBuilder().WithUnitOfWork(unitOfWork)
                            .WithEmailService(emailService)
                            .Build();

            var user = GetUser();
            user.InvitationSent = false;
            user.Password = "pwd";
            await unitOfWork.Users.Add(user);

            await userService.SendAgainEmailToUsers();
            A.CallTo(() => emailService.SendAccountCreationEmail(user, "pwd")).MustNotHaveHappened();
        }

        [Fact]
        public async Task UserService_Should_ReSend_Email_To_Invited_Users()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var emailService = A.Fake<IEmailService>();

            var userService = new UserServiceBuilder().WithUnitOfWork(unitOfWork)
                            .WithEmailService(emailService)
                            .Build();

            var user = GetUser();
            user.InvitationSent = true;
            user.Password = "pwd";
            await unitOfWork.Users.Add(user);

            await userService.SendAgainEmailToUsers();
            A.CallTo(() => emailService.SendAccountCreationEmail(user, "pwd")).MustHaveHappened();
        }

        [Fact]
        public async Task UserService_Should_Not_ReSend_Email_To_Active_Users()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var emailService = A.Fake<IEmailService>();

            var userService = new UserServiceBuilder().WithUnitOfWork(unitOfWork)
                            .WithEmailService(emailService)
                            .Build();

            var user = GetUser();
            user.InvitationSent = true;
            user.InvitationAccepted = true;
            user.Password = "pwd";
            await unitOfWork.Users.Add(user);

            await userService.SendAgainEmailToUsers();
            A.CallTo(() => emailService.SendAccountCreationEmail(user, "pwd")).MustNotHaveHappened();
        }

        [Fact]
        public async Task Invited_User_Should_Be_Flagged()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var emailService = A.Fake<IEmailService>();
            var datetime = DateTime.UtcNow;

            var userService = new UserServiceBuilder().WithUnitOfWork(unitOfWork)
                            .WithEmailService(emailService)
                                .WithDateTime(datetime)
                            .Build();

            var user = GetUser();
            await unitOfWork.Users.Add(user);
            await userService.SendEmailToUsers();
            var savedUser = await unitOfWork.Users.GetFirst(x => x.Email == user.Email);
            savedUser.InvitationSent.Should().BeTrue();
            savedUser.SendingDate.Should().Be(datetime);
        }

        [Fact]
        public async Task Accepting_Invitation_Should_Be_Flagged()
        {
            var unitOfWork = new UnitOfWorkBuilder().Build();
            var datetime = DateTime.UtcNow;
            var userService = new UserServiceBuilder()
                                .WithDateTime(datetime)
                                .WithUnitOfWork(unitOfWork)
                            .Build();
            var user = GetUser();
            user.Password = "code";
            await unitOfWork.Users.Add(user);

            await userService.AcceptInvitation("code");
            var savedUser = await unitOfWork.Users.GetFirst(x => x.Email == user.Email);
            savedUser.InvitationAccepted.Should().BeTrue();
            savedUser.AcceptingDate.Should().Be(datetime);
        }
    }
}
