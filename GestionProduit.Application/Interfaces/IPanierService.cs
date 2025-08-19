using GestionProduit.Application.DTOs;

namespace GestionProduit.Application.Interfaces
{
    public interface IPanierService
    {
        Task<List<PanierDto>> GetPanierByUserAsync(Guid userId);
        Task<PanierDto> AjouterAuPanierAsync(Guid userId, int produitId, int quantite);
        Task RetirerDuPanierAsync(int panierItemId, Guid userId);
        Task ViderPanierAsync(Guid userId);
    }
}
