using InventaireMcService.Data;
using InventaireMcService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace InventaireMcService.Test.UnitTests.Services
{
    public class StockServiceTests
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
        public async Task GetAllStocksAsync_RetourneTousLesStocks()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateContext(dbName);
            var logger = NullLogger<StockService>.Instance;
            var service = new StockService(context, logger);

            // Act
            var stocks = await service.GetAllStocksAsync();

            // Assert
            Assert.Equal(context.StockItems.Count(), stocks.Count());
        }

        [Fact]
        public async Task GetStockByMagasinProduitAsync_Existant_RetourneDto()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateContext(dbName);
            var logger = NullLogger<StockService>.Instance;
            var service = new StockService(context, logger);

            // Act
            var dto = await service.GetStockByMagasinProduitAsync(1, 3);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(400, dto.Quantite);
        }

        [Fact]
        public async Task GetStockByMagasinProduitAsync_NonExistant_RetourneNull()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateContext(dbName);
            var logger = NullLogger<StockService>.Instance;
            var service = new StockService(context, logger);

            // Act
            var dto = await service.GetStockByMagasinProduitAsync(10, 10);

            // Assert
            Assert.Null(dto);
        }

        [Fact]
        public async Task UpdateStockAsync_Existant_RetourneTrueEtAugmenteQuantite()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateContext(dbName);
            var logger = NullLogger<StockService>.Instance;
            var service = new StockService(context, logger);
            int magasinId = 1, produitId = 2, quantiteAAjouter = 50;

            // Act
            var result = await service.UpdateStockAsync(magasinId, produitId, quantiteAAjouter);

            // Assert
            Assert.True(result);
            var si = await context.StockItems.FindAsync(magasinId, produitId);
            Assert.Equal(400 + quantiteAAjouter, si.Quantite);
        }

        [Fact]
        public async Task UpdateStockAsync_NonExistant_RetourneFalse()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateContext(dbName);
            var logger = NullLogger<StockService>.Instance;
            var service = new StockService(context, logger);

            // Act
            var result = await service.UpdateStockAsync(99, 99, 10);

            // Assert
            Assert.False(result);
        }
    }
}
