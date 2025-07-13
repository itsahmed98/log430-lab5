using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using InventaireMcService.Data;
using InventaireMcService.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace InventaireMcService.Test.IntegrationTests
{
    public class InventoryApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public InventoryApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    // Remplacer le DbContext réel par InMemory
                    services.RemoveAll<DbContextOptions<InventaireDbContext>>();
                    services.RemoveAll<InventaireDbContext>();
                    services.AddDbContext<InventaireDbContext>(opts =>
                        opts.UseInMemoryDatabase("InvTestDb"));
                }));
        }

        private async Task SeedAsync(params StockItem[] items)
        {
            using var scope = _factory.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<InventaireDbContext>();
            ctx.StockItems.RemoveRange(ctx.StockItems);
            await ctx.SaveChangesAsync();
            ctx.StockItems.AddRange(items);
            await ctx.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllStocks_WhenSeeded()
        {
            // Arrange
            await SeedAsync(new StockItem { MagasinId = 1, ProduitId = 100, Quantite = 10 });

            var client = _factory.CreateClient();

            // Act
            var resp = await client.GetAsync("/api/v1/inventaire/stocks");

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await resp.Content.ReadFromJsonAsync<List<StockDto>>();
            list.Should().ContainSingle(s =>
                s.MagasinId == 1 && s.ProduitId == 100 && s.Quantite == 10);
        }

        [Fact]
        public async Task GetStockCentral_ShouldReturnOnlyCentral_WhenMultipleSeeded()
        {
            // Arrange
            await SeedAsync(
                new StockItem { MagasinId = 1, ProduitId = 100, Quantite = 10 },
                new StockItem { MagasinId = 2, ProduitId = 200, Quantite = 5 }
            );

            var client = _factory.CreateClient();

            // Act
            var resp = await client.GetAsync("/api/v1/inventaire/stocks/stockcentral");

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await resp.Content.ReadFromJsonAsync<List<StockDto>>();
            list.Should().ContainSingle(s => s.MagasinId == 1 && s.ProduitId == 100);
        }

        [Fact]
        public async Task GetStockMagasin_ShouldReturnStocksForGivenMagasin_WhenSeeded()
        {
            // Arrange
            await SeedAsync(
                new StockItem { MagasinId = 3, ProduitId = 300, Quantite = 7 },
                new StockItem { MagasinId = 4, ProduitId = 400, Quantite = 8 }
            );

            var client = _factory.CreateClient();

            // Act
            var resp = await client.GetAsync("/api/v1/inventaire/stocks/stockmagasin/4");

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await resp.Content.ReadFromJsonAsync<List<StockDto>>();
            list.Should().ContainSingle(s => s.MagasinId == 4 && s.ProduitId == 400);
        }

        [Fact]
        public async Task GetOne_ShouldReturnStockDto_WhenExists()
        {
            // Arrange
            await SeedAsync(new StockItem { MagasinId = 5, ProduitId = 500, Quantite = 12 });

            var client = _factory.CreateClient();

            // Act
            var resp = await client.GetAsync("/api/v1/inventaire/stocks/5/500");

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var dto = await resp.Content.ReadFromJsonAsync<StockDto>();
            dto!.Quantite.Should().Be(12);
        }

        [Fact]
        public async Task GetOne_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            await SeedAsync(new StockItem { MagasinId = 6, ProduitId = 600, Quantite = 9 });

            var client = _factory.CreateClient();

            // Act
            var resp = await client.GetAsync("/api/v1/inventaire/stocks/7/700");

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateStock_ShouldReturnNoContentAndUpdateQuantity_WhenExists()
        {
            // Arrange
            await SeedAsync(new StockItem { MagasinId = 8, ProduitId = 800, Quantite = 4 });

            var client = _factory.CreateClient();
            var request = new HttpRequestMessage(
                HttpMethod.Put,
                "/api/v1/inventaire/stocks?magasinId=8&produitId=800&quantite=6"
            );

            // Act
            var respPut = await client.SendAsync(request);

            // Assert PUT
            respPut.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Act GET after update
            var respGet = await client.GetAsync("/api/v1/inventaire/stocks/8/800");
            respGet.StatusCode.Should().Be(HttpStatusCode.OK);
            var dto = await respGet.Content.ReadFromJsonAsync<StockDto>();
            dto!.Quantite.Should().Be(10); // 4 + 6
        }

        [Fact]
        public async Task UpdateStock_ShouldReturnNoContentAndNotUpdate_WhenNotExists()
        {
            // Arrange
            await SeedAsync(new StockItem { MagasinId = 9, ProduitId = 900, Quantite = 3 });

            var client = _factory.CreateClient();
            var request = new HttpRequestMessage(
                HttpMethod.Put,
                "/api/v1/inventaire/stocks?magasinId=99&produitId=999&quantite=5"
            );

            // Act
            var respPut = await client.SendAsync(request);

            // Assert PUT (controller returns 204 même si pas trouvé)
            respPut.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Ensure original remains unchanged
            var respGet = await client.GetAsync("/api/v1/inventaire/stocks/9/900");
            var dto = await respGet.Content.ReadFromJsonAsync<StockDto>();
            dto!.Quantite.Should().Be(3);
        }
    }
}
