using CatalogueMcService.Data;
using CatalogueMcService.Models;
using CatalogueMcService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace CatalogueMcService.Test.UnitTests.Services
{
    public class CatalogueServiceTests
    {
        private readonly CatalogueService _service;
        private readonly CatalogueDbContext _context;
        private readonly IMemoryCache _cache;

        public CatalogueServiceTests()
        {
            var options = new DbContextOptionsBuilder<CatalogueDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new CatalogueDbContext(options);
            _cache = new MemoryCache(new MemoryCacheOptions());

            var loggerMock = new Mock<ILogger<CatalogueService>>();
            _service = new CatalogueService(loggerMock.Object, _context, _cache);

            // Seed in-memory DB
            _context.Produits.Add(new Produit
            {
                ProduitId = 1,
                Nom = "Test",
                Categorie = "Cat",
                Prix = 9.99m,
                Description = "Desc"
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllProduitsAsync_ShouldReturnProduits()
        {
            var produits = await _service.GetAllProduitsAsync();
            Assert.Single(produits);
        }

        [Fact]
        public async Task GetProduitByIdAsync_ShouldReturnCorrectProduit()
        {
            var produit = await _service.GetProduitByIdAsync(1);
            Assert.NotNull(produit);
            Assert.Equal("Test", produit?.Nom);
        }

        [Fact]
        public async Task RechercherProduitsAsync_ShouldFindByName()
        {
            var produits = await _service.RechercherProduitsAsync("test");
            Assert.Single(produits);
        }

        [Fact]
        public async Task ModifierProduitAsync_ShouldUpdateProduit()
        {
            var dto = new ProduitDto
            {
                Nom = "Updated",
                Categorie = "Updated",
                Prix = 10.5m,
                Description = "New desc"
            };

            await _service.ModifierProduitAsync(1, dto);
            var updated = await _service.GetProduitByIdAsync(1);

            Assert.Equal("Updated", updated?.Nom);
        }
    }
}
