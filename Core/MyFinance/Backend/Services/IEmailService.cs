using System.Threading.Tasks;

namespace MyFinance.Backend.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false);
    }
}
