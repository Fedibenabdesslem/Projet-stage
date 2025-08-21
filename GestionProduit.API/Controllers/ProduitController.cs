using GestionProduit.Infrastructure.Interfaces;
using GestionProduit.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionProduit.API.Controllers;

[ApiController]
[Authorize(Roles = "admin,user")]
[Route("api/[controller]")]
public class ProduitController : ControllerBase
{
    private readonly IProduitService _produitService;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ProduitController> _logger;

    public ProduitController(IProduitService produitService, IWebHostEnvironment env, ILogger<ProduitController> logger)
    {
        _produitService = produitService;
        _env = env;
        _logger = logger;
    }

    // GET: api/produit
    [HttpGet]
    public async Task<ActionResult<List<Produit>>> GetAll()
    {
        return Ok(await _produitService.GetAllProduitsAsync());
    }

    // GET: api/produit/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Produit>> GetById(int id)
    {
        var produit = await _produitService.GetProduitByIdAsync(id);
        if (produit == null) return NotFound();
        return Ok(produit);
    }

    // POST: api/produit
    [HttpPost]
    public async Task<ActionResult> Create([FromForm] Produit produit)
    {
        try
        {
            if (Request.Form.Files.Any())
            {
                produit.ImageUrl = await SaveImageAsync(Request.Form.Files.First());
            }

            await _produitService.AjouterProduitAsync(produit);
            return CreatedAtAction(nameof(GetById), new { id = produit.Id }, produit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du produit");
            return StatusCode(500, ex.Message);
        }
    }

    
    // PUT: api/produit/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromForm] Produit produit)
    {
        if (id != produit.Id)
            return BadRequest("L'ID du produit ne correspond pas.");

        try
        {
            // Récupérer l'entité suivie par EF via le service
            var existingProduit = await _produitService.GetProduitByIdAsync(id);
            if (existingProduit == null)
                return NotFound();

            // Gérer l'image si uploadé
            if (Request.Form.Files.Any())
            {
                existingProduit.ImageUrl = await SaveImageAsync(Request.Form.Files.First());
            }

            // Mettre à jour les autres propriétés
            existingProduit.Nom = produit.Nom;
            existingProduit.Description = produit.Description;
            existingProduit.Prix = produit.Prix;
            existingProduit.Stock = produit.Stock;
            // ajouter d'autres propriétés si nécessaire

            // Appeler le service pour sauvegarder les modifications
            await _produitService.ModifierProduitAsync(existingProduit);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erreur lors de la modification du produit avec ID {id}");
            return StatusCode(500, ex.Message);
        }
    }


    // DELETE: api/produit/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _produitService.SupprimerProduitAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erreur lors de la suppression du produit avec ID {id}");
            return StatusCode(500, ex.Message);
        }
    }

    // POST: api/produit/upload-image/{id}
    [HttpPost("upload-image/{id}")]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("Fichier invalide.");

        try
        {
            var produit = await _produitService.GetProduitByIdAsync(id);
            if (produit == null) return NotFound();

            produit.ImageUrl = await SaveImageAsync(file);
            await _produitService.ModifierProduitAsync(produit);

            return Ok(produit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erreur lors de l'upload de l'image pour le produit {id}");
            return StatusCode(500, ex.Message);
        }
    }

    // Méthode utilitaire pour sauvegarder l'image
    private async Task<string> SaveImageAsync(IFormFile file)
    {
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var path = Path.Combine(_env.WebRootPath, "images", fileName);

        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/images/{fileName}";
    }
}
