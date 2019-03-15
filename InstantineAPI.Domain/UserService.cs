using System.Collections.Generic;
using System.Threading.Tasks;
using InstantineAPI.Core;
using InstantineAPI.Core.Database;
using InstantineAPI.Core.Domain;
using InstantineAPI.Data;

namespace InstantineAPI.Domain
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IClock _clock;
        private readonly IGuid _guid;
        private readonly IConstants _constants;
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;

        public UserService(IUnitOfWork unitOfWork,
                           IEmailService emailService,
                           IClock clock,
                           IGuid guid,
                           IConstants constants,
                           IPasswordService passwordService,
                           IEncryptionService encryptionService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _clock = clock;
            _guid = guid;
            _constants = constants;
            _passwordService = passwordService;
            _encryptionService = encryptionService;
        }

        public async Task RegisterMembers(IEnumerable<User> members)
        {
            foreach (var member in members)
            {
                var (hash, salt) = _passwordService.GenerateRandomPassword();
                member.Password = hash;
                member.PasswordSalt = salt;
                member.Role = UserRole.Member;
                await SaveUser(member);
            }
        }

        private async Task SaveUser(User user)
        {
            if(await _unitOfWork.Users.Any(x => x.Email == user.Email))
            {
                return;
            }
            user.UserId = _guid.NewGuid().ToString();
            user.CreationDate = _clock.UtcNow;

            await _unitOfWork.Users.Add(user);
        }

        private Task SendEmail(User user)
        {
             return _emailService.SendAccountCreationEmail(user, user.Password);
        }

        public async Task SendEmailToUsers()
        {
            var users = await _unitOfWork.Users.GetAll(x => !x.InvitationSent);
            await SendEmailToUsersAndUpdate(users);
        }

        private async Task SendEmailToUsersAndUpdate(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                await SendEmail(user);
                user.InvitationSent = true;
                user.SendingDate = _clock.UtcNow;
                await _unitOfWork.Users.Update(user);
            }
        }

        public async Task SendAgainEmailToUsers()
        {
            var users = await _unitOfWork.Users.GetAll(x => x.InvitationSent && !x.InvitationAccepted);
            await SendEmailToUsersAndUpdate(users);
        }

        public async Task AcceptInvitation(string code)
        {
            var user = await _unitOfWork.Users.GetFirst(x => x.Password == code);
            user.InvitationAccepted = true;
            user.AcceptingDate = _clock.UtcNow;
            await _unitOfWork.Users.Update(user);
        }

        public Task<User> GetUserFromId(string userId)
        {
            return _unitOfWork.Users.GetFirst(x => x.UserId == userId);
        }

        public Task<User> GetUserFromEmail(string email)
        {
            return _unitOfWork.Users.GetFirst(x => x.Email == email);
        }

        public async Task<User> Authenticate(string email, string password)
        {
            var user = await _unitOfWork.Users.GetFirst(x => x.Email == email);
            var decryptedPassword = _encryptionService.StringDecrypt(password);
            if (_passwordService.VerifyPasswordHash(decryptedPassword, user.Password, user.PasswordSalt))
            {
                return user;
            }
            return null;
        }

        public Task RegisterAdmin(User admin)
        {
            var (hash, salt) = _passwordService.GenerateRandomPassword();
            admin.Role = UserRole.Admin;
            admin.Password = hash;
            admin.PasswordSalt = salt;
            return SaveUser(admin);
        }

        public Task RegisterManager(User manager)
        {
            var (hash, salt) = _passwordService.GenerateRandomPassword();
            manager.Role = UserRole.Manager;
            manager.Password = hash;
            manager.PasswordSalt = salt;
            return SaveUser(manager);
        }

        public Task RegisterDefaultAdmin()
        {
            var (hash, salt) = _passwordService.CreatePasswordHash(_constants.AdminPwd);
            var admin = new User
            {
                FirstName = "Admin",
                Email = _constants.AdminEmail,
                Password = hash,
                PasswordSalt = salt,
                Role = UserRole.Admin
            };
            return SaveUser(admin);
        }

        public async Task ChangeUser(User user, UserChangeRequest userChangeRequest)
        {
            user.FirstName = userChangeRequest.ChangeFirstName ? userChangeRequest.FirstName : user.FirstName;
            user.LastName = userChangeRequest.ChangeLastName ? userChangeRequest.LastName : user.LastName;
            if (userChangeRequest.ChangePassword)
            {
                var (hash, salt) = _passwordService.CreatePasswordHash(userChangeRequest.NewPassword);
                user.Password = hash;
                user.PasswordSalt = salt;
            }
            await _unitOfWork.Users.Update(user);
        }

        public async Task UpdateRefreshToken(User user, string refreshToken)
        {
            var (hash, salt) = _passwordService.CreatePasswordHash(refreshToken);
            user.RefreshToken = hash;
            user.RefreshTokenSalt = salt;
            await _unitOfWork.Users.Update(user);
        }

        public bool VerifyRefreshToken(User user, string refreshToken)
        {
            return _passwordService.VerifyPasswordHash(refreshToken, user.RefreshToken, user.RefreshTokenSalt);
        }
    }
}
