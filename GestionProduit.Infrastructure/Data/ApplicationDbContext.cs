using Microsoft.EntityFrameworkCore;
using GestionProduit.Domain.Entities;

namespace GestionProduit.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Produit> Produits => Set<Produit>();

    public DbSet<User> Users => Set<User>();

    public DbSet<PanierItem> PanierItems => Set<PanierItem>();




}
