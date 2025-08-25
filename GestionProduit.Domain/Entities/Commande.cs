using System.ComponentModel.DataAnnotations;

namespace GestionProduit.Domain.Entities
{
    public class Commande
    {
        public int Id { get; set; }

        // Utilisateur (référence à l’utilisateur qui passe la commande)
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;

        // Métadonnées
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string Statut { get; set; } = "EnAttente";
        // Valeurs possibles : EnAttente, EnPreparation, Expediee, Livree, Annulee

        [MaxLength(50)]
        public string ModePaiement { get; set; } = "CashOnDelivery";
        // Ou "EnLigne"

        // Livraison
        [MaxLength(200)]
        public string AdresseLivraison { get; set; } = string.Empty;

        [MaxLength(20)]
        [Phone]
        public string Telephone { get; set; } = string.Empty;  // ? Ajout numéro de téléphone

        // Total calculé au moment de la création
        public decimal Total { get; set; }

        // Relation 1..N avec CommandeItem
        public ICollection<CommandeItem> Items { get; set; } = new List<CommandeItem>();

        public User? User { get; set; }
    }
}
