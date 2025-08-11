using GestionProduit.Domain.Entities;

namespace GestionProduit.Infrastructure.Interfaces;

public interface IProduitService
{
    Task<List<Produit>> GetAllProduitsAsync();
    Task<Produit?> GetProduitByIdAsync(int id);
    Task AjouterProduitAsync(Produit produit);
    Task ModifierProduitAsync(Produit produit);
    Task SupprimerProduitAsync(int id);
}
