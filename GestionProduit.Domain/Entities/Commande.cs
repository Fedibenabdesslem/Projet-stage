using System.ComponentModel.DataAnnotations;

namespace GestionProduit.Domain.Entities
{
    public class Commande
    {
        public int Id { get; set; }

        // Utilisateur (r�f�rence � l�utilisateur qui passe la commande)
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;

        // M�tadonn�es
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
        public string Telephone { get; set; } = string.Empty;  // ? Ajout num�ro de t�l�phone

        // Total calcul� au moment de la cr�ation
        public decimal Total { get; set; }

        // Relation 1..N avec CommandeItem
        public ICollection<CommandeItem> Items { get; set; } = new List<CommandeItem>();

        public User? User { get; set; }
    }
}
