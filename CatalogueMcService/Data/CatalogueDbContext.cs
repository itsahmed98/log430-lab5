using Microsoft.EntityFrameworkCore;
using CatalogueMcService.Models;

namespace CatalogueMcService.Data
{
    /// <summary>
    /// DbContext spécifique au microservice produit.
    /// </summary>
    public class CatalogueDbContext : DbContext
    {
        public CatalogueDbContext(DbContextOptions<CatalogueDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produit> Produits { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed initial de quelques produits
            modelBuilder.Entity<Produit>().HasData(
                new Produit { ProduitId = 1, Nom = "Stylo", Categorie = "Papeterie", Prix = 1.50m, Description = "Stylo à bille bleu" },
                new Produit { ProduitId = 2, Nom = "Carnet", Categorie = "Papeterie", Prix = 3.75m, Description = "Carnet de notes A5" },
                new Produit { ProduitId = 3, Nom = "Clé USB 16 Go", Categorie = "Électronique", Prix = 12.00m, Description = "Clé USB 16 Go" }
            );
        }
    }
}