using System.Threading.Tasks;
using InstantineAPI.Core.Domain;
using InstantineAPI.Core.Email;
using InstantineAPI.Data;

namespace InstantineAPI.Domain
{
    public class EmailService : IEmailService
    {
        private readonly IEmailSender _emailSender;
        private readonly ICodeGenerator _codeGenrator;

        public EmailService(IEmailSender emailSender, ICodeGenerator codeGenrator)
        {
            _emailSender = emailSender;
            _codeGenrator = codeGenrator;
        }

        public Task SendAccountCreationEmail(User user, string password)
        {
            var qrCode = _codeGenrator.GenerateImageFromCode(password);
            var email = new EmailObject
            {
                Recipient = user.Email,
                Subject = "Instantine pour des photos privées",
                Text = $"{user.Password}",
                DisplayName = $"{user.FirstName} {user.LastName}",
                QRCode = qrCode
            };

            return _emailSender.SendEmail(email);
        }
    }
}
