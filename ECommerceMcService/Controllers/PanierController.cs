using ECommerceMcService.Models;
using ECommerceMcService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMcService.Controllers
{
    /// <summary>
    /// Contrôleur pour gérer les opérations liées aux paniers des clients.
    /// </summary>
    [ApiController]
    [Route("api/v1/panier")]
    public class PanierController : ControllerBase
    {
        private readonly IPanierService _service;
        private readonly ILogger<PanierController> _logger;

        public PanierController(ILogger<PanierController> logger, IPanierService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// Retourne le panier d'un client par son ID.
        /// </summary>
        [HttpGet("{clientId}")]
        public async Task<ActionResult<PanierDto>> Get(int clientId)
        {
            _logger.LogInformation("Requête GET /panier/{ClientId} : récupération du panier du client {ClientId}.", clientId, clientId);

            try
            {
                var panier = await _service.GetPanierParClient(clientId);
                if (panier == null)
                {
                    _logger.LogWarning("Panier non trouvé pour le client ID {ClientId}.", clientId);
                    return NotFound();
                }

                _logger.LogInformation("Panier récupéré avec succès pour le client ID {ClientId}.", clientId);
                return Ok(panier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du panier du client ID {ClientId}.", clientId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Ajoute ou met à jour un produit dans le panier d'un client.
        /// </summary>
        [HttpPost("{clientId}/ajouter")]
        public async Task<IActionResult> Ajouter(int clientId, [FromBody] LignePanierDto dto)
        {
            _logger.LogInformation("Requête POST /panier/{ClientId}/ajouter : ajout/mise à jour du produit ID {ProduitId}, quantité {Quantite}.",
                clientId, dto.ProduitId, dto.Quantite);

            try
            {
                await _service.AjouterOuMettreAJourProduit(clientId, dto.ProduitId, dto.Quantite);
                _logger.LogInformation("Produit ID {ProduitId} ajouté/mis à jour avec succès dans le panier du client ID {ClientId}.",
                    dto.ProduitId, clientId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l’ajout/mise à jour du produit ID {ProduitId} dans le panier du client ID {ClientId}.",
                    dto.ProduitId, clientId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprime un produit spécifique du panier d'un client.
        /// </summary>
        [HttpDelete("{clientId}/supprimer/{produitId}")]
        public async Task<IActionResult> Supprimer(int clientId, int produitId)
        {
            _logger.LogInformation("Requête DELETE /panier/{ClientId}/supprimer/{ProduitId} : suppression du produit ID {ProduitId} du panier du client ID {ClientId}.",
                clientId, produitId, produitId, clientId);

            try
            {
                await _service.SupprimerProduit(clientId, produitId);
                _logger.LogInformation("Produit ID {ProduitId} supprimé du panier du client ID {ClientId}.", produitId, clientId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du produit ID {ProduitId} du panier du client ID {ClientId}.", produitId, clientId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Vide le panier d’un client.
        /// </summary>
        [HttpDelete("{clientId}/vider")]
        public async Task<IActionResult> Vider(int clientId)
        {
            _logger.LogInformation("Requête DELETE /panier/{ClientId}/vider : vidage du panier du client ID {ClientId}.", clientId, clientId);

            try
            {
                await _service.ViderPanier(clientId);
                _logger.LogInformation("Panier du client ID {ClientId} vidé avec succès.", clientId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du vidage du panier du client ID {ClientId}.", clientId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Valider un panier pour créer une commande.
        /// </summary>
        /// <param name="panierId">L'identifiant du panier a valider</param>
        /// <returns></returns>
        [HttpPost("{panierId}/valider")]
        public async Task<IActionResult> ValiderCommande(int panierId)
        {
            _logger.LogInformation("Validation de commande pour le panier {panierId} en cours...", panierId);

            var success = await _service.ValiderCommandeAsync(panierId);
            if (!success)
            {
                _logger.LogWarning("Échec de la validation de la commande pour le panier {panierId}", panierId);
                return BadRequest("Commande invalide (stock insuffisant ou données incorrectes).");
            }

            _logger.LogInformation("Commande validée avec succès pour le panier {panierId}", panierId);
            return Ok("Commande validée avec succès.");
        }
    }
}
