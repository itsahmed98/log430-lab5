using AdministrationMcService.Models;
using AdministrationMcService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdministrationMcService.Controllers
{
    /// <summary>
    /// Un contrôleur pour gérer les rapports consolidés des ventes.
    /// </summary>
    [ApiController]
    [Route("api/v1/rapports")]
    public class RapportController : ControllerBase
    {
        private readonly IRapportService _rapportService;
        private readonly ILogger<RapportController> _logger;

        public RapportController(ILogger<RapportController> logger, IRapportService rapportService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _rapportService = rapportService ?? throw new ArgumentNullException(nameof(rapportService));
        }

        /// <summary>
        /// Retrouner un rapport consolidé des ventes de tous les magasins.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<RapportVentesDto>> GetRapport()
        {
            _logger.LogInformation("Début de la récupération du rapport consolidé...");
            var rapports = await _rapportService.ObtenirRapportConsolideAsync();
            if (rapports == null)
            {
                _logger.LogWarning("Aucun rapport trouvé.");
                return NotFound("Aucun rapport disponible.");
            }
            _logger.LogInformation("Rapport consolidé récupéré avec succès");
            return Ok(rapports);
        }
    }
}
