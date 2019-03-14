using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InstantineAPI.Data;

namespace InstantineAPI.Core.Domain
{
    public interface IUserService
    {
        Task RegisterDefaultAdmin();
        Task RegisterAdmin(User admin);
        Task RegisterManager(User manager);
        Task RegisterMembers(IEnumerable<User> members);
        Task SendEmailToUsers();
        Task SendAgainEmailToUsers();
        Task AcceptInvitation(string code);
        Task<User> GetUserFromId(string userId);
        Task<User> GetUserFromEmail(string userId);
        Task<User> Authenticate(string email, string code);
        Task ChangeUser(User user, UserChangeRequest userChangeRequest);
    }
}
