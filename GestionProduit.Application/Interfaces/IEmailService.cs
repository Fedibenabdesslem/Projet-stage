using System.Threading.Tasks;

namespace GestionProduit.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
