using InventaireMcService.Data;
using InventaireMcService.Models;
using InventaireMcService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace InventaireMcService.Test.UnitTests.Services
{
    public class ReapprovisionnementServiceTests
    {
        private InventaireDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<InventaireDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            var context = new InventaireDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task CreerDemandeAsync_CreeDemandeAvecStatutEnAttente()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateContext(dbName);
            var loggerInv = NullLogger<InventaireService>.Instance;
            var inventaireService = new InventaireService(context, loggerInv);
            var loggerReap = NullLogger<ReapprovisionnementService>.Instance;
            var service = new ReapprovisionnementService(loggerReap, context, inventaireService);
            int magasinId = 3, produitId = 2, quantite = 50;

            // Act
            var demande = await service.CreerDemandeAsync(magasinId, produitId, quantite);

            // Assert
            Assert.NotNull(demande);
            Assert.Equal(magasinId, demande.MagasinId);
            Assert.Equal(produitId, demande.ProduitId);
            Assert.Equal(quantite, demande.QuantiteDemandee);
            Assert.Equal("EN_ATTENTE", demande.Statut);
        }

        [Fact]
        public async Task GetDemandesEnAttenteAsync_RetourneUniquementEnAttente()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateContext(dbName);
            context.Demandes.Add(new DemandeReapprovisionnement { MagasinId = 1, ProduitId = 1, QuantiteDemandee = 10, Statut = "VALIDÉE" });
            await context.SaveChangesAsync();
            var loggerInv = NullLogger<InventaireService>.Instance;
            var inventaireService = new InventaireService(context, loggerInv);
            var loggerReap = NullLogger<ReapprovisionnementService>.Instance;
            var service = new ReapprovisionnementService(loggerReap, context, inventaireService);

            // Act
            var demandes = await service.GetDemandesEnAttenteAsync();

            // Assert
            Assert.Empty(demandes);
        }

        [Fact]
        public async Task ValiderDemandeAsync_Succes_ValideStatutEtTransfert()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateContext(dbName);
            var loggerInv = NullLogger<InventaireService>.Instance;
            var inventaireService = new InventaireService(context, loggerInv);
            // Créer une demande avec quantite <= stock central (400)
            context.Demandes.Add(new DemandeReapprovisionnement { DemandeId = 1, MagasinId = 2, ProduitId = 1, QuantiteDemandee = 100, Statut = "EN_ATTENTE" });
            await context.SaveChangesAsync();
            var loggerReap = NullLogger<ReapprovisionnementService>.Instance;
            var service = new ReapprovisionnementService(loggerReap, context, inventaireService);

            // Act
            var result = await service.ValiderDemandeAsync(1);

            // Assert
            Assert.True(result);
            var demande = await context.Demandes.FindAsync(1);
            Assert.Equal("VALIDÉE", demande.Statut);
            var central = await context.StockItems.FindAsync(1, 1);
            var local = await context.StockItems.FindAsync(2, 1);
            Assert.Equal(400 - 100, central.Quantite);
            Assert.Equal(100 + 100, local.Quantite);
        }

        [Fact]
        public async Task ValiderDemandeAsync_EchecStockInsuffisant_RetourneFalseEtStatutNonChange()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateContext(dbName);
            var loggerInv = NullLogger<InventaireService>.Instance;
            var inventaireService = new InventaireService(context, loggerInv);
            context.Demandes.Add(new DemandeReapprovisionnement { DemandeId = 2, MagasinId = 2, ProduitId = 1, QuantiteDemandee = 500, Statut = "EN_ATTENTE" });
            await context.SaveChangesAsync();
            var loggerReap = NullLogger<ReapprovisionnementService>.Instance;
            var service = new ReapprovisionnementService(loggerReap, context, inventaireService);

            // Act
            var result = await service.ValiderDemandeAsync(2);

            // Assert
            Assert.False(result);
            var demande = await context.Demandes.FindAsync(2);
            Assert.Equal("EN_ATTENTE", demande.Statut);
        }
    }
}
