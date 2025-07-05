using MagasinCentral.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace MagasinCentral.Controllers
{
    /// <summary>
    /// Un controller pour gérer les opérations liées au panier d'un client dans le magasin central.
    /// </summary>
    public class ECommerceController : Controller
    {
        private readonly HttpClient _httpEcommerce;
        private readonly HttpClient _httpCatalogue;
        private readonly ILogger<ECommerceController> _logger;

        public ECommerceController(IHttpClientFactory factory, ILogger<ECommerceController> logger)
        {
            _httpEcommerce = factory.CreateClient("ECommerceMcService") ?? throw new ArgumentNullException(nameof(factory));
            _httpCatalogue = factory.CreateClient("CatalogueMcService") ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Affiche le panier du client.
        /// </summary>
        public async Task<IActionResult> PanierClient(int clientId)
        {
            _logger.LogInformation("Requête GET /Panier : récupération du panier pour le client ID {ClientId}", clientId);

            try
            {
                var panier = await _httpEcommerce.GetFromJsonAsync<PanierDto>($"{_httpEcommerce.BaseAddress}/panier/{clientId}")
                            ?? new PanierDto { ClientId = clientId, Lignes = new List<LignePanierDto>() };

                var produits = await _httpCatalogue.GetFromJsonAsync<List<ProduitDto>>("") ?? new List<ProduitDto>();

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
        public async Task<IActionResult> AjouterProduit(int clientId, int produitId, int quantite)
        {
            _logger.LogInformation("Tentative d’ajout du produit {ProduitId} (quantité : {Quantite}) au panier du client ID {ClientId}.", produitId, quantite, clientId);

            var payload = new { ClientId = clientId, ProduitId = produitId, Quantite = quantite };

            try
            {
                var temp = $"{_httpEcommerce.BaseAddress}/{clientId}/ajouter";
                var response = await _httpEcommerce.PostAsJsonAsync($"{_httpEcommerce.BaseAddress}/panier/{clientId}/ajouter", payload);

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

            return RedirectToAction("PanierClient", new { clientId });
        }

        /// <summary>
        /// Vide le panier d’un client.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ViderPanier(int panierId)
        {
            _logger.LogInformation("Requête DELETE /Panier : tentative de vider le panier du client ID {panierId}.", panierId);

            try
            {
                var response = await _httpEcommerce.DeleteAsync($"{_httpEcommerce.BaseAddress}/panier/{panierId}/vider");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Panier vidé avec succès pour le client ID {panierId}.", panierId);
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
                _logger.LogError(ex, "Erreur lors du vidage du panier pour le client ID {ClienpanierIdtId}.", panierId);
                TempData["Error"] = "Erreur interne lors du vidage du panier.";
            }

            return RedirectToAction("PanierClient", new { clientId = 2 });
        }

        /// <summary>
        /// Affiche la liste des clients.
        /// </summary>
        public async Task<IActionResult> Clients()
        {
            _logger.LogInformation("Requête GET /Client : récupération de la liste des clients.");

            try
            {
                var clients = await _httpEcommerce.GetFromJsonAsync<List<ClientDto>>($"{_httpEcommerce.BaseAddress}/clients");

                if (clients == null)
                {
                    _logger.LogWarning("Aucun client récupéré depuis ClientMcService.");
                    return View(new List<ClientDto>());
                }

                _logger.LogInformation("Récupération de {Count} clients réussie.", clients.Count);
                return View(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des clients depuis ClientMcService.");
                return View("Error");
            }
        }

        /// <summary>
        /// Affiche le formulaire de création de client.
        /// </summary>
        [HttpGet]
        public IActionResult CreateClient()
        {
            _logger.LogInformation("Requête GET /Client/Create : affichage du formulaire de création.");
            return View();
        }

        /// <summary>
        /// Traite la soumission du formulaire de création.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateClient(ClientDto client)
        {
            _logger.LogInformation("Requête POST /Client/Create : tentative de création du client {Nom}, {Courriel}.", client.Nom, client.Courriel);

            try
            {
                var response = await _httpEcommerce.PostAsJsonAsync($"{_httpEcommerce.BaseAddress}/clients", client);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Client créé avec succès.");
                    return RedirectToAction("Clients");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Échec de la création du client. Code: {StatusCode}. Détails: {Content}", response.StatusCode, errorContent);

                ModelState.AddModelError("", "Erreur lors de la création du client.");
                return View(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l’envoi de la requête de création du client.");
                ModelState.AddModelError("", "Erreur interne. Veuillez réessayer plus tard.");
                return View(client);
            }
        }

        /// <summary>
        /// Valide une commande à partir du panier du client.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ValiderCommande(int panierId)
        {
            _logger.LogInformation("Début de la validation du panier {panierId}.", panierId);

            try
            {
                // Envoyer la commande
                var response = await _httpEcommerce.PostAsJsonAsync($"{_httpEcommerce.BaseAddress}/panier/{panierId}/valider", "");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Commande validée avec succès pour le panier {panierId}.", panierId);
                    TempData["Message"] = "Commande validée avec succès.";
                }
                else
                {
                    var erreur = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Échec de la validation de commande pour le panier {panierId}. Code: {Code}, Détails: {Erreur}",
                        panierId, response.StatusCode, erreur);
                    TempData["Error"] = "Erreur lors de la validation de la commande.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la validation du panier {panierId}.", panierId);
                TempData["Error"] = "Erreur interne lors de la validation de la commande.";
            }

            return RedirectToAction(nameof(PanierClient), new { clientId = 2 });
        }
    }
}
