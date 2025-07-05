using MagasinCentral.Models;
using MagasinCentral.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace MagasinCentral.Controllers
{
    /// <summary>
    /// Controller pour gérer les opérations liées au stock central et aux demandes de réapprovisionnement.
    /// </summary>
    public class InventaireController : Controller
    {
        private readonly ILogger<InventaireController> _logger;
        private readonly HttpClient _httpInventaire;
        private readonly HttpClient _httpAdmin;
        private readonly HttpClient _httpCatalogue;

        public InventaireController(ILogger<InventaireController> logger, IHttpClientFactory client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpInventaire = client.CreateClient("InventaireMcService") ?? throw new ArgumentNullException(nameof(client));
            _httpAdmin = client.CreateClient("AdministrationMcService") ?? throw new ArgumentNullException(nameof(client));
            _httpCatalogue = client.CreateClient("CatalogueMcService") ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IActionResult> StockCentral()
        {
            var magasins = await _httpAdmin.GetFromJsonAsync<List<MagasinDto>>($"{_httpAdmin.BaseAddress}/magasins");
            var temp = $"{_httpInventaire.BaseAddress}/stockcentral";
            var stockCentral = await _httpInventaire.GetFromJsonAsync<List<StockDto>>($"{_httpInventaire.BaseAddress}/stocks/stockcentral");
            var produits = await _httpCatalogue.GetFromJsonAsync<List<ProduitDto>>("");

            foreach (var stock in stockCentral)
            {
                var produit = produits.FirstOrDefault(p => p.ProduitId == stock.ProduitId);
                stock.NomProduit = produit?.Nom ?? $"Produit {stock.ProduitId}";
            }

            var viewModel = new StockParMagasinViewModel
            {
                Magasins = new List<MagasinStockViewModel>()
            };

            viewModel.Magasins.Add(new MagasinStockViewModel
            {
                NomMagasin = "Entrepôt Central",
                MagasinId = 0,
                Produits = stockCentral.Select(s => new ProduitStockViewModel
                {
                    ProduitId = s.ProduitId,
                    NomProduit = s.NomProduit ?? $"Produit {s.ProduitId}",
                    QuantiteDisponible = s.Quantite,
                    MagasinId = 0
                }).ToList()
            });

            foreach (var magasin in magasins)
            {
                var stockLocal = await _httpInventaire.GetFromJsonAsync<List<StockDto>>($"{_httpInventaire.BaseAddress}/stocks/stockmagasin/{magasin.MagasinId}");

                var produitsParMagasin = stockCentral.Select(central =>
                {
                    var local = stockLocal.FirstOrDefault(s => s.ProduitId == central.ProduitId);
                    return new ProduitStockViewModel
                    {
                        ProduitId = central.ProduitId,
                        NomProduit = central.NomProduit ?? $"Produit {central.ProduitId}",
                        QuantiteDisponible = local?.Quantite ?? 0,
                        MagasinId = magasin.MagasinId
                    };
                }).ToList();

                viewModel.Magasins.Add(new MagasinStockViewModel
                {
                    NomMagasin = magasin.Nom,
                    MagasinId = magasin.MagasinId,
                    Produits = produitsParMagasin
                });
            }

            return View(viewModel);
        }

        public IActionResult NouvelleDemande(int magasinId, int produitId)
        {
            ViewBag.MagasinId = magasinId;
            ViewBag.ProduitId = produitId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NouvelleDemande(int magasinId, int produitId, int quantite)
        {
            var payload = new
            {
                MagasinId = magasinId,
                ProduitId = produitId,
                QuantiteDemandee = quantite
            };
            var response = await _httpInventaire.PostAsJsonAsync($"{_httpInventaire.BaseAddress}/reapprovisionnement", payload);

            if (response.IsSuccessStatusCode)
            {
                TempData["Succès"] = "Demande créée avec succès.";
                return RedirectToAction("StockCentral");
            }
            else
            {
                TempData["Erreur"] = "Erreur lors de l'envoi de la demande.";
            }

            return RedirectToAction(nameof(StockCentral));
        }

        public async Task<IActionResult> DemandesEnAttente()
        {
            var demandes = await _httpInventaire.GetFromJsonAsync<List<DemandeReapprovisionnementDto>>($"{_httpInventaire.BaseAddress}/reapprovisionnement/en-attente");
            return View(demandes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValiderDemande(int demandeId)
        {
            // https://localhost:7221/api/v1/inventaire/reapprovisionnement/2/valider
            var temp = $"{_httpInventaire.BaseAddress}/reapprovisionnement/{demandeId}/valider";
            var response = await _httpInventaire.PutAsync($"{_httpInventaire.BaseAddress}/reapprovisionnement/{demandeId}/valider", null);

            if (response.IsSuccessStatusCode)
                TempData["Succès"] = "Demande validée avec succès.";
            else
                TempData["Erreur"] = "Échec de la validation de la demande.";

            return RedirectToAction("DemandesEnAttente");
        }
    }
}
