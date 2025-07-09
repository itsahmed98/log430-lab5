using Microsoft.AspNetCore.Mvc;
using MagasinCentral.Models;

namespace MagasinCentral.Api.Controllers
{
    /// <summary>
    /// Contrôleur pour visualiser les performances des magasins.
    /// </summary>
    [ApiController]
    //[Authorize]
    [Route("api/v1/administration/performances")]
    public class PerformancesController : ControllerBase
    {
        private readonly ILogger<PerformancesController> _logger;
        private readonly HttpClient _httpClient;

        public PerformancesController(ILogger<PerformancesController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClientFactory?.CreateClient("AdministrationMcService") ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <summary>
        /// Obtenir les performances des magasins.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetPerformances()
        {
            IActionResult result = null!;

            try
            {
                result = Ok(await _httpClient.GetFromJsonAsync<List<PerformanceDto>>($"{_httpClient.BaseAddress}/performances"));
                _logger.LogInformation("Performances récupérées avec succès.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des performances.");
                result = StatusCode(500, "Une erreur s'est produite lors de la récupération des performances.");
            }

            return result;
        }
    }
}