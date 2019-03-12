using System.Threading.Tasks;
using InstantineAPI.Data;

namespace InstantineAPI.Core.Email
{
    public interface IEmailSender
    {
        Task SendEmail(EmailObject email);
    }
}
