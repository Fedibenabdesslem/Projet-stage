using GestionProduit.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionProduit.Application.Interfaces
{
    public interface IPanierService
    {
        Task<List<PanierItem>> GetPanierByUserAsync(Guid userId);
        Task<PanierItem> AjouterAuPanierAsync(Guid userId, int produitId, int quantite);
        Task RetirerDuPanierAsync(int panierItemId);
        Task ViderPanierAsync(Guid userId);
    }
}
