using GestionProduit.Application.DTOs;

namespace GestionProduit.Application.Interfaces
{
    public interface ICommandeService
    {
        // Cr�ation commande depuis le panier
        Task<CommandeDto> CreerDepuisPanierAsync(Guid userId, string username, CommandeCreateDto input);

        // Commandes d�un utilisateur
        Task<List<CommandeDto>> GetMesCommandesAsync(Guid userId);

        // D�tails d�une commande (avec option admin pour bypasser le filtrage par user)
        Task<CommandeDto?> GetByIdAsync(int id, Guid userId, bool admin = false);

        // Annulation d�une commande
        Task<bool> AnnulerAsync(int id, Guid userId, bool admin = false);

        // Changer le statut d�une commande (Admin)
        Task<bool> ChangerStatutAsync(int id, string nouveauStatut);

        // --- Admin : r�cup�rer toutes les commandes ---
        Task<List<CommandeDto>> GetAllCommandesAsync(string? statut = null);
    }
}
