using MagasinCentral.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagasinCentral.Api.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des produits.
    /// </summary>
    [ApiController]
    //[Authorize]
    [Route("api/v1/Catalogue/produits")]
    public class ProduitApiController : ControllerBase
    {
        private readonly ILogger<ProduitApiController> _logger;
        private readonly HttpClient _httpClient;

        public ProduitApiController(ILogger<ProduitApiController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClientFactory?.CreateClient("CatalogueMcService") ?? throw new ArgumentNullException(nameof(httpClientFactory));

        }

        /// <summary>
        /// Récupérer la liste de tous les produits.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Produits()
        {
            return Ok(await _httpClient.GetFromJsonAsync<List<ProduitDto>>(""));
        }


        /// <summary>
        /// Récupérer un produit par son ID.
        /// Si le produit n'existe pas, retourne 404 Not Found.
        /// </summary>
        /// <param name="produitId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{produitId:int}")]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> Produit(int produitId)
        {
            _logger.LogInformation("Récupération du produit ID={ProduitId}", produitId);
            var produit = await _httpClient.GetFromJsonAsync<ProduitDto>($"{_httpClient.BaseAddress}/{produitId}");
            return produit is not null ? Ok(produit) : NotFound();
        }

        /// <summary>
        /// Mettre à jour un produit existant.
        /// </summary>
        /// <param name="produitId">ID du produit à modifier.</param>
        /// <param name="payload">Les nouvelles données du produit.</param>
        /// 
        /// test avec:
        /// {
        ///    "produitId": 3,
        ///    "nom": "Clé USB 32 Go",
        ///    "categorie": "Électronique",
        ///    "prix": 15.00,
        ///    "description": "Clé USB 32 Go avec protection améliorée333"
        /// }
        [HttpPut("{produitId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Modifier(
            [FromRoute] int produitId,
            [FromBody] ProduitDto payload)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_httpClient.BaseAddress}/{produitId}", payload);
                if (response.IsSuccessStatusCode)
                    return NoContent();

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound(new { message = $"Produit ID {produitId} non trouvé." });

                _logger.LogError("Erreur inconnue depuis ProduitMcService (StatusCode: {StatusCode})", response.StatusCode);
                return StatusCode((int)response.StatusCode, "Erreur côté microservice Produit.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inconnue lors de la modification du produit ID={ProduitId}", produitId);
                return StatusCode(500, "Une erreur s'est produite côté serveur.");
            }
        }
    }
}
