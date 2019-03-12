using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly ICodeGenerator _codeGenerator;

        public UserService(IUnitOfWork unitOfWork,
                           IEmailService emailService,
                           IClock clock,
                           IGuid guid,
                           ICodeGenerator codeGenerator)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _clock = clock;
            _guid = guid;
            _codeGenerator = codeGenerator;
        }

        public async Task SubscribeUsers(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                await SubscribeUser(user);
            }
        }

        private async Task SubscribeUser(User user)
        {
            if(await _unitOfWork.Users.Any(x => x.Email == user.Email))
            {
                return;
            }
            user.UserId = _guid.NewGuid().ToString();
            user.CreationDate = _clock.UtcNow;
            user.Code = _codeGenerator.GenerateRandomCode();
            await _unitOfWork.Users.Add(user);
        }

        private Task SendEmail(User user)
        {
            var qrCode = _codeGenerator.GenrateImageFromCode(user.Code);
            return _emailService.SendAccountCreationEmail(user, qrCode);
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
            var user = await _unitOfWork.Users.GetFirst(x => x.Code == code);
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

        public Task<User> Authenticate(string email, string password)
        {
            return _unitOfWork.Users.GetFirst(x => x.Email == email && x.Code == password);   
        }
    }
}
