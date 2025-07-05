using MagasinCentral.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagasinCentral.Controllers
{
    /// <summary>
    /// Un controller pour gérer les opérations liées au panier d'un client dans le magasin central.
    /// </summary>
    public class PanierController : Controller
    {
        private readonly HttpClient _httpPanier;
        private readonly HttpClient _httpProduit;
        private readonly ILogger<PanierController> _logger;

        public PanierController(IHttpClientFactory factory, ILogger<PanierController> logger)
        {
            _httpPanier = factory.CreateClient("PanierMcService");
            _httpProduit = factory.CreateClient("ProduitMcService");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Affiche le panier du client.
        /// </summary>
        public async Task<IActionResult> Index(int clientId)
        {
            _logger.LogInformation("Requête GET /Panier : récupération du panier pour le client ID {ClientId}", clientId);

            try
            {
                var panier = await _httpPanier.GetFromJsonAsync<PanierDto>($"{_httpPanier.BaseAddress}/{clientId}")
                            ?? new PanierDto { ClientId = clientId, Lignes = new List<LignePanierDto>() };

                var produits = await _httpProduit.GetFromJsonAsync<List<ProduitDto>>("") ?? new List<ProduitDto>();

                ViewBag.Produits = produits;

                return View(panier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du panier du client ID {ClientId}.", clientId);
                TempData["Error"] = "Impossible de charger le panier.";
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Ajoute un produit dans le panier d'un client.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Ajouter(int clientId, int produitId, int quantite)
        {
            _logger.LogInformation("Tentative d’ajout du produit {ProduitId} (quantité : {Quantite}) au panier du client ID {ClientId}.", produitId, quantite, clientId);

            var payload = new { ClientId = clientId, ProduitId = produitId, Quantite = quantite };

            try
            {
                var temp = $"{_httpPanier.BaseAddress}/{clientId}/ajouter";
                var response = await _httpPanier.PostAsJsonAsync($"{_httpPanier.BaseAddress}/{clientId}/ajouter", payload);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Produit ajouté au panier du client ID {ClientId}.", clientId);
                    TempData["Message"] = "Produit ajouté au panier.";
                }
                else
                {
                    var details = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Échec de l’ajout au panier. Code: {Code}, Détails: {Details}", response.StatusCode, details);
                    TempData["Error"] = "Erreur lors de l’ajout au panier.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l’ajout du produit {ProduitId} au panier du client ID {ClientId}.", produitId, clientId);
                TempData["Error"] = "Erreur interne. Veuillez réessayer.";
            }

            return RedirectToAction("Index", new { clientId });
        }

        /// <summary>
        /// Vide le panier d’un client.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Vider(int clientId)
        {
            _logger.LogInformation("Requête DELETE /Panier : tentative de vider le panier du client ID {ClientId}.", clientId);

            try
            {
                var temp = $"{_httpPanier.BaseAddress}/{clientId}/vider";
                var response = await _httpPanier.DeleteAsync($"{_httpPanier.BaseAddress}/{clientId}/vider");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Panier vidé avec succès pour le client ID {ClientId}.", clientId);
                    TempData["Message"] = "Panier vidé.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Erreur lors du vidage du panier. Code: {Code}, Détails: {Error}", response.StatusCode, error);
                    TempData["Error"] = "Erreur lors du vidage du panier.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du vidage du panier pour le client ID {ClientId}.", clientId);
                TempData["Error"] = "Erreur interne lors du vidage du panier.";
            }

            return RedirectToAction("Index", new { clientId });
        }
    }
}
