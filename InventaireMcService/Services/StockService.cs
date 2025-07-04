using InventaireMcService.Data;
using InventaireMcService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventaireMcService.Services
{
    /// <summary>
    /// Service pour la gestion des inventaires et des transferts de stock.
    /// </summary>
    public class StockService : IStockService
    {
        private readonly InventaireDbContext _context;
        private readonly ILogger<StockService> _logger;

        public StockService(InventaireDbContext context, ILogger<StockService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<StockDto>> GetAllStocksAsync()
        {
            _logger.LogInformation("Récupération de tous les stocks.");
            try
            {
                var stocks = await _context.StockItems
                    .AsNoTracking()
                    .Select(si => new StockDto
                    {
                        MagasinId = si.MagasinId,
                        ProduitId = si.ProduitId,
                        Quantite = si.Quantite
                    })
                    .ToListAsync();

                _logger.LogInformation("Nombre de stocks récupérés : {Count}", stocks.Count);
                return stocks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de tous les stocks.");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<StockDto?> GetStockByMagasinProduitAsync(int magasinId, int produitId)
        {
            _logger.LogInformation("Récupération du stock pour MagasinId={MagasinId}, ProduitId={ProduitId}", magasinId, produitId);
            try
            {
                var si = await _context.StockItems
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.MagasinId == magasinId && x.ProduitId == produitId);

                if (si == null)
                {
                    _logger.LogWarning("Stock introuvable pour MagasinId={MagasinId}, ProduitId={ProduitId}", magasinId, produitId);
                    return null;
                }

                return new StockDto
                {
                    MagasinId = si.MagasinId,
                    ProduitId = si.ProduitId,
                    Quantite = si.Quantite
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du stock pour MagasinId={MagasinId}, ProduitId={ProduitId}", magasinId, produitId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<StockDto>> GetStockByMagasinAsync(int magasinId)
        {
            _logger.LogInformation("Récupération du stock pour le magasin {MagasinId}", magasinId);
            try
            {
                var stocks = await _context.StockItems
                    .Where(s => s.MagasinId == magasinId)
                    .Select(s => new StockDto
                    {
                        ProduitId = s.ProduitId,
                        MagasinId = s.MagasinId,
                        Quantite = s.Quantite
                    })
                    .ToListAsync();

                _logger.LogInformation("Nombre d'articles récupérés pour le magasin {MagasinId} : {Count}", magasinId, stocks.Count);
                return stocks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du stock pour le magasin {MagasinId}", magasinId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateStockAsync(int magasinId, int produitId, int quantite)
        {
            _logger.LogInformation("Mise à jour du stock : MagasinId={MagasinId}, ProduitId={ProduitId}, Quantité={Quantite}", magasinId, produitId, quantite);
            try
            {
                var stockLocal = await _context.StockItems
                    .FirstOrDefaultAsync(s => s.MagasinId == magasinId && s.ProduitId == produitId);

                if (stockLocal == null)
                {
                    _logger.LogWarning("Stock introuvable pour mise à jour : MagasinId={MagasinId}, ProduitId={ProduitId}", magasinId, produitId);
                    return false;
                }

                stockLocal.Quantite += quantite;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Stock mis à jour avec succès : MagasinId={MagasinId}, ProduitId={ProduitId}, NouvelleQuantité={Quantite}", magasinId, produitId, stockLocal.Quantite);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du stock : MagasinId={MagasinId}, ProduitId={ProduitId}", magasinId, produitId);
                throw;
            }
        }
    }
}
