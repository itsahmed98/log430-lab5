using InventaireMcService.Models;

namespace InventaireMcService.Services
{
    public interface IStockService
    {
        /// <summary>
        /// Retrouve tous les stocks disponibles.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<StockDto>> GetAllStocksAsync();

        /// <summary>
        /// Retourne le stock d'un produit spécifique dans un magasin donné.
        /// </summary>
        /// <param name="magasinId"></param>
        /// <param name="produitId"></param>
        /// <returns></returns>
        Task<StockDto?> GetStockByMagasinProduitAsync(int magasinId, int produitId);

        /// <summary>
        /// Récupère le stock d'un magasin spécifique.
        /// </summary>
        /// <param name="magasinId"></param>
        /// <returns></returns>
        Task<IEnumerable<StockDto>> GetStockByMagasinAsync(int magasinId);

        /// <summary>
        /// Met à jour le stock d'un produit dans un magasin spécifique.
        /// </summary>
        /// <param name="magasinId"></param>
        /// <param name="produitId"></param>
        /// <param name="quantite"></param>
        /// <returns></returns>
        Task<bool> UpdateStockAsync(int magasinId, int produitId, int quantite);
    }
}
