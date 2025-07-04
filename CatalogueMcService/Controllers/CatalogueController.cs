using Microsoft.AspNetCore.Mvc;
using CatalogueMcService.Services;
using CatalogueMcService.Models;

namespace CatalogueMcService.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]/produits")]
    public class CatalogueController : ControllerBase
    {
        private readonly ICatalogueService _catalogueService;
        private readonly ILogger<CatalogueController> _logger;

        public CatalogueController(ILogger<CatalogueController> logger, ICatalogueService produitService)
        {
            _catalogueService = produitService ?? throw new ArgumentNullException(nameof(produitService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Récupère la liste de tous les produits.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProduits()
        {
            _logger.LogInformation("Récupération de tous les produits.");

            try
            {
                var produits = await _catalogueService.GetAllProduitsAsync();
                return Ok(produits);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des produits.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la récupération des produits.");
            }
        }

        /// <summary>
        /// Endpoint pour récupérer un produit par son identifiant.
        /// </summary>
        /// <param name="produitId">L'identifiant du produit à récupérer</param>
        /// <returns></returns>
        [HttpGet("{produitId:int}")]
        [ProducesResponseType(typeof(Produit), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProduit(int produitId)
        {
            _logger.LogInformation("Récupération du produit avec ID {ProduitId}", produitId);

            try
            {
                var produit = await _catalogueService.GetProduitByIdAsync(produitId);
                if (produit == null)
                {
                    _logger.LogWarning("Produit avec ID {ProduitId} non trouvé.", produitId);
                    return NotFound($"Produit avec ID {produitId} non trouvé.");
                }

                return Ok(produit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du produit avec ID {ProduitId}", produitId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la récupération du produit.");
            }
        }

        /// <summary>
        /// Endpoint pour la modification d'un produit existant.
        /// </summary>
        /// <param name="produitId">L'identifiant du produit à modifier</param>
        /// <param name="produit">Le nouveau produit</param>
        /// <returns></returns>
        [HttpPut("{produitId:int}")]
        [ProducesResponseType(typeof(ProduitDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ModifierProduit(int produitId, [FromBody] ProduitDto produit)
        {
            _logger.LogInformation("Modification du produit avec ID {ProduitId}", produitId);

            try
            {
                var produitExistant = await _catalogueService.GetProduitByIdAsync(produitId);
                if (produitExistant == null)
                {
                    _logger.LogWarning("Produit avec ID {ProduitId} non trouvé.", produitId);
                    return NotFound($"Produit avec ID {produitId} non trouvé.");
                }

                await _catalogueService.ModifierProduitAsync(produitId, produit);
                return Ok(produit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la modification du produit avec ID {ProduitId}", produitId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la modification du produit.");
            }
        }

        /// <summary>
        /// Recherche de produits par nom ou catégorie.
        /// </summary>
        /// <param name="terme">Mot-clé à rechercher.</param>
        /// <returns>Liste des produits correspondants.</returns>
        [HttpGet("recherche")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Rechercher([FromQuery] string terme)
        {
            _logger.LogInformation("Recherche de produits avec le terme : {Terme}", terme);
            var resultat = await _catalogueService.RechercherProduitsAsync(terme);
            _logger.LogInformation("produits trouvés pour le terme '{Terme}'.", terme);
            return Ok(resultat);
        }
    }
}
