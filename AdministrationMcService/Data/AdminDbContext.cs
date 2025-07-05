using AdministrationMcService.Models;
using Microsoft.EntityFrameworkCore;

namespace AdministrationMcService.Data
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options)
            : base(options)
        {
        }

        public DbSet<Performance> Performances { get; set; } = null!;
        public DbSet<Magasin> Magasins { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Magasin>().HasData(
                new Magasin { MagasinId = 1, Nom = "Entrepot Central", Adresse = "123 Rue entrepot" },
                new Magasin { MagasinId = 2, Nom = "Magasin A", Adresse = "123 Rue Principale" },
                new Magasin { MagasinId = 3, Nom = "Magasin B", Adresse = "456 Avenue Centrale" },
                new Magasin { MagasinId = 4, Nom = "Magasin C", Adresse = "789 Boulevard Sud" },
                new Magasin { MagasinId = 5, Nom = "Magasin D", Adresse = "321 Rue Nord" }
            );

            // Exemple de seed : 2 magasins pour les 3 derniers jours
            modelBuilder.Entity<Performance>().HasData(
                new Performance
                {
                    PerformanceId = 1,
                    MagasinId = 2,
                    Date = DateTime.UtcNow.Date.AddDays(-2),
                    ChiffreAffaires = 1000m,
                    NbVentes = 50,
                    RupturesStock = 2,
                    Surstock = 5
                },
                new Performance
                {
                    PerformanceId = 2,
                    MagasinId = 3,
                    Date = DateTime.UtcNow.Date.AddDays(-2),
                    ChiffreAffaires = 1500m,
                    NbVentes = 70,
                    RupturesStock = 1,
                    Surstock = 3
                },
                new Performance
                {
                    PerformanceId = 3,
                    MagasinId = 2,
                    Date = DateTime.UtcNow.Date.AddDays(-1),
                    ChiffreAffaires = 1100m,
                    NbVentes = 55,
                    RupturesStock = 0,
                    Surstock = 4
                },
                new Performance
                {
                    PerformanceId = 4,
                    MagasinId = 3,
                    Date = DateTime.UtcNow.Date.AddDays(-1),
                    ChiffreAffaires = 1600m,
                    NbVentes = 75,
                    RupturesStock = 2,
                    Surstock = 2
                },
                new Performance
                {
                    PerformanceId = 5,
                    MagasinId = 2,
                    Date = DateTime.UtcNow.Date,
                    ChiffreAffaires = 1200m,
                    NbVentes = 60,
                    RupturesStock = 1,
                    Surstock = 6
                },
                new Performance
                {
                    PerformanceId = 6,
                    MagasinId = 3,
                    Date = DateTime.UtcNow.Date,
                    ChiffreAffaires = 1700m,
                    NbVentes = 80,
                    RupturesStock = 0,
                    Surstock = 3
                }
            );
        }
    }
}
