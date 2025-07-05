using ECommerceMcService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMcService.Data
{
    public class ECommerceDbContext : DbContext
    {
        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Panier> Paniers { get; set; } = null!;
        public DbSet<LignePanier> LignesPanier { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Client>().HasData(
                new Client { ClientId = 1, Nom = "Alice Dupont", Courriel = "alice@dupont.ca", Adresse = "" },
                new Client { ClientId = 2, Nom = "Alex Alexandre", Courriel = "alex@alexandre.ca", Adresse = "" },
                new Client { ClientId = 3, Nom = "Chris Christopher", Courriel = "chris@christopher.ca", Adresse = "" },
                new Client { ClientId = 4, Nom = "Simon Samuel", Courriel = "simon@samuel.ca", Adresse = "" }
            );

            // Relation : un panier a plusieurs lignes
            modelBuilder.Entity<LignePanier>()
                .HasOne<Panier>()
                .WithMany(p => p.Lignes)
                .HasForeignKey(lp => lp.PanierId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed optionnel pour les tests
            modelBuilder.Entity<Panier>().HasData(
                new Panier { PanierId = 1, ClientId = 2 }
            );

            modelBuilder.Entity<LignePanier>().HasData(
                new LignePanier { LignePanierId = 1, PanierId = 1, ProduitId = 1, Quantite = 2 },
                new LignePanier { LignePanierId = 2, PanierId = 1, ProduitId = 2, Quantite = 1 }
            );
        }
    }
}
