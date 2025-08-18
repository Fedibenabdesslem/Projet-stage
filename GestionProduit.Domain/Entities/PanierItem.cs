using System;
using GestionProduit.Domain.Entities;

namespace GestionProduit.Domain.Entities
{
    public class PanierItem
    {
        public int Id { get; set; }

        // Relation avec Produit
        public int ProduitId { get; set; }
        public Produit Produit { get; set; }

        // Relation avec User
        public Guid UserId { get; set; }   // <-- changé de int à Guid
        public User User { get; set; }

        public int Quantite { get; set; }
    }
}
