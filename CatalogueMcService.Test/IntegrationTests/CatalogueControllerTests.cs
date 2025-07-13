using CatalogueMcService.Data;
using CatalogueMcService.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace CatalogueMcService.Test.IntegrationTests
{
    public class CatalogueApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CatalogueApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<CatalogueDbContext>>();
                    services.RemoveAll<CatalogueDbContext>();
                    services.AddDbContext<CatalogueDbContext>(opts =>
                        opts.UseInMemoryDatabase("CatTestDb"));
                });
            });
        }

        private async Task SeedAsync(params Produit[] items)
        {
            using var scope = _factory.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<CatalogueDbContext>();
            ctx.Produits.RemoveRange(ctx.Produits);
            await ctx.SaveChangesAsync();
            ctx.Produits.AddRange(items);
            await ctx.SaveChangesAsync();
        }

        [Fact]
        public async Task GetProduits_ShouldReturnAllProduits_WhenSeeded()
        {
            // Arrange
            await SeedAsync(new Produit { ProduitId = 1, Nom = "X" });

            var client = _factory.CreateClient();

            // Act
            var resp = await client.GetAsync("/api/v1/Catalogue/produits");

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await resp.Content.ReadFromJsonAsync<List<Produit>>();
            list.Should().ContainSingle(p => p.ProduitId == 1);
        }

        [Fact]
        public async Task GetProduit_ShouldReturnProduit_WhenExists()
        {
            // Arrange
            await SeedAsync(new Produit { ProduitId = 2, Nom = "Y" });

            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/api/v1/Catalogue/produits/2");

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var p = await resp.Content.ReadFromJsonAsync<Produit>();
            p!.ProduitId.Should().Be(2);
        }

        [Fact]
        public async Task GetProduit_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            await SeedAsync(new Produit { ProduitId = 3, Nom = "Z" });

            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/api/v1/Catalogue/produits/99");

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Rechercher_ShouldReturnMatches_WhenSeeded()
        {
            // Arrange
            await SeedAsync(
                new Produit { ProduitId = 4, Nom = "ABC", Categorie = "Cat" },
                new Produit { ProduitId = 5, Nom = "DEF", Categorie = "Cat" }
            );

            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/api/v1/Catalogue/produits/recherche?terme=ab");

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await resp.Content.ReadFromJsonAsync<List<Produit>>();
            list.Should().ContainSingle(p => p.ProduitId == 4);
        }
    }
}
