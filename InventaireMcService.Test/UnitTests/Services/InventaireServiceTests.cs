using InventaireMcService.Data;
using InventaireMcService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace InventaireMcService.Test.UnitTests.Services
{
    public class InventaireServiceTests
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
        public async Task TransfertStock_CentralSuffisant_RetourneTrueEtMetAJourQuantites()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateContext(dbName);
            var logger = NullLogger<InventaireService>.Instance;
            var service = new InventaireService(context, logger);
            int produitId = 1;
            int magasinDestination = 2;
            int quantiteATransferer = 100;

            // Act
            var result = await service.TransférerStockAsync(produitId, magasinDestination, quantiteATransferer);

            // Assert
            Assert.True(result);
            var central = await context.StockItems.FindAsync(1, produitId);
            var local = await context.StockItems.FindAsync(magasinDestination, produitId);
            Assert.Equal(400 - quantiteATransferer, central.Quantite);
            Assert.Equal(100 + quantiteATransferer, local.Quantite);
        }

        [Fact]
        public async Task TransfertStock_CentralInsuffisant_RetourneFalseEtNeChangeRien()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateContext(dbName);
            var logger = NullLogger<InventaireService>.Instance;
            var service = new InventaireService(context, logger);
            int produitId = 1;
            int magasinDestination = 2;
            int quantiteATransferer = 500;

            // Act
            var result = await service.TransférerStockAsync(produitId, magasinDestination, quantiteATransferer);

            // Assert
            Assert.False(result);
            var central = await context.StockItems.FindAsync(1, produitId);
            Assert.Equal(400, central.Quantite);
        }
    }
}
