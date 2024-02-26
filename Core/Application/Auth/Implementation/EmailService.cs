using Authentication.Core.Application.Auth.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Authentication.Core.Application.Auth.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string toEmail, string subject, string content, bool isBodyHtml)
        {
            string MailServe = _configuration["EmailSettings:MailServer"];
            string FromEmail = _configuration["EmailSettings:FromEmail"];
            string Password = _configuration["EmailSettings:Password"];
            int Port = int.Parse(_configuration["EmailSettings:MailPort"]);

            var client = new SmtpClient(MailServe, Port)
            {
                Credentials = new NetworkCredential(FromEmail, Password),
                EnableSsl = true,
            };
            MailMessage mailMessage = new MailMessage(FromEmail, toEmail, subject, content)
            {
                IsBodyHtml = isBodyHtml
            };
            return client.SendMailAsync(mailMessage);
        }
    }
}
