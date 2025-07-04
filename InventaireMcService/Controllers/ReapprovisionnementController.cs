using InventaireMcService.Models;
using InventaireMcService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventaireMcService.Controllers
{
    /// <summary>
    /// Contrôleur pour gérer les demandes de réapprovisionnement des produits dans les magasins.
    /// </summary>
    [ApiController]
    [Route("api/v1/Inventaire/[Controller]")]
    public class ReapprovisionnementController : ControllerBase
    {
        private readonly IReapprovisionnementService _service;
        private readonly ILogger<ReapprovisionnementController> _logger;

        public ReapprovisionnementController(IReapprovisionnementService service, ILogger<ReapprovisionnementController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Crée une nouvelle demande de réapprovisionnement.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DemandeReapprovisionnement>> CreerDemande([FromBody] CreerDemandeDto dto)
        {
            _logger.LogInformation("Requête de création d’une demande reçue : Magasin {MagasinId}, Produit {ProduitId}, Quantité {Quantite}",
                dto.MagasinId, dto.ProduitId, dto.QuantiteDemandee);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Données invalides reçues pour la création d’une demande.");
                return BadRequest(ModelState);
            }

            var result = await _service.CreerDemandeAsync(dto.MagasinId, dto.ProduitId, dto.QuantiteDemandee);
            return CreatedAtAction(nameof(GetEnAttente), new { id = result.DemandeId }, result);
        }

        /// <summary>
        /// Liste toutes les demandes de réapprovisionnement en attente.
        /// </summary>
        [HttpGet("en-attente")]
        public async Task<ActionResult<IEnumerable<DemandeReapprovisionnement>>> GetEnAttente()
        {
            _logger.LogInformation("Consultation des demandes en attente.");
            var demandes = await _service.GetDemandesEnAttenteAsync();
            return Ok(demandes);
        }

        /// <summary>
        /// Valide une demande de réapprovisionnement donnée.
        /// </summary>
        [HttpPut("{id:int}/valider")]
        public async Task<ActionResult> ValiderDemande(int id)
        {
            _logger.LogInformation("Tentative de validation de la demande ID {Id}", id);

            var succès = await _service.ValiderDemandeAsync(id);
            if (!succès)
            {
                _logger.LogWarning("Échec de la validation de la demande ID {Id}", id);
                return BadRequest($"La validation de la demande {id} a échoué (stock insuffisant ou demande inexistante).");
            }

            _logger.LogInformation("Demande ID {Id} validée avec succès.", id);
            return NoContent(); // 204
        }
    }
}
