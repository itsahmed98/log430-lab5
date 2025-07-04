using InventaireMcService.Data;
using InventaireMcService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventaireMcService.Services
{
    /// <summary>
    /// Service pour la gestion des inventaires et des transferts de stock.
    /// </summary>
    public class InventaireService : IInventaireService
    {
        private readonly InventaireDbContext _context;
        private readonly ILogger<InventaireService> _logger;

        public InventaireService(InventaireDbContext context, ILogger<InventaireService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <inheritdoc/> 
        public async Task<bool> TransférerStockAsync(int produitId, int magasinId, int quantite)
        {
            _logger.LogInformation("Tentative de transfert de {quantite} unités du produit {produitId} vers le magasin {magasinId}", quantite, produitId, magasinId);
            var stockCentral = await _context.StockItems
                .FirstOrDefaultAsync(s => s.MagasinId == 1 && s.ProduitId == produitId);

            if (stockCentral == null || stockCentral.Quantite < quantite)
            {
                _logger.LogWarning("Stock central insuffisant pour ProduitId={ProduitId}", produitId);
                return false;
            }

            var stockLocal = await _context.StockItems
                .FirstOrDefaultAsync(s => s.MagasinId == magasinId && s.ProduitId == produitId);

            if (magasinId > 1)
            {
                stockCentral.Quantite -= quantite;
            }

            if (stockLocal != null)
                stockLocal.Quantite += quantite;
            else
                _context.StockItems.Add(new StockItem { MagasinId = magasinId, ProduitId = produitId, Quantite = quantite });

            await _context.SaveChangesAsync();
            _logger.LogInformation("Transfert effectué : {quantite} unités du produit {produitId} vers magasin {magasinId}", quantite, produitId, magasinId);
            return true;
        }
    }
}
