using GestionProduit.Application.DTOs;
using GestionProduit.Application.Interfaces;
using GestionProduit.Domain.Entities;
using GestionProduit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionProduit.Infrastructure.Services
{
    public class CommandeService : ICommandeService
    {
        private readonly ApplicationDbContext _context;

        public CommandeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandeDto> CreerDepuisPanierAsync(Guid userId, string username, CommandeCreateDto? input)
        {
            var panier = await _context.PanierItems
                .Include(p => p.Produit)
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (panier.Count == 0)
                throw new InvalidOperationException("Votre panier est vide.");

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal total = 0m;
                foreach (var ligne in panier)
                {
                    if (ligne.Produit == null)
                        throw new InvalidOperationException($"Produit #{ligne.ProduitId} introuvable.");
                    if (ligne.Produit.Stock < ligne.Quantite)
                        throw new InvalidOperationException($"Stock insuffisant pour {ligne.Produit.Nom}.");

                    total += ligne.Produit.Prix * ligne.Quantite;
                }

                var cmd = new Commande
                {
                    UserId = userId,
                    Username = username,
                    AdresseLivraison = input?.AdresseLivraison ?? "Adresse par défaut",
                    ModePaiement = input?.ModePaiement ?? "à la livraison",
                    Statut = "EnAttente",
                    Total = total
                };

                foreach (var ligne in panier)
                {
                    ligne.Produit!.Stock -= ligne.Quantite;
                    _context.Produits.Update(ligne.Produit);

                    cmd.Items.Add(new CommandeItem
                    {
                        ProduitId = ligne.ProduitId,
                        ProduitNom = ligne.Produit.Nom,
                        ImageUrl = ligne.Produit.ImageUrl,
                        PrixUnitaire = ligne.Produit.Prix,
                        Quantite = ligne.Quantite
                    });
                }

                _context.Commandes.Add(cmd);
                _context.PanierItems.RemoveRange(panier);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return await BuildCommandeDtoAsync(cmd.Id, admin: true);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        public async Task<List<CommandeDto>> GetMesCommandesAsync(Guid userId)
        {
            var list = await _context.Commandes
                .AsNoTracking()
                .Include(c => c.Items)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.DateCreation)
                .ToListAsync();

            return list.Select(MapToDto).ToList();
        }

        public async Task<CommandeDto?> GetByIdAsync(int id, Guid userId, bool admin = false)
        {
            var q = _context.Commandes
                .AsNoTracking()
                .Include(c => c.Items)
                .AsQueryable();

            if (!admin) q = q.Where(c => c.UserId == userId);

            var cmd = await q.FirstOrDefaultAsync(c => c.Id == id);
            return cmd is null ? null : MapToDto(cmd);
        }

        public async Task<bool> AnnulerAsync(int id, Guid userId, bool admin = false)
        {
            var q = _context.Commandes.Include(c => c.Items).AsQueryable();
            if (!admin) q = q.Where(c => c.UserId == userId);

            var cmd = await q.FirstOrDefaultAsync(c => c.Id == id);
            if (cmd == null) return false;
            if (cmd.Statut is "Expediee" or "Livree")
                throw new InvalidOperationException("Commande déjà expédiée/livrée, annulation impossible.");

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var produitIds = cmd.Items.Select(i => i.ProduitId).ToList();
                var produits = await _context.Produits
                    .Where(p => produitIds.Contains(p.Id))
                    .ToListAsync();

                foreach (var item in cmd.Items)
                {
                    var p = produits.FirstOrDefault(x => x.Id == item.ProduitId);
                    if (p != null) p.Stock += item.Quantite;
                }

                cmd.Statut = "Annulee";
                _context.Commandes.Update(cmd);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> ChangerStatutAsync(int id, string nouveauStatut)
        {
            var cmd = await _context.Commandes.FirstOrDefaultAsync(c => c.Id == id);
            if (cmd == null) return false;

            cmd.Statut = nouveauStatut;
            _context.Commandes.Update(cmd);
            await _context.SaveChangesAsync();
            return true;
        }

        // ----------------- Admin -----------------
        public async Task<List<CommandeDto>> GetAllCommandesAsync(string? statut = null)
        {
            var query = _context.Commandes
                .AsNoTracking()
                .Include(c => c.Items)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(statut))
                query = query.Where(c => c.Statut == statut);

            var list = await query
                .OrderByDescending(c => c.DateCreation)
                .ToListAsync();

            return list.Select(MapToDto).ToList();
        }

        // ----------------- Helpers -----------------
        private async Task<CommandeDto> BuildCommandeDtoAsync(int id, bool admin = false)
        {
            var cmd = await _context.Commandes
                .AsNoTracking()
                .Include(c => c.Items)
                .FirstAsync(c => c.Id == id);

            return MapToDto(cmd);
        }

        private static CommandeDto MapToDto(Commande c) => new()
        {
            Id = c.Id,
            UserId = c.UserId,
            Username = c.Username,
            DateCreation = c.DateCreation,
            Statut = c.Statut,
            ModePaiement = c.ModePaiement,
            AdresseLivraison = c.AdresseLivraison,
            Total = c.Total,
            Items = c.Items.Select(i => new CommandeItemDto
            {
                ProduitId = i.ProduitId,
                ProduitNom = i.ProduitNom,
                ImageUrl = i.ImageUrl,
                PrixUnitaire = i.PrixUnitaire,
                Quantite = i.Quantite,
                SousTotal = i.PrixUnitaire * i.Quantite
            }).ToList()
        };
    }
}
