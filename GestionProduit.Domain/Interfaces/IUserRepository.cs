using GestionProduit.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionProduit.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task<List<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid userId);
        Task SaveChangesAsync();
    }
}
