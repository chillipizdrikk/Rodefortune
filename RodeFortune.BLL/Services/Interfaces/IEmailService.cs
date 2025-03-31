using System.Threading.Tasks;

namespace RodeFortune.BLL.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlMessage);
        Task SendPasswordResetEmailAsync(string email, string callbackUrl);
    }
}