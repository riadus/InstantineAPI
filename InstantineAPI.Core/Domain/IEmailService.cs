using System.Threading.Tasks;
using InstantineAPI.Data;

namespace InstantineAPI.Core.Domain
{
    public interface IEmailService
    {
        Task SendAccountCreationEmail(User user, byte[] QRCode);
    }
}
