namespace GestionProduit.Application.DTOs
{
    public class ProduitDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Prix { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; } // image facultative
    }

    public class PanierDto
    {
        public int Id { get; set; }
        public int ProduitId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Quantite { get; set; }

        // ?? Objet produit complet pour le frontend
        public ProduitDto Produit { get; set; } = new ProduitDto();
    }
}
