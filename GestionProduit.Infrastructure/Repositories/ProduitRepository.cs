using GestionProduit.Domain.Entities;
using GestionProduit.Domain.Interfaces;
using GestionProduit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionProduit.Infrastructure.Repositories;

public class ProduitRepository : IProduitRepository
{
    private readonly ApplicationDbContext _context;

    public ProduitRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Produit>> GetAllAsync()
    {
        return await _context.Produits.ToListAsync();
    }

    public async Task<Produit?> GetByIdAsync(int id)
    {
        return await _context.Produits.FindAsync(id);
    }

    public async Task AddAsync(Produit produit)
    {
        _context.Produits.Add(produit);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Produit produit)
    {
        _context.Produits.Update(produit);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var produit = await _context.Produits.FindAsync(id);
        if (produit != null)
        {
            _context.Produits.Remove(produit);
            await _context.SaveChangesAsync();
        }
    }
}
