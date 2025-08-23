using GestionProduit.Application.DTOs;
using GestionProduit.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GestionProduit.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "user,admin")]
    [Route("api/[controller]")]
    public class CommandesController : ControllerBase
    {
        private readonly ICommandeService _commandeService;
        private readonly ILogger<CommandesController> _logger;

        public CommandesController(ICommandeService commandeService, ILogger<CommandesController> logger)
        {
            _commandeService = commandeService;
            _logger = logger;
        }

        private (Guid userId, string username) GetUser()
        {
            var idStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrWhiteSpace(idStr) || string.IsNullOrWhiteSpace(username))
                throw new Exception("Token invalide.");

            return (Guid.Parse(idStr), username);
        }


        // POST api/commandes -> crée une commande depuis le panier courant
        [HttpPost]
        public async Task<ActionResult<CommandeDto>> Creer([FromBody] CommandeCreateDto input)
        {
            try
            {
                var (userId, username) = GetUser();
                var result = await _commandeService.CreerDepuisPanierAsync(userId, username, input);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur création commande");
                return BadRequest(ex.Message);
            }
        }

        // GET api/commandes/mes -> commandes de l'utilisateur connecté
        [HttpGet("mes")]
        public async Task<ActionResult<List<CommandeDto>>> MesCommandes()
        {
            var (userId, _) = GetUser();
            var data = await _commandeService.GetMesCommandesAsync(userId);
            return Ok(data);
        }

        // GET api/commandes/5 -> détails d'une commande
        [HttpGet("{id}")]
        public async Task<ActionResult<CommandeDto>> GetById(int id)
        {
            var (userId, _) = GetUser();
            var cmd = await _commandeService.GetByIdAsync(id, userId);
            if (cmd == null) return NotFound();
            return Ok(cmd);
        }

        // POST api/commandes/5/annuler -> annuler une commande
        [HttpPost("{id}/annuler")]
        public async Task<ActionResult> Annuler(int id)
        {
            try
            {
                var (userId, _) = GetUser();
                var ok = await _commandeService.AnnulerAsync(id, userId);
                return ok ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur annulation");
                return BadRequest(ex.Message);
            }
        }

        // ------ Admin ------

        // PATCH api/commandes/5/statut?value=Expediee -> changer le statut
        [Authorize(Roles = "admin")]
        [HttpPatch("{id}/statut")]
        public async Task<ActionResult> ChangerStatut(int id, [FromQuery] string value)
        {
            var ok = await _commandeService.ChangerStatutAsync(id, value);
            return ok ? NoContent() : NotFound();
        }

        // GET api/commandes/all -> toutes les commandes (Admin)
        [Authorize(Roles = "admin")]
        [HttpGet("all")]
        public async Task<ActionResult<List<CommandeDto>>> GetAllCommandes([FromQuery] string? statut = null)
        {
            var commandes = await _commandeService.GetAllCommandesAsync(statut);
            return Ok(commandes);
        }
    }
}
