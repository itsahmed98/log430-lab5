using CatalogueMcService.Data;
using CatalogueMcService.Models;
using CatalogueMcService.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace CatalogueMcService.Test.UnitTests.Services
{
    public class CatalogueServiceTests
    {
        private CatalogueDbContext CreateContext(IEnumerable<Produit> seed = null)
        {
            var opts = new DbContextOptionsBuilder<CatalogueDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var ctx = new CatalogueDbContext(opts);
            if (seed != null)
            {
                ctx.Produits.AddRange(seed);
                ctx.SaveChanges();
            }
            return ctx;
        }

        private IMemoryCache CreateCache() => new MemoryCache(new MemoryCacheOptions());

        [Fact]
        public async Task GetAllProduitsAsync_ShouldReturnAllProduits_WhenDbNotEmpty()
        {
            // Arrange
            var produits = new[]
            {
                new Produit { ProduitId = 1, Nom = "A" },
                new Produit { ProduitId = 2, Nom = "B" },
            };
            var ctx = CreateContext(produits);
            var svc = new CatalogueService(
                Mock.Of<ILogger<CatalogueService>>(),
                ctx,
                CreateCache());

            // Act
            var result = await svc.GetAllProduitsAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetProduitByIdAsync_ShouldReturnProduit_WhenExistsAndCacheEmpty()
        {
            // Arrange
            var p = new Produit { ProduitId = 5, Nom = "P5" };
            var ctx = CreateContext(new[] { p });
            var cache = CreateCache();
            var svc = new CatalogueService(
                Mock.Of<ILogger<CatalogueService>>(),
                ctx,
                cache);

            // Act
            var result = await svc.GetProduitByIdAsync(5);

            // Assert
            result.Should().NotBeNull().And.BeEquivalentTo(p);
            // le cache doit maintenant contenir la clé "produit_5"
            cache.TryGetValue("produit_5", out Produit cached).Should().BeTrue();
            cached.ProduitId.Should().Be(5);
        }

        [Fact]
        public async Task GetProduitByIdAsync_ShouldReturnProduitFromCache_WhenCalledTwice()
        {
            // Arrange
            var p = new Produit { ProduitId = 7, Nom = "P7" };
            var ctx = CreateContext(new[] { p });
            var cache = CreateCache();
            var svc = new CatalogueService(
                Mock.Of<ILogger<CatalogueService>>(),
                ctx,
                cache);

            // Prime cache
            await svc.GetProduitByIdAsync(7);
            // remove from DB
            ctx.Produits.RemoveRange(ctx.Produits);
            await ctx.SaveChangesAsync();

            // Act
            var result = await svc.GetProduitByIdAsync(7);

            // Assert
            result.Should().NotBeNull().And.BeEquivalentTo(p);
        }

        [Fact]
        public async Task GetProduitByIdAsync_ShouldThrowArgumentException_WhenIdInvalid()
        {
            // Arrange
            var ctx = CreateContext();
            var svc = new CatalogueService(
                Mock.Of<ILogger<CatalogueService>>(),
                ctx,
                CreateCache());

            // Act / Assert
            await Assert.ThrowsAsync<ArgumentException>(() => svc.GetProduitByIdAsync(0));
        }

        [Fact]
        public async Task GetProduitByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var ctx = CreateContext();
            var svc = new CatalogueService(
                Mock.Of<ILogger<CatalogueService>>(),
                ctx,
                CreateCache());

            // Act
            var result = await svc.GetProduitByIdAsync(99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ModifierProduitAsync_ShouldUpdateProduit_WhenExists()
        {
            // Arrange
            var initial = new Produit { ProduitId = 2, Nom = "Old", Categorie = "C", Prix = 1m, Description = "D" };
            var ctx = CreateContext(new[] { initial });
            var cache = CreateCache();
            var svc = new CatalogueService(
                Mock.Of<ILogger<CatalogueService>>(),
                ctx,
                cache);

            var dto = new ProduitDto
            {
                Nom = "New",
                Categorie = "NC",
                Prix = 2m,
                Description = "ND"
            };

            // Act
            await svc.ModifierProduitAsync(2, dto);

            // Assert
            var updated = await ctx.Produits.FindAsync(2);
            updated!.Nom.Should().Be("New");
            cache.TryGetValue("produit_2", out _).Should().BeFalse("le cache doit être invalidé");
        }

        [Fact]
        public async Task ModifierProduitAsync_ShouldThrowArgumentNullException_WhenDtoIsNull()
        {
            // Arrange
            var ctx = CreateContext();
            var svc = new CatalogueService(
                Mock.Of<ILogger<CatalogueService>>(),
                ctx,
                CreateCache());

            // Act / Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => svc.ModifierProduitAsync(1, null!));
        }

        [Fact]
        public async Task ModifierProduitAsync_ShouldThrowKeyNotFoundException_WhenNotExists()
        {
            // Arrange
            var ctx = CreateContext();
            var svc = new CatalogueService(
                Mock.Of<ILogger<CatalogueService>>(),
                ctx,
                CreateCache());

            var dto = new ProduitDto { Nom = "X", Categorie = "Y", Prix = 0m, Description = "" };

            // Act / Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => svc.ModifierProduitAsync(3, dto));
        }

        [Fact]
        public async Task RechercherProduitsAsync_ShouldReturnMatches_WhenTermMatches()
        {
            // Arrange
            var list = new[]
            {
                new Produit { ProduitId = 1, Nom = "Stylo", Categorie = "Papeterie" },
                new Produit { ProduitId = 2, Nom = "Crayon", Categorie = "Papeterie" },
                new Produit { ProduitId = 3, Nom = "Gomme", Categorie = "Bureau" }
            };
            var ctx = CreateContext(list);
            var svc = new CatalogueService(
                Mock.Of<ILogger<CatalogueService>>(),
                ctx,
                CreateCache());

            // Act
            var result = await svc.RechercherProduitsAsync("sty");

            // Assert
            result.Should().ContainSingle(p => p.ProduitId == 1);
        }

        [Fact]
        public async Task RechercherProduitsAsync_ShouldReturnEmpty_WhenNoMatches()
        {
            // Arrange
            var ctx = CreateContext(new[]
            {
                new Produit { ProduitId = 1, Nom = "A", Categorie = "X" }
            });
            var svc = new CatalogueService(
                Mock.Of<ILogger<CatalogueService>>(),
                ctx,
                CreateCache());

            // Act
            var result = await svc.RechercherProduitsAsync("zz");

            // Assert
            result.Should().BeEmpty();
        }
    }
}
