namespace GestionProduit.Application.DTOs
{
    public class CommandeCreateDto
    {
        public string AdresseLivraison { get; set; } = string.Empty;
        public string NumeroTelephone { get; set; } = string.Empty; // ? Ajout du numéro de téléphone
        public string ModePaiement { get; set; } = "CashOnDelivery";
    }

    public class CommandeItemDto
    {
        public int ProduitId { get; set; }
        public string ProduitNom { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal PrixUnitaire { get; set; }
        public int Quantite { get; set; }
        public decimal SousTotal { get; set; }
    }

    public class CommandeDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;

        public DateTime DateCreation { get; set; }
        public string Statut { get; set; } = "EnAttente";

        public string ModePaiement { get; set; } = "CashOnDelivery";
        public string AdresseLivraison { get; set; } = string.Empty;
        public string NumeroTelephone { get; set; } = string.Empty; // ? Ajout du numéro de téléphone

        public decimal Total { get; set; }
        public List<CommandeItemDto> Items { get; set; } = new();
    }
}
