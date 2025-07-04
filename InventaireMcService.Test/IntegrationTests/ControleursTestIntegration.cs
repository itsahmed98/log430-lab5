using InventaireMcService.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace InventaireMcService.Test.IntegrationTests
{
    public class ControleursTestIntegration : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ControleursTestIntegration(WebApplicationFactory<Program> factory)
        {
            // Crée un client HTTP pour l'application
            _client = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {

                    });
                })
                .CreateClient();
        }

        [Fact]
        public async Task Reapprovisionnement_TestEndpoints()
        {
            // 1. Créer une demande
            var creerDto = new CreerDemandeDto { MagasinId = 2, ProduitId = 1, QuantiteDemandee = 50 };
            var createResponse = await _client.PostAsJsonAsync("/api/v1/Inventaire/Reapprovisionnement", creerDto);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var demande = await createResponse.Content.ReadFromJsonAsync<DemandeReapprovisionnement>();
            Assert.Equal("EN_ATTENTE", demande.Statut);

            // 2. Récupérer les demandes en attente
            var getResponse = await _client.GetAsync("/api/v1/Inventaire/Reapprovisionnement/en-attente");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var listes = await getResponse.Content.ReadFromJsonAsync<IEnumerable<DemandeReapprovisionnement>>();
            Assert.Contains(listes, d => d.DemandeId == demande.DemandeId);

            // 3. Valider la demande
            var putResponse = await _client.PutAsync($"/api/v1/Inventaire/Reapprovisionnement/{demande.DemandeId}/valider", null);
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // 4. Vérifier qu’elle n’apparaît plus en attente
            var getAfter = await _client.GetAsync("/api/v1/Inventaire/Reapprovisionnement/en-attente");
            var listesAfter = await getAfter.Content.ReadFromJsonAsync<IEnumerable<DemandeReapprovisionnement>>();
            Assert.DoesNotContain(listesAfter, d => d.DemandeId == demande.DemandeId);
        }

        [Fact]
        public async Task Stock_TestEndpoints()
        {
            // GET all stocks
            var allResponse = await _client.GetAsync("/api/v1/inventaire/Stock");
            Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);
            var stocks = await allResponse.Content.ReadFromJsonAsync<IEnumerable<StockDto>>();
            Assert.NotEmpty(stocks);

            // GET central stock
            var centralResponse = await _client.GetAsync("/api/v1/inventaire/Stock/stockcentral");
            Assert.Equal(HttpStatusCode.OK, centralResponse.StatusCode);
            var central = await centralResponse.Content.ReadFromJsonAsync<IEnumerable<StockDto>>();
            Assert.All(central, s => Assert.Equal(1, s.MagasinId));

            // GET stock by magasin
            var magasinResponse = await _client.GetAsync("/api/v1/inventaire/Stock/stockmagasin/2");
            Assert.Equal(HttpStatusCode.OK, magasinResponse.StatusCode);
            var magasinStocks = await magasinResponse.Content.ReadFromJsonAsync<IEnumerable<StockDto>>();
            Assert.All(magasinStocks, s => Assert.Equal(2, s.MagasinId));

            // GET one stock
            var oneResponse = await _client.GetAsync("/api/v1/inventaire/Stock/2/1");
            Assert.Equal(HttpStatusCode.OK, oneResponse.StatusCode);
            var one = await oneResponse.Content.ReadFromJsonAsync<StockDto>();
            Assert.Equal(2, one.MagasinId);
            Assert.Equal(1, one.ProduitId);

            // PUT update stock
            var updateResponse = await _client.PutAsync($"/api/v1/inventaire/Stock?magasinId=2&produitId=1&quantite=10", null);
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            // Vérifier mise à jour
            var oneAfter = await _client.GetAsync("/api/v1/inventaire/Stock/2/1");
            var updated = await oneAfter.Content.ReadFromJsonAsync<StockDto>();
            Assert.Equal(one.Quantite + 10, updated.Quantite);
        }
    }
}
