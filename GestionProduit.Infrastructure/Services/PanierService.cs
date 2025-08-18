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

        public async Task<List<PanierItem>> GetPanierByUserAsync(Guid userId)
        {
            return await _context.PanierItems
                .Include(p => p.Produit)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<PanierItem> AjouterAuPanierAsync(Guid userId, int produitId, int quantite)
        {
            var existingItem = await _context.PanierItems
                .FirstOrDefaultAsync(p => p.UserId == userId && p.ProduitId == produitId);

            if (existingItem != null)
            {
                existingItem.Quantite += quantite;
                _context.PanierItems.Update(existingItem);
                await _context.SaveChangesAsync();
                return existingItem;
            }

            var panierItem = new PanierItem
            {
                UserId = userId,
                ProduitId = produitId,
                Quantite = quantite
            };

            _context.PanierItems.Add(panierItem);
            await _context.SaveChangesAsync();

            return panierItem;
        }

        public async Task RetirerDuPanierAsync(int panierItemId)
        {
            var item = await _context.PanierItems.FindAsync(panierItemId);
            if (item != null)
            {
                _context.PanierItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ViderPanierAsync(Guid userId)
        {
            var items = _context.PanierItems.Where(p => p.UserId == userId);
            _context.PanierItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
