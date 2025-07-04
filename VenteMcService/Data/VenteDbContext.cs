using Microsoft.EntityFrameworkCore;
using VenteMcService.Models;

namespace VenteMcService.Data
{
    public class VenteDbContext : DbContext
    {
        public VenteDbContext(DbContextOptions<VenteDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vente> Ventes { get; set; } = null!;
        public DbSet<LigneVente> LignesVente { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration relation Vente - LigneVente
            modelBuilder.Entity<LigneVente>()
                .HasOne<Vente>()
                .WithMany(v => v.Lignes)
                .HasForeignKey(l => l.VenteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed des ventes
            modelBuilder.Entity<Vente>().HasData(
                new Vente { VenteId = 1, MagasinId = 2, ClientId = null, IsEnLigne = false, Date = DateTime.UtcNow.AddDays(-2) },
                new Vente { VenteId = 2, MagasinId = 3, ClientId = null, IsEnLigne = false, Date = DateTime.UtcNow.AddDays(-1) },
                new Vente { VenteId = 3, MagasinId = null, ClientId = 2, IsEnLigne = true, Date = DateTime.UtcNow.AddDays(-3) },
                new Vente { VenteId = 4, MagasinId = 1, ClientId = null, IsEnLigne = false, Date = DateTime.UtcNow.AddDays(-5) }
            );

            // Seed des lignes de vente
            modelBuilder.Entity<LigneVente>().HasData(
                new LigneVente { LigneVenteId = 1, VenteId = 1, ProduitId = 1, Quantite = 2, PrixUnitaire = 1.50m },
                new LigneVente { LigneVenteId = 2, VenteId = 1, ProduitId = 2, Quantite = 1, PrixUnitaire = 3.75m },
                new LigneVente { LigneVenteId = 3, VenteId = 2, ProduitId = 3, Quantite = 5, PrixUnitaire = 12.00m }
            );
        }
    }
}
