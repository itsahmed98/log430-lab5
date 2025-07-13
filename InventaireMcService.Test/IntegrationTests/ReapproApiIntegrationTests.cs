using System.Collections.Generic;
using System.Net;
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
    public class ReapproApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ReapproApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<InventaireDbContext>>();
                    services.RemoveAll<InventaireDbContext>();
                    services.AddDbContext<InventaireDbContext>(opts =>
                        opts.UseInMemoryDatabase("ReapproTestDb"));
                }));
            _client = _factory.CreateClient();
        }

        private async Task SeedCentralStockAsync(int produitId, int quantite)
        {
            using var scope = _factory.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<InventaireDbContext>();
            ctx.StockItems.RemoveRange(ctx.StockItems);
            await ctx.SaveChangesAsync();
            ctx.StockItems.Add(new StockItem { MagasinId = 1, ProduitId = produitId, Quantite = quantite });
            await ctx.SaveChangesAsync();
        }

        private async Task<int> CreateDemandeAsync(int magasinId, int produitId, int quantite)
        {
            var dto = new CreerDemandeDto
            {
                MagasinId = magasinId,
                ProduitId = produitId,
                QuantiteDemandee = quantite
            };
            var resp = await _client.PostAsJsonAsync("/api/v1/inventaire/reapprovisionnement", dto);
            resp.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await resp.Content.ReadFromJsonAsync<DemandeReapprovisionnement>();
            created.Should().NotBeNull();
            created!.Statut.Should().Be("EN_ATTENTE");
            return created.DemandeId;
        }

        [Fact]
        public async Task CreerDemande_ShouldReturnCreated_WhenDtoValid()
        {
            // Act
            var dto = new CreerDemandeDto { MagasinId = 2, ProduitId = 10, QuantiteDemandee = 5 };
            var resp = await _client.PostAsJsonAsync("/api/v1/inventaire/reapprovisionnement", dto);

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await resp.Content.ReadFromJsonAsync<DemandeReapprovisionnement>();
            created!.MagasinId.Should().Be(2);
            created.ProduitId.Should().Be(10);
            created.QuantiteDemandee.Should().Be(5);
            created.Statut.Should().Be("EN_ATTENTE");
        }

        [Fact]
        public async Task GetEnAttente_ShouldReturnDemandes_WhenTheyExist()
        {
            // Arrange
            var id = await CreateDemandeAsync(3, 20, 7);

            // Act
            var resp = await _client.GetAsync("/api/v1/inventaire/reapprovisionnement/en-attente");

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await resp.Content.ReadFromJsonAsync<List<DemandeReapprovisionnement>>();
            list.Should().ContainSingle(d => d.DemandeId == id && d.Statut == "EN_ATTENTE");
        }

        [Fact]
        public async Task ValiderDemande_ShouldReturnNoContent_WhenStockSufficient()
        {
            // Arrange
            await SeedCentralStockAsync(produitId: 30, quantite: 50);
            var id = await CreateDemandeAsync(magasinId: 4, produitId: 30, quantite: 15);

            // Act
            var resp = await _client.PutAsync($"/api/v1/inventaire/reapprovisionnement/4/valider", null);

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // And the demande should no longer appear in en-attente
            var get = await _client.GetAsync("/api/v1/inventaire/reapprovisionnement/en-attente");
            var list = await get.Content.ReadFromJsonAsync<List<DemandeReapprovisionnement>>();
            list.Should().NotContain(d => d.DemandeId == id);
        }

        [Fact]
        public async Task ValiderDemande_ShouldReturnBadRequest_WhenStockInsufficient()
        {
            // Arrange
            await SeedCentralStockAsync(produitId: 40, quantite: 3);
            var id = await CreateDemandeAsync(magasinId: 5, produitId: 40, quantite: 10);

            // Act
            var resp = await _client.PutAsync($"/api/v1/inventaire/reapprovisionnement/{id}/valider", null);

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
