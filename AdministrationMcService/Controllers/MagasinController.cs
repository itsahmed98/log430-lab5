using AdministrationMcService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdministrationMcService.Controllers
{
    /// <summary>
    /// Contrôleur pour gérer les opérations liées aux magasins.
    /// </summary>
    [ApiController]
    [Route("api/v1/administration/magasins")]
    public class MagasinController : ControllerBase
    {
        private readonly ILogger<MagasinController> _logger;
        private readonly IMagasinService _magasinService;

        /// <summary>
        /// Constructeur du contrôleur MagasinController.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="magasinService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MagasinController(ILogger<MagasinController> logger, IMagasinService magasinService)
        {
            _magasinService = magasinService ?? throw new ArgumentNullException(nameof(magasinService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retourner tous les magasins.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Récupération de tous les magasins.");
            var magasins = await _magasinService.GetAllAsync();
            _logger.LogInformation("{Count} magasins récupérés.", magasins.Count);
            return Ok(magasins);
        }

        /// <summary>
        /// Retourner un magasin par son ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Recherche du magasin avec ID {Id}.", id);
            var magasin = await _magasinService.GetMagasinByIdAsync(id);
            if (magasin == null)
            {
                _logger.LogWarning("Magasin avec ID {Id} non trouvé.", id);
                return NotFound(new { message = $"Magasin ID={id} non trouvé." });
            }
            _logger.LogInformation("Magasin ID {Id} trouvé : {Nom}", magasin.MagasinId, magasin.Nom);
            return Ok(magasin);
        }

    }
}
