using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using CENG382_TERM_PROJECT.Models;

namespace CENG382_TERM_PROJECT.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, "CENG382 Term Project"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            mail.To.Add(toEmail);

            using var client = new SmtpClient
            {
                Host = _settings.SmtpServer,
                Port = _settings.SmtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.SmtpLogin, _settings.SenderPassword)
            };

            await client.SendMailAsync(mail);
        }
    }
}
