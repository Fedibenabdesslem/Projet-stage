using GestionProduit.Application.DTOs; // ← obligatoire
using System.Threading.Tasks;

namespace GestionProduit.Application.Interfaces
{
    public interface IAuthService
    {
        Task<SignInResponseDto> SignInAsync(string email, string password);
        Task<(bool IsSuccess, string? ErrorMessage)> RegisterAsync(string username, string email, string password);
    }
}
