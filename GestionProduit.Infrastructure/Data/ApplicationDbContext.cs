using Microsoft.EntityFrameworkCore;
using GestionProduit.Domain.Entities;

namespace GestionProduit.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Produits
        public DbSet<Produit> Produits => Set<Produit>();

        // Utilisateurs
        public DbSet<User> Users => Set<User>();

        // Panier
        public DbSet<PanierItem> PanierItems => Set<PanierItem>();

        // Commandes
        public DbSet<Commande> Commandes => Set<Commande>();

        // Éléments de commande (si tu les gères séparément)
        public DbSet<CommandeItem> CommandeItems => Set<CommandeItem>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relation Commande -> User
            modelBuilder.Entity<Commande>()
                .HasOne(c => c.User)
                .WithMany(u => u.Commandes) // <- pas de 'static' ici
                .HasForeignKey(c => c.UserId);

            // Relation CommandeItem -> Commande
            modelBuilder.Entity<CommandeItem>()
                .HasOne(ci => ci.Commande)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CommandeId);

            // Relation CommandeItem -> Produit
            modelBuilder.Entity<CommandeItem>()
                .HasOne<Produit>()       // Produit est la cible, pas de lambda
                .WithMany()
                .HasForeignKey(ci => ci.ProduitId);
        }

    }
}
