using System.Threading.Tasks;

namespace GestionProduit.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, string? Token, string? ErrorMessage)> SignInAsync(string email, string password);
    }
}
