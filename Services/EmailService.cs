using Core.Helpers;
using Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;

namespace Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            this.emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            string? email = emailSettings.Email;
            var password = emailSettings.Password;

            var client = new SmtpClient(emailSettings.SmtpServer)
            {
                Port = 587,
                Credentials = new NetworkCredential(email, password),
                UseDefaultCredentials = false,
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(email),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);
            await client.SendMailAsync(mailMessage);
        }
        public string GenerateVerificatonCode(int length = 6)
        {
            var randomNumber = new byte[length];
            RandomNumberGenerator.Fill(randomNumber);

            return string.Join("", randomNumber.Select(b => (b % 10).ToString()));
        }
    }
}
