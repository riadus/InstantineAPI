using System;
using System.Net;
using System.Threading.Tasks;
using InstantineAPI.Core.Email;
using InstantineAPI.Data;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace InstantineAPI.Email
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmail(EmailObject email)
        {
            var apiKey = "";
            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress("noreplay@instantine.io", "Instantine Team"),
                Subject = email.Subject,
                PlainTextContent = email.Text,
            };
            msg.AddTo(new EmailAddress(email.Recipient, email.DisplayName));

            var qrCode = Convert.ToBase64String(email.QRCode);
            msg.AddAttachment("Code à scanner.png", qrCode);

            var response = await client.SendEmailAsync(msg);
        }
    }
}
