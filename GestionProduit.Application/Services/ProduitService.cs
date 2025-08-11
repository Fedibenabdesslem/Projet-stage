using GestionProduit.Infrastructure.Interfaces;
using GestionProduit.Domain.Entities;
using GestionProduit.Domain.Interfaces;

namespace GestionProduit.Infrastructure.Services;

public class ProduitService : IProduitService
{
    private readonly IProduitRepository _repository;

    public ProduitService(IProduitRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Produit>> GetAllProduitsAsync() => _repository.GetAllAsync();

    public Task<Produit?> GetProduitByIdAsync(int id) => _repository.GetByIdAsync(id);

    public Task AjouterProduitAsync(Produit produit) => _repository.AddAsync(produit);

    public Task ModifierProduitAsync(Produit produit) => _repository.UpdateAsync(produit);

    public Task SupprimerProduitAsync(int id) => _repository.DeleteAsync(id);
}
