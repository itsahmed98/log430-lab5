using Microsoft.AspNetCore.Mvc;
using MagasinCentral.Models;

namespace MagasinCentral.Controllers
{
    /// <summary>
    /// Contrôleur pour les rapports consolidés des ventes.
    /// </summary>
    public class AdministrationController : Controller
    {
        private readonly ILogger<AdministrationController> _logger;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructeur de <see cref="AdministrationController"/>.
        /// </summary>
        public AdministrationController(ILogger<AdministrationController> logger, IHttpClientFactory httpClientFactory)

        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClientFactory?.CreateClient("AdministrationMcService") ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <summary>
        /// Affiche la page d'accueil du rapport consolidé.
        /// </summary>
        public async Task<IActionResult> Rapport()
        {
            IActionResult? result = null!;

            try
            {
                _logger.LogInformation("Tentative de récupération du rapport consolidé...");
                var rapportConsolide = await _httpClient.GetFromJsonAsync<RapportVentesDto>($"{_httpClient.BaseAddress}/rapports");

                if (rapportConsolide == null)
                {
                    _logger.LogWarning("Aucun rapport consolidé trouvé.");
                    result = View("NotFound");
                }
                else
                {
                    _logger.LogInformation("Rapport consolidé récupéré avec succès.");
                    result = View(rapportConsolide);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors de la récupération du rapport consolidé.");
                ViewBag.ErrorMessage = $"Une erreur est survenue lors de la génération du rapport: {ex.Message}";
                result = View("Error");
            }

            return result;
        }

        /// <summary>
        /// Affiche le tableau de bord des performances.
        /// </summary>
        public async Task<IActionResult> Performance()
        {
            IActionResult? result = null;

            _logger.LogInformation("Tentative de récupération des performances...");

            try
            {
                var performances = await _httpClient.GetFromJsonAsync<List<PerformanceDto>>($"{_httpClient.BaseAddress}/performances");

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

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
