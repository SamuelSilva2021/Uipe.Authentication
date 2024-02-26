namespace Authentication.Core.Application.Auth.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string content, bool isBodyHtml);
    }
}
