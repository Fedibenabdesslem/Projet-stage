using GestionProduit.Application.Interfaces;
using GestionProduit.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GestionProduit.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProduitController : ControllerBase
{
    private readonly IProduitService _produitService;

    public ProduitController(IProduitService produitService)
    {
        _produitService = produitService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Produit>>> GetAll()
    {
        return Ok(await _produitService.GetAllProduitsAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Produit>> GetById(int id)
    {
        var produit = await _produitService.GetProduitByIdAsync(id);
        if (produit == null)
            return NotFound();
        return Ok(produit);
    }

    [HttpPost]
    public async Task<ActionResult> Create(Produit produit)
    {
        await _produitService.AjouterProduitAsync(produit);
        return CreatedAtAction(nameof(GetById), new { id = produit.Id }, produit);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, Produit produit)
    {
        if (id != produit.Id)
            return BadRequest();

        await _produitService.ModifierProduitAsync(produit);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _produitService.SupprimerProduitAsync(id);
        return NoContent();
    }
}
