using Microsoft.AspNetCore.Mvc;
using MagasinCentral.Models;

namespace MagasinCentral.Controllers
{
    /// <summary>
    /// Contrôleur pour les rapports consolidés des ventes.
    /// </summary>
    public class RapportController : Controller
    {
        private readonly ILogger<RapportController> _logger;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructeur de <see cref="RapportController"/>.
        /// </summary>
        /// <param name="rapportService"></param>
        public RapportController(ILogger<RapportController> logger, IHttpClientFactory httpClientFactory)

        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClientFactory?.CreateClient("AdministrationMcService") ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <summary>
        /// Affiche la page d'accueil du rapport consolidé.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            IActionResult? result = null!;

            try
            {
                _logger.LogInformation("Tentative de récupération du rapport consolidé...");
                var temp = $"{_httpClient.BaseAddress}/rapports";
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

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
