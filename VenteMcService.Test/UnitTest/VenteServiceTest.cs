using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using VenteMcService.Data;
using VenteMcService.Models;
using VenteMcService.Services;

namespace VenteMcService.Test.UnitTest
{
    public class VenteServiceTests
    {
        private VenteService CreateService(DbContextOptions<VenteDbContext> options)
        {
            var ctx = new VenteDbContext(options);
            var logger = Mock.Of<ILogger<VenteService>>();
            var httpFactory = new Mock<IHttpClientFactory>();

            var clientCatalogue = new HttpClient(new StubHttpMessageHandler<ProduitDto>(
                new ProduitDto { ProduitId = 1, Nom = "Test", Categorie = "Cat", Prix = 10m }))
            { BaseAddress = new Uri("http://localhost/api/v1/catalogue") };

            var clientInventaire = new HttpClient(new StubHttpMessageHandler<object>(
                null, HttpStatusCode.NoContent))
            { BaseAddress = new Uri("http://localhost/api/v1/inventaire") };

            httpFactory.Setup(f => f.CreateClient("CatalogueMcService")).Returns(clientCatalogue);
            httpFactory.Setup(f => f.CreateClient("InventaireMcService")).Returns(clientInventaire);

            return new VenteService(logger, ctx, httpFactory.Object);
        }

        [Fact]
        public async Task CreateAsync_Should_Add_Vente_And_UpdateStock()
        {
            var options = new DbContextOptionsBuilder<VenteDbContext>()
                .UseInMemoryDatabase("TestDb_Create")
                .Options;

            var service = CreateService(options);
            var vente = new Vente
            {
                MagasinId = 1,
                Date = DateTime.UtcNow,
                Lignes = new List<LigneVente>
                {
                    new LigneVente { ProduitId = 1, Quantite = 2 }
                }
            };

            var result = await service.CreateAsync(vente);

            Assert.NotNull(result);
            Assert.Equal(1, result.VenteId);

            using var ctx = new VenteDbContext(options);
            Assert.Equal(1, await ctx.Ventes.CountAsync());
            Assert.Equal(1, await ctx.LignesVente.CountAsync());
        }

        [Fact]
        public async Task GetAllAsync_Returns_All_Ventes()
        {
            var options = new DbContextOptionsBuilder<VenteDbContext>()
                .UseInMemoryDatabase("TestDb_GetAll")
                .Options;

            using (var ctx = new VenteDbContext(options))
            {
                ctx.Ventes.Add(new Vente { Date = DateTime.UtcNow });
                ctx.SaveChanges();
            }

            var service = CreateService(options);
            var list = await service.GetAllAsync();
            Assert.Single(list);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Vente()
        {
            var options = new DbContextOptionsBuilder<VenteDbContext>()
                .UseInMemoryDatabase("TestDb_Delete")
                .Options;

            using (var ctx = new VenteDbContext(options))
            {
                ctx.Ventes.Add(new Vente { VenteId = 1, Date = DateTime.UtcNow, Lignes = new List<LigneVente>() });
                ctx.SaveChanges();
            }

            var service = CreateService(options);
            await service.DeleteAsync(1);

            using var ctx2 = new VenteDbContext(options);
            Assert.Empty(ctx2.Ventes);
        }

        [Fact]
        public async Task GetByIdAsync_Returns_Vente_When_Exists()
        {
            var options = new DbContextOptionsBuilder<VenteDbContext>()
                .UseInMemoryDatabase("TestDb_GetById")
                .Options;

            using (var ctx = new VenteDbContext(options))
            {
                ctx.Ventes.Add(new Vente { VenteId = 1, Date = DateTime.UtcNow, Lignes = new List<LigneVente>() });
                ctx.SaveChanges();
            }

            var service = CreateService(options);
            var v = await service.GetByIdAsync(1);
            Assert.NotNull(v);
            Assert.Equal(1, v.VenteId);
        }

        [Fact]
        public async Task GetByMagasinAsync_Filters_By_MagasinId()
        {
            var options = new DbContextOptionsBuilder<VenteDbContext>()
                .UseInMemoryDatabase("TestDb_GetByMagasin")
                .Options;

            using (var ctx = new VenteDbContext(options))
            {
                ctx.Ventes.Add(new Vente { VenteId = 1, MagasinId = 2, Date = DateTime.UtcNow });
                ctx.Ventes.Add(new Vente { VenteId = 2, MagasinId = 3, Date = DateTime.UtcNow });
                ctx.SaveChanges();
            }

            var service = CreateService(options);
            var list = await service.GetByMagasinAsync(3);
            Assert.Single(list);
            Assert.Equal(3, list[0].MagasinId);
        }

        // Stub HTTP handler
        private class StubHttpMessageHandler<T> : HttpMessageHandler
        {
            private readonly T _response;
            private readonly HttpStatusCode _status;
            public StubHttpMessageHandler(T response, HttpStatusCode status = HttpStatusCode.OK)
            {
                _response = response;
                _status = status;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                var msg = new HttpResponseMessage(_status);
                if (_response != null)
                    msg.Content = JsonContent.Create(_response);
                return Task.FromResult(msg);
            }
        }
    }
}
