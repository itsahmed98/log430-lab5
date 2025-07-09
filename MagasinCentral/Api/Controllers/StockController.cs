using MagasinCentral.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagasinCentral.Api.Controllers
{
    /// <summary>
    /// Contrôleur pour le stock des magasins (appel du microservice StockMcService).
    /// </summary>
    [ApiController]
    [Route("api/v1/inventaire")]
    public class StockController : ControllerBase
    {
        private readonly ILogger<StockController> _logger;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructeur du contrôleur.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="httpClientFactory"></param>
        public StockController(ILogger<StockController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClientFactory?.CreateClient("InventaireMcService") ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <summary>
        /// Récupérer le stock d’un magasin spécifique.
        /// </summary>
        /// <param name="magasinId">Identifiant du magasin</param>
        [HttpGet("{magasinId:int}")]
        [ProducesResponseType(typeof(List<StockDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client)]
        public async Task<ActionResult<List<StockDto>>> GetStockMagasin(int magasinId)
        {
            try
            {
                var stocks = await _httpClient.GetFromJsonAsync<List<StockDto>>($"{_httpClient.BaseAddress}/stocks/stockmagasin/{magasinId}");

                if (stocks == null || !stocks.Any())
                {
                    _logger.LogWarning("Aucun stock trouvé pour le magasin ID={MagasinId}", magasinId);
                    return NotFound();
                }

                _logger.LogInformation("Stock récupéré pour magasin ID={MagasinId}. Nombre de produits : {Count}", magasinId, stocks.Count);
                return Ok(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du stock pour le magasin ID={MagasinId}", magasinId);
                return StatusCode(500, "Une erreur s'est produite lors de la récupération du stock.");
            }
        }
    }
}