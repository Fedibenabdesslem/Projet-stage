using GestionProduit.Domain.Entities;

namespace GestionProduit.Domain.Interfaces;

public interface IProduitRepository
{
    Task<List<Produit>> GetAllAsync();
    Task<Produit?> GetByIdAsync(int id);
    Task AddAsync(Produit produit);
    Task UpdateAsync(Produit produit);
    Task DeleteAsync(int id);
}
