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

    public ProduitController(IProduitService produitService, IWebHostEnvironment env)
    {
        _produitService = produitService;
        _env = env;
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
        if (produit == null)
            return NotFound();
        return Ok(produit);
    }

    // POST: api/produit
    [HttpPost]
    public async Task<ActionResult> Create([FromForm] Produit produit)
    {
        // Gestion de l'image
        var file = Request.Form.Files.FirstOrDefault();
        if (file != null && file.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var path = Path.Combine(_env.WebRootPath, "images", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            produit.ImageUrl = $"/images/{fileName}";
        }

        await _produitService.AjouterProduitAsync(produit);
        return CreatedAtAction(nameof(GetById), new { id = produit.Id }, produit);
    }

    // PUT: api/produit/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromForm] Produit produit)
    {
        if (id != produit.Id)
            return BadRequest();

        // Gestion de l'image si elle est uploadée
        var file = Request.Form.Files.FirstOrDefault();
        if (file != null && file.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var path = Path.Combine(_env.WebRootPath, "images", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            produit.ImageUrl = $"/images/{fileName}";
        }

        await _produitService.ModifierProduitAsync(produit);
        return NoContent();
    }

    // DELETE: api/produit/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _produitService.SupprimerProduitAsync(id);
        return NoContent();
    }

    // POST: api/produit/upload-image/{id}
    [HttpPost("upload-image/{id}")]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Fichier invalide.");

        var produit = await _produitService.GetProduitByIdAsync(id);
        if (produit == null) return NotFound();

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var path = Path.Combine(_env.WebRootPath, "images", fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        produit.ImageUrl = $"/images/{fileName}";
        await _produitService.ModifierProduitAsync(produit);

        return Ok(produit);
    }
}
