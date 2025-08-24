using GestionProduit.Application.DTOs;

public interface ICommandeService
{
    Task<CommandeDto> CreerDepuisPanierAsync(Guid userId, string username, CommandeCreateDto? input);
    Task<List<CommandeDto>> GetMesCommandesAsync(Guid userId);
    Task<CommandeDto?> GetByIdAsync(int id, Guid userId, bool admin = false);
    Task<bool> AnnulerAsync(int id, Guid userId, bool admin = false);
    Task<bool> ChangerStatutAsync(int id, string nouveauStatut);
    Task<List<CommandeDto>> GetAllCommandesAsync(string? statut = null);
}
