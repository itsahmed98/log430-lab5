using System.Net;
using System.Text;
using System.Text.Json;
using AdministrationMcService.Models;
using AdministrationMcService.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdministrationMcService.Test.UnitTests.Services
{
    public class RapportServiceTests
    {
        // Un HttpMessageHandler factice qui renvoie ce qu'on veut
        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
            public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
                => _responder = responder;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => Task.FromResult(_responder(request));
        }

        private HttpClient CreateClientReturning<T>(string baseAddress, T payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
            var client = new HttpClient(handler) { BaseAddress = new Uri(baseAddress) };
            return client;
        }

        [Fact]
        public async Task ObtenirRapportConsolideAsync_ShouldReturnRapportDto_WhenMicroservicesReturnValidData()
        {
            // Arrange
            var ventes = new List<VenteDto>
            {
                new VenteDto
                {
                    MagasinId = 1,
                    Lignes = new List<LigneVenteDto>
                    {
                        new LigneVenteDto { ProduitId = 10, PrixUnitaire = 2m, Quantite = 3 }
                    }
                }
            };
            var produits = new List<ProduitDto>
            {
                new ProduitDto { ProduitId = 10, Nom = "P10" }
            };
            var stocks = new List<StockDto>
            {
                new StockDto { MagasinId = 1, ProduitId = 10, Quantite = 5 }
            };

            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(f => f.CreateClient("VenteMcService"))
                       .Returns(CreateClientReturning("http://vente/", ventes));
            factoryMock.Setup(f => f.CreateClient("CatalogueMcService"))
                       .Returns(CreateClientReturning("http://cat/", produits));
            factoryMock.Setup(f => f.CreateClient("InventaireMcService"))
                       .Returns(CreateClientReturning("http://inv/", stocks));

            var logger = Mock.Of<ILogger<RapportService>>();
            var svc = new RapportService(logger, factoryMock.Object);

            // Act
            var result = await svc.ObtenirRapportConsolideAsync();

            // Assert
            result.VentesParMagasin.Single().MagasinId.Should().Be(1);
            result.VentesParMagasin.Single().TotalVentes.Should().Be(2m * 3);
            result.ProduitsLesPlusVendus.Single().ProduitId.Should().Be(10);
            result.StocksRestants.Single().Quantite.Should().Be(5);
        }

        [Fact]
        public async Task ObtenirRapportConsolideAsync_ShouldReturnRapportDtoWithEmptyLists_WhenMicroservicesReturnNull()
        {
            // Arrange : chaque appel renvoie null
            var emptyHandler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null", Encoding.UTF8, "application/json")
            });
            var client = new HttpClient(emptyHandler) { BaseAddress = new Uri("http://any/") };

            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(f => f.CreateClient("VenteMcService")).Returns(client);
            factoryMock.Setup(f => f.CreateClient("CatalogueMcService")).Returns(client);
            factoryMock.Setup(f => f.CreateClient("InventaireMcService")).Returns(client);

            var logger = Mock.Of<ILogger<RapportService>>();
            var svc = new RapportService(logger, factoryMock.Object);

            // Act
            var result = await svc.ObtenirRapportConsolideAsync();

            // Assert
            result.VentesParMagasin.Should().BeEmpty();
            result.ProduitsLesPlusVendus.Should().BeEmpty();
            result.StocksRestants.Should().BeEmpty();
        }

        [Fact]
        public async Task ObtenirRapportConsolideAsync_ShouldThrowApplicationException_WhenHttpRequestExceptionOccursAsync()
        {
            // Arrange : on simule une HttpRequestException
            var throwingHandler = new FakeHttpMessageHandler(_ => throw new HttpRequestException());
            var client = new HttpClient(throwingHandler);

            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

            var logger = Mock.Of<ILogger<RapportService>>();
            var svc = new RapportService(logger, factoryMock.Object);

            // Act & Assert
            Func<Task> act = () => svc.ObtenirRapportConsolideAsync();
            await act.Should().ThrowAsync<ApplicationException>();
        }
    }
}
