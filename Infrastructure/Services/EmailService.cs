using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var email = _configuration["EmailSettings:Email"];
            var password = _configuration["EmailSettings:Password"];

            var client = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
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
    }
}
