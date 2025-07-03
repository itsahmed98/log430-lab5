using CatalogueMcService.Controllers;
using CatalogueMcService.Models;
using CatalogueMcService.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace CatalogueMcService.Test.IntegrationTests.Controllers
{

    public class ProduitControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ProduitControllerTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenHttpClientFactoryIsNull()
        {
            var ILoggerMock = new Mock<ILogger<CatalogueController>>();
            var IProduitServiceMock = new Mock<ICatalogueService>();

            Assert.Throws<ArgumentNullException>(() => new CatalogueController(null!, IProduitServiceMock.Object));
            Assert.Throws<ArgumentNullException>(() => new CatalogueController(ILoggerMock.Object, null!));
        }

        [Fact]
        public async Task GetProduits_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/api/v1/produits");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetProduit_ValidId_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/api/v1/produits/1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetProduit_InvalidId_ShouldReturnNotFound()
        {
            var response = await _client.GetAsync("/api/v1/produits/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ModifierProduit_ValidId_ShouldReturnOk()
        {
            var produitDto = new ProduitDto
            {
                Nom = "Produit modifié",
                Categorie = "Autre",
                Prix = 5.99m,
                Description = "Nouveau"
            };

            var response = await _client.PutAsJsonAsync("/api/v1/produits/1", produitDto);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ModifierProduit_InvalidId_ShouldReturnNotFound()
        {
            var produitDto = new ProduitDto
            {
                Nom = "Inexistant",
                Categorie = "Test",
                Prix = 1.0m,
                Description = "Desc"
            };

            var response = await _client.PutAsJsonAsync("/api/v1/produits/999", produitDto);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task RechercherProduits_TermFound_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/api/v1/produits/recherche?terme=stylo");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}