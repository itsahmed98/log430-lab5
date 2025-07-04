using InventaireMcService.Models;
using InventaireMcService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventaireMcService.Controllers
{
    /// <summary>
    /// Contrôleur pour gérer les demandes de réapprovisionnement des produits dans les magasins.
    /// </summary>
    [ApiController]
    [Route("api/v1/inventaire/[Controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _service;
        private readonly ILogger<StockController> _logger;

        public StockController(IStockService service, ILogger<StockController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Liste tous les stocks.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetAll()
        {
            _logger.LogInformation("Récupération de tous les stocks.");
            var stocks = await _service.GetAllStocksAsync();
            _logger.LogInformation("{Count} stocks trouvés.", stocks.Count());
            return Ok(stocks);
        }

        /// <summary>
        /// Récupère le stock central (MagasinId = 1).
        /// </summary>
        [HttpGet("stockcentral")]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetStockCentral()
        {
            _logger.LogInformation("Récupération du stock central.");
            var stocks = await _service.GetStockByMagasinAsync(1);
            return Ok(stocks);
        }

        /// <summary>
        /// Récupère le stock d’un magasin donné.
        /// </summary>
        [HttpGet("stockmagasin/{magasinId:int}")]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetStockMagasin(int magasinId)
        {
            _logger.LogInformation("Récupération du stock pour le magasin {MagasinId}.", magasinId);
            var stocks = await _service.GetStockByMagasinAsync(magasinId);
            return Ok(stocks);
        }

        /// <summary>
        /// Récupère le stock d’un produit dans un magasin.
        /// </summary>
        /// <param name="magasinId">Identifiant du magasin</param>
        /// <param name="produitId">Identifiant du produit</param>
        [HttpGet("{magasinId:int}/{produitId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StockDto>> GetOne(int magasinId, int produitId)
        {
            _logger.LogInformation("Recherche du stock pour MagasinId={MagasinId}, ProduitId={ProduitId}.", magasinId, produitId);

            var dto = await _service.GetStockByMagasinProduitAsync(magasinId, produitId);
            if (dto is not null)
            {
                _logger.LogInformation("Stock trouvé : {Quantite} unités disponibles.", dto.Quantite);
                return Ok(dto);
            }

            _logger.LogWarning("Stock non trouvé pour MagasinId={MagasinId}, ProduitId={ProduitId}.", magasinId, produitId);
            return NotFound(new { message = $"Stock introuvable pour magasin #{magasinId} et produit #{produitId}." });
        }

        /// <summary>
        /// Mettre à jour le stock d’un produit dans un magasin.
        /// </summary>
        /// <param name="magasinId">Le magasin dans laquelle la vente a prit lieu. Si la vente s'est fait en ligne alors magasin id = 1</param>
        /// <param name="produitId"></param>
        /// <param name="quantite"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateStock(int magasinId, int produitId, int quantite)
        {
            _logger.LogInformation("Mise à jour du stock pour MagasinId={MagasinId}, ProduitId={ProduitId}, Quantité={Quantite}.", magasinId, produitId, quantite);
            try
            {
                await _service.UpdateStockAsync(magasinId, produitId, quantite);
                _logger.LogInformation("Stock mis à jour avec succès.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du stock pour MagasinId={MagasinId}, ProduitId={ProduitId}.", magasinId, produitId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}
