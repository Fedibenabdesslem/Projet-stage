namespace GestionProduit.Domain.Entities
{
    public class CommandeItem
    {
        public int Id { get; set; }

        // Relation avec Commande
        public int CommandeId { get; set; }
        public Commande Commande { get; set; } = default!;

        // Produit
        public int ProduitId { get; set; }
        public string ProduitNom { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        // Détails
        public decimal PrixUnitaire { get; set; }
        public int Quantite { get; set; }

        // Calcul automatique
        public decimal SousTotal => PrixUnitaire * Quantite;
    }
}
