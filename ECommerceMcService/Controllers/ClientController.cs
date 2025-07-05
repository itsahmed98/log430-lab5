using ECommerceMcService.Models;
using ECommerceMcService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMcService.Controllers
{
    [ApiController]
    [Route("api/v1/ecommerce/clients")]
    public class ClientController : ControllerBase
    {
        private readonly ILogger<ClientController> _logger;
        private readonly IClientService _service;

        public ClientController(ILogger<ClientController> logger, IClientService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Récupère tous les clients.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ClientDto>>> GetAll()
        {
            _logger.LogInformation("Requête GET /clients : récupération de tous les clients.");

            try
            {
                var clients = await _service.GetAllAsync();
                _logger.LogInformation("Succès : {Count} clients récupérés.", clients.Count);
                return Ok(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de tous les clients.");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Récupère un client par ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClientDto>> GetOne(int id)
        {
            _logger.LogInformation("Requête GET /clients/{Id} : récupération du client ID {Id}.", id, id);

            try
            {
                var client = await _service.GetByIdAsync(id);
                if (client == null)
                {
                    _logger.LogWarning("Client avec ID {Id} non trouvé.", id);
                    return NotFound();
                }

                _logger.LogInformation("Client ID {Id} trouvé avec succès.", id);
                return Ok(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du client ID {Id}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Crée un nouveau client.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ClientDto>> Create([FromBody] CreateClientDto dto)
        {
            _logger.LogInformation("Requête POST /clients : création d’un nouveau client {Nom}, {Courriel}.", dto.Nom, dto.Courriel);

            try
            {
                var created = await _service.CreateAsync(dto);
                _logger.LogInformation("Client créé avec succès. ID : {ClientId}.", created.ClientId);

                return CreatedAtAction(nameof(GetOne), new { id = created.ClientId }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du client {Nom}, {Courriel}.", dto.Nom, dto.Courriel);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}
