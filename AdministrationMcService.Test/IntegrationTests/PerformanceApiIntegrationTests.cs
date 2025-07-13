using System.Net;
using System.Net.Http.Json;
using AdministrationMcService.Data;
using AdministrationMcService.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AdministrationMcService.Test.UnitTests
{
    public class PerformanceApiIntegrationTests :
        IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public PerformanceApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remplacer le DbContext réel par InMemory
                    services.RemoveAll<DbContextOptions<AdminDbContext>>();
                    services.RemoveAll<AdminDbContext>();
                    services.AddDbContext<AdminDbContext>(opts =>
                        opts.UseInMemoryDatabase("PerfTestDb"));
                });
            });
        }

        private async Task SeedAsync(params Performance[] items)
        {
            using var scope = _factory.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
            ctx.Performances.RemoveRange(ctx.Performances);
            await ctx.SaveChangesAsync();

            ctx.Performances.AddRange(items);
            await ctx.SaveChangesAsync();
        }

        [Fact]
        public async Task GetPerformances_ShouldReturnAll()
        {
            // Arrange
            await SeedAsync(
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
                }
            );

            var client = _factory.CreateClient();

            // Act
            var resp = await client.GetAsync("/api/v1/administration/performances");

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await resp.Content.ReadFromJsonAsync<List<Performance>>();
            list.Should().HaveCount(2);
        }
    }
}
