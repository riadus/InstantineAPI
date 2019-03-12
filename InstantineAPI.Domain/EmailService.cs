using System.Threading.Tasks;
using InstantineAPI.Core.Domain;
using InstantineAPI.Core.Email;
using InstantineAPI.Data;

namespace InstantineAPI.Domain
{
    public class EmailService : IEmailService
    {
        private readonly IEmailSender _emailSender;

        public EmailService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public Task SendAccountCreationEmail(User user, byte[] QRCode)
        {
            var email = new EmailObject
            {
                Recipient = user.Email,
                Subject = "Instantine pour des photos privées",
                Text = $"{user.Code}",
                DisplayName = $"{user.FirstName} {user.LastName}",
                QRCode = QRCode
            };

            return _emailSender.SendEmail(email);
        }
    }
}
