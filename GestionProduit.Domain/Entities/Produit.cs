namespace GestionProduit.Domain.Entities;

public class Produit
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    public int Stock { get; set; }
}
