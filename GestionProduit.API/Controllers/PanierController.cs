using GestionProduit.Application.DTOs;
using GestionProduit.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionProduit.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "user,admin")]
    [Route("api/[controller]")]
    public class PanierController : ControllerBase
    {
        private readonly IPanierService _panierService;

        public PanierController(IPanierService panierService)
        {
            _panierService = panierService;
        }

        // Récupère l'ID utilisateur depuis le JWT
        private Guid GetUserIdFromToken()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(userIdString))
                throw new Exception("Impossible de récupérer l'ID utilisateur depuis le token.");

            return Guid.Parse(userIdString);
        }

        [HttpGet("liste")]
        public async Task<ActionResult<List<PanierDto>>> GetPanier()
        {
            var userId = GetUserIdFromToken();
            var panier = await _panierService.GetPanierByUserAsync(userId);
            return Ok(panier);
        }

        [HttpPost("ajouter")]
        public async Task<ActionResult<PanierDto>> AjouterAuPanier(int produitId, int quantite)
        {
            var userId = GetUserIdFromToken();
            var item = await _panierService.AjouterAuPanierAsync(userId, produitId, quantite);
            return Ok(item);
        }

        [HttpDelete("{panierItemId}")]
        public async Task<ActionResult> RetirerDuPanier(int panierItemId)
        {
            var userId = GetUserIdFromToken();
            try
            {
                await _panierService.RetirerDuPanierAsync(panierItemId, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }



        [HttpDelete("vider")]
        public async Task<ActionResult> ViderPanier()
        {
            var userId = GetUserIdFromToken();
            await _panierService.ViderPanierAsync(userId);
            return NoContent();
        }
    }
}
