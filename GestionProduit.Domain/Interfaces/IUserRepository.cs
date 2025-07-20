using GestionProduit.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace GestionProduit.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid userId);

        Task SaveChangesAsync();  // <-- Cette méthode doit être implémentée
    }
}
