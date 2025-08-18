using GestionProduit.Application.Interfaces;
using GestionProduit.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<PanierItem>>> GetPanier(Guid userId)
        {
            return Ok(await _panierService.GetPanierByUserAsync(userId));
        }

        [HttpPost("ajouter")]
        public async Task<ActionResult<PanierItem>> AjouterAuPanier(Guid userId, int produitId, int quantite)
        {
            var item = await _panierService.AjouterAuPanierAsync(userId, produitId, quantite);
            return Ok(item);
        }

        [HttpDelete("{panierItemId}")]
        public async Task<ActionResult> RetirerDuPanier(int panierItemId)
        {
            await _panierService.RetirerDuPanierAsync(panierItemId);
            return NoContent();
        }

        [HttpDelete("vider/{userId}")]
        public async Task<ActionResult> ViderPanier(Guid userId)
        {
            await _panierService.ViderPanierAsync(userId);
            return NoContent();
        }
    }
}
