using AdministrationMcService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdministrationMcService.Controllers
{
    [ApiController]
    [Route("api/v1/administration/performances")]
    public class PerformanceController : ControllerBase
    {
        private readonly IPerformanceService _performanceService;
        private readonly ILogger<PerformanceController> _logger;

        public PerformanceController(ILogger<PerformanceController> logger, IPerformanceService performanceService)
        {
            _performanceService = performanceService ?? throw new ArgumentNullException(nameof(performanceService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retourner toutes les performances.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Récupération de toutes les performances.");
            var list = await _performanceService.GetAllPerformancesAsync();
            _logger.LogInformation("{Count} performances récupérées.", list.Count());
            return Ok(list);
        }
    }
}
