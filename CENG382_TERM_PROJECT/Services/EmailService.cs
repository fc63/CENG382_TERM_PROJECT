using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using CENG382_TERM_PROJECT.Models;

namespace CENG382_TERM_PROJECT.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ISystemLogService _systemLogService;

        public EmailService(IOptions<EmailSettings> options, ISystemLogService systemLogService)
        {
            _settings = options.Value;
            _systemLogService = systemLogService;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // maili geçici olarak iptal etmek için
                return;
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

                await _systemLogService.LogAsync(null, "SendEmail",
                    $"Email sent successfully to {toEmail}. Subject: {subject}.", true);
            }
            catch (Exception ex)
            {
                await _systemLogService.LogAsync(null, "SendEmail",
                    $"Failed to send email to {toEmail}. Subject: {subject}. Error: {ex.Message}", false);
                throw;
            }
        }
    }
}
