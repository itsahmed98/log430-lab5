using AdministrationMcService.Data;
using AdministrationMcService.Models;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Json;

namespace AdministrationMcService.Test.UnitTests
{
    public class MagasinApiIntegrationTests :
        IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public MagasinApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<AdminDbContext>>();
                    services.RemoveAll<AdminDbContext>();

                    services.AddDbContext<AdminDbContext>(opts =>
                        opts.UseInMemoryDatabase("TestDb"));
                });
            });
        }

        private async Task SeedDatabaseAsync(params (int Id, string Nom)[] magasins)
        {
            using var scope = _factory.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AdminDbContext>();

            // remove any existing data
            ctx.Magasins.RemoveRange(ctx.Magasins);
            await ctx.SaveChangesAsync();

            // add each magasin
            foreach (var (id, nom) in magasins)
            {
                ctx.Magasins.Add(new Magasin { MagasinId = id, Nom = nom });
            }
            await ctx.SaveChangesAsync();
        }

        [Fact]
        public async Task GetMagasins_ShouldReturnAllMagasins()
        {
            await SeedDatabaseAsync((1, "Int1"));

            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/api/v1/administration/magasins");
            resp.StatusCode.Should().Be(HttpStatusCode.OK);

            var list = await resp.Content.ReadFromJsonAsync<List<Magasin>>();
            list.Should().ContainSingle(m => m.MagasinId == 1);
        }

        [Fact]
        public async Task GetMagasinById_ShouldReturnMagasin_WhenExists()
        {
            await SeedDatabaseAsync((2, "Int2"));

            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/api/v1/administration/magasins/2");
            resp.StatusCode.Should().Be(HttpStatusCode.OK);

            var mag = await resp.Content.ReadFromJsonAsync<Magasin>();
            mag!.MagasinId.Should().Be(2);
        }

        [Fact]
        public async Task GET_api_v1_administration_magasins_99_Retourne_NotFound()
        {
            await SeedDatabaseAsync((2, "Int2"));

            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/api/v1/administration/magasins/99");
            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
