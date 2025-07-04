using InventaireMcService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventaireMcService.Data
{
    public class InventaireDbContext : DbContext
    {
        public InventaireDbContext(DbContextOptions<InventaireDbContext> options)
            : base(options)
        {
        }

        public DbSet<StockItem> StockItems { get; set; } = null!;
        public DbSet<DemandeReapprovisionnement> Demandes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StockItem>()
                .HasKey(si => new { si.MagasinId, si.ProduitId });

            modelBuilder.Entity<StockItem>().HasData(
                new StockItem { MagasinId = 1, ProduitId = 1, Quantite = 400 },
                new StockItem { MagasinId = 1, ProduitId = 2, Quantite = 400 },
                new StockItem { MagasinId = 1, ProduitId = 3, Quantite = 400 },
                new StockItem { MagasinId = 2, ProduitId = 1, Quantite = 100 },
                new StockItem { MagasinId = 2, ProduitId = 2, Quantite = 80 },
                new StockItem { MagasinId = 3, ProduitId = 1, Quantite = 60 },
                new StockItem { MagasinId = 3, ProduitId = 3, Quantite = 90 },
                new StockItem { MagasinId = 5, ProduitId = 2, Quantite = 100 },
                new StockItem { MagasinId = 4, ProduitId = 2, Quantite = 200 },
                new StockItem { MagasinId = 5, ProduitId = 1, Quantite = 310 }
            );
        }
    }
}
