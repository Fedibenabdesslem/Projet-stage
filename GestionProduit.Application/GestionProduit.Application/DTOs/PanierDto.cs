namespace GestionProduit.Application.DTOs
{
    public class PanierDto
    {
        public int Id { get; set; }
        public int ProduitId { get; set; }
        public string ProduitNom { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty; // ? ici
        public int Quantite { get; set; }
    }
}
