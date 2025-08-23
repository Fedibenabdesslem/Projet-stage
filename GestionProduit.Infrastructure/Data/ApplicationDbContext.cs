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

        // CommandeItems
        public DbSet<CommandeItem> CommandeItems => Set<CommandeItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relation Commande -> User
            modelBuilder.Entity<Commande>()
                .HasOne(c => c.User)
                .WithMany(u => u.Commandes)
                .HasForeignKey(c => c.UserId);

            // Relation CommandeItem -> Commande
            modelBuilder.Entity<CommandeItem>()
                .HasOne(ci => ci.Commande)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CommandeId);

            // Relation CommandeItem -> Produit
            modelBuilder.Entity<CommandeItem>()
                .HasOne<Produit>()
                .WithMany()
                .HasForeignKey(ci => ci.ProduitId);
        }
    }
}
