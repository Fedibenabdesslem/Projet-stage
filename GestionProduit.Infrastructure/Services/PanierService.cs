using GestionProduit.Application.DTOs;
using GestionProduit.Application.Interfaces;
using GestionProduit.Domain.Entities;
using GestionProduit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionProduit.Infrastructure.Services
{
    public class PanierService : IPanierService
    {
        private readonly ApplicationDbContext _context;

        public PanierService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PanierDto>> GetPanierByUserAsync(Guid userId)
        {
            return await _context.PanierItems
                .Include(p => p.Produit)
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .Select(p => new PanierDto
                {
                    Id = p.Id,
                    ProduitId = p.ProduitId,
                    ProduitNom = p.Produit != null ? p.Produit.Nom : "Inconnu",
                    UserId = p.UserId,
                    Username = p.User != null ? p.User.Username : "Inconnu", // ? corrigé
                    Quantite = p.Quantite
                })
                .ToListAsync();
        }

        public async Task<PanierDto> AjouterAuPanierAsync(Guid userId, int produitId, int quantite)
        {
            var existingItem = await _context.PanierItems
                .Include(p => p.Produit)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId && p.ProduitId == produitId);

            if (existingItem != null)
            {
                existingItem.Quantite += quantite;
                _context.PanierItems.Update(existingItem);
                await _context.SaveChangesAsync();

                return new PanierDto
                {
                    Id = existingItem.Id,
                    ProduitId = existingItem.ProduitId,
                    ProduitNom = existingItem.Produit?.Nom ?? "Inconnu",
                    UserId = existingItem.UserId,
                    Username = existingItem.User?.Username ?? "Inconnu", // ? corrigé
                    Quantite = existingItem.Quantite
                };
            }

            var panierItem = new PanierItem
            {
                UserId = userId,
                ProduitId = produitId,
                Quantite = quantite
            };

            _context.PanierItems.Add(panierItem);
            await _context.SaveChangesAsync();

            panierItem = await _context.PanierItems
                .Include(p => p.Produit)
                .Include(p => p.User)
                .FirstAsync(p => p.Id == panierItem.Id);

            return new PanierDto
            {
                Id = panierItem.Id,
                ProduitId = panierItem.ProduitId,
                ProduitNom = panierItem.Produit?.Nom ?? "Inconnu",
                UserId = panierItem.UserId,
                Username = panierItem.User?.Username ?? "Inconnu", // ? corrigé
                Quantite = panierItem.Quantite
            };
        }

        public async Task RetirerDuPanierAsync(int panierItemId, Guid userId)
        {
            var item = await _context.PanierItems
                .FirstOrDefaultAsync(p => p.Id == panierItemId && p.UserId == userId);

            if (item == null)
                throw new Exception("Panier introuvable ou non autorisé.");

            _context.PanierItems.Remove(item);
            await _context.SaveChangesAsync();
        }



        public async Task ViderPanierAsync(Guid userId)
        {
            var items = _context.PanierItems.Where(p => p.UserId == userId);
            _context.PanierItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public Task RetirerDuPanierAsync(int panierItemId)
        {
            throw new NotImplementedException();
        }
    }
}
