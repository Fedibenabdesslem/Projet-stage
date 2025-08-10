using System.Threading.Tasks;

namespace GestionProduit.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, string? Token, string? ErrorMessage)> SignInAsync(string email, string password);

        Task<(bool IsSuccess, string? ErrorMessage)> RegisterAsync(string username, string email, string password);
    }
}
