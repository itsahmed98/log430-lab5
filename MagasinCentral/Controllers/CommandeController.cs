using MagasinCentral.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagasinCentral.Controllers
{
    /// <summary>
    /// Contrôleur pour valider les commandes clients à partir du panier.
    /// </summary>
    public class CommandeController : Controller
    {
        private readonly HttpClient _httpPanier;
        private readonly HttpClient _httpCommande;
        private readonly ILogger<CommandeController> _logger;

        public CommandeController(IHttpClientFactory factory, ILogger<CommandeController> logger)
        {
            _httpPanier = factory.CreateClient("PanierMcService");
            _httpCommande = factory.CreateClient("CommandeMcService");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Valide une commande à partir du panier du client.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ValiderCommande(int clientId)
        {
            _logger.LogInformation("Début de la validation de la commande pour le client ID {ClientId}.", clientId);

            try
            {
                // Récupération du panier
                var temp = $"{_httpPanier.BaseAddress}/{clientId}";
                var panier = await _httpPanier.GetFromJsonAsync<PanierDto>($"{_httpPanier.BaseAddress}/{clientId}");

                if (panier == null || panier.Lignes == null || !panier.Lignes.Any())
                {
                    _logger.LogWarning("Le panier du client ID {ClientId} est vide ou introuvable.", clientId);
                    TempData["Error"] = "Le panier est vide ou introuvable.";
                    return RedirectToAction("Index", "Panier", new { clientId });
                }

                // Préparer la commande
                var dto = new CommandeValidationDto
                {
                    ClientId = clientId,
                    Lignes = panier.Lignes
                        .Select(p => new LigneCommandeDto
                        {
                            ProduitId = p.ProduitId,
                            Quantite = p.Quantite
                        }).ToList()
                };

                // Envoyer la commande
                var response = await _httpCommande.PostAsJsonAsync("", dto);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Commande validée avec succès pour le client ID {ClientId}.", clientId);
                    TempData["Message"] = "Commande validée avec succès.";
                }
                else
                {
                    var erreur = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Échec de la validation de commande pour le client ID {ClientId}. Code: {Code}, Détails: {Erreur}",
                        clientId, response.StatusCode, erreur);
                    TempData["Error"] = "Erreur lors de la validation de la commande.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de la validation de la commande pour le client ID {ClientId}.", clientId);
                TempData["Error"] = "Erreur interne lors de la validation de la commande.";
            }

            return RedirectToAction("Index", "Panier", new { clientId });
        }
    }
}
