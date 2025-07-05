using MagasinCentral.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace MagasinCentral.Controllers
{
    /// <summary>
    /// Contrôleur pour les performances du tableau de bord (UC3).
    /// </summary>
    public class ProduitController : Controller
    {
        private readonly ILogger<ProduitController> _logger;
        private readonly HttpClient _httpClient;

        public ProduitController(ILogger<ProduitController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClientFactory?.CreateClient("ProduitMcService") ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <summary>
        /// Afficher la liste des produits disponibles.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Requête de récupération de la liste des produits envoyée.");
            var produits = await _httpClient.GetFromJsonAsync<List<ProduitDto>>("");
            
            if (produits == null || !produits.Any())
            {
                _logger.LogWarning("Aucun produit trouvé dans la base de données.");
                return View(new List<ProduitDto>());
            }

            _logger.LogInformation("{Count} produits récupérés avec succès.", produits?.Count ?? 0);
            return View(produits);
        }

        /// <summary>
        /// Modifier un produit existant.
        /// </summary>
        /// <param name="produitId"></param>
        public async Task<IActionResult> Modifier(int produitId)
        {
            _logger.LogInformation("Requête pour modifier le produit ID : {ProduitId}", produitId);
            var produit = await _httpClient.GetFromJsonAsync<ProduitDto>($"{_httpClient.BaseAddress}/{produitId}");

            if (produit == null)
            {
                _logger.LogWarning("Produit ID {ProduitId} non trouvé.", produitId);
                return NotFound($"Produit avec l'ID {produitId} non trouvé.");
            }

            _logger.LogInformation("Produit ID {ProduitId} récupéré pour modification.", produitId);
            return View(produit);
        }

        /// <summary>
        /// Modifier un produit existant avec les données du formulaire.
        /// </summary>
        /// <param name="produit"></param>
        [HttpPost]
        public async Task<IActionResult> Modifier(ProduitDto produit)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Échec de validation du formulaire pour le produit ID : {ProduitId}", produit.ProduitId);
                return View(produit);
            }

            _logger.LogInformation("Envoi des données modifiées pour le produit ID : {ProduitId}", produit.ProduitId);
            var json = JsonSerializer.Serialize(produit);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/{produit.ProduitId}", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Échec de la mise à jour du produit ID : {ProduitId}. Code HTTP : {StatusCode}", produit.ProduitId, response.StatusCode);
                return View("Error");
            }

            _logger.LogInformation("Produit ID : {ProduitId} mis à jour avec succès.", produit.ProduitId);
            TempData["Succès"] = $"Le produit « {produit.Nom} » a bien été mis à jour.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Recherche de produits par identifiant, nom ou catégorie.
        /// </summary>
        /// <param name="produit"></param>
        /// <returns></returns>
        public async Task<IActionResult> Recherche(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                _logger.LogWarning("Terme de recherche vide. Affichage de la page sans résultats.");
                ViewData["Terme"] = "";
                return View(new List<ProduitDto>());
            }

            _logger.LogInformation("Recherche de produits avec le terme : {Terme}", term);
            ViewData["Terme"] = term;

            var produits = await _httpClient.GetFromJsonAsync<List<ProduitDto>>($"produits/recherche?terme={term}");

            if (produits == null || !produits.Any())
            {
                _logger.LogWarning("Aucun produit trouvé pour le terme : {Terme}", term);
                return View(new List<ProduitDto>());
            }

            _logger.LogInformation("{Count} résultats trouvés pour le terme : {Terme}", produits.Count, term);
            return View(produits);
        }
    }
}
