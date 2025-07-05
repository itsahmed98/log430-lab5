using MagasinCentral.Models;
using MagasinCentral.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace MagasinCentral.Controllers
{
    /// <summary>
    /// Controller pour gérer les opérations liées au stock central et aux demandes de réapprovisionnement.
    /// </summary>
    public class StockController : Controller
    {
        private readonly ILogger<StockController> _logger;
        private readonly HttpClient _httpStock;
        private readonly HttpClient _httpMagasin;
        private readonly HttpClient _httpProduit;

        public StockController(ILogger<StockController> logger, IHttpClientFactory httpStock, IHttpClientFactory httpMagasin, IHttpClientFactory httpProduit)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpStock = httpStock.CreateClient("StockMcService") ?? throw new ArgumentNullException(nameof(httpStock));
            _httpMagasin = httpMagasin.CreateClient("MagasinMcService") ?? throw new ArgumentNullException(nameof(httpMagasin));
            _httpProduit = httpProduit.CreateClient("ProduitMcService") ?? throw new ArgumentNullException(nameof(httpProduit));
        }

        public async Task<IActionResult> StockCentral()
        {
            var magasins = await _httpMagasin.GetFromJsonAsync<List<MagasinDto>>("");
            var stockCentral = await _httpStock.GetFromJsonAsync<List<StockDto>>($"{_httpStock.BaseAddress}api/v1/stocks/stockcentral");
            var produits = await _httpProduit.GetFromJsonAsync<List<ProduitDto>>("");

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
                var stockLocal = await _httpStock.GetFromJsonAsync<List<StockDto>>($"{_httpStock.BaseAddress}api/v1/stocks/stockmagasin/{magasin.MagasinId}");

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
            var response = await _httpStock.PostAsJsonAsync($"{_httpStock.BaseAddress}api/v1/reapprovisionnement", payload);

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
            var demandes = await _httpStock.GetFromJsonAsync<List<DemandeReapprovisionnementDto>>($"{_httpStock.BaseAddress}api/v1/reapprovisionnement/en-attente");
            return View(demandes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValiderDemande(int demandeId)
        {
            var response = await _httpStock.PostAsync($"{_httpStock.BaseAddress}api/v1/reapprovisionnement/{demandeId}/valider", null);

            if (response.IsSuccessStatusCode)
                TempData["Succès"] = "Demande validée avec succès.";
            else
                TempData["Erreur"] = "Échec de la validation de la demande.";

            return RedirectToAction("DemandesEnAttente");
        }
    }
}
