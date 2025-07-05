using MagasinCentral.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagasinCentral.Controllers
{
    /// <summary>
    /// Contrôleur pour les performances du tableau de bord.
    /// </summary>
    public class PerformancesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PerformancesController> _logger;

        public PerformancesController(ILogger<PerformancesController> logger, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory?.CreateClient("PerformancesMcService") ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Affiche le tableau de bord des performances.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            IActionResult? result = null;

            _logger.LogInformation("Tentative de récupération des performances...");

            try
            {
                var performances = await _httpClient.GetFromJsonAsync<List<PerformanceDto>>("");

                if (performances == null || !performances.Any())
                {
                    _logger.LogError("Échec de la récupération des performances");
                    result = View("Error");
                }

                _logger.LogInformation("Performaces ont été récupérées avec succès.");
                result = View(performances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors de la récupération des performances.");
                result = View("Error");
            }

            return result;
        }
    }
}