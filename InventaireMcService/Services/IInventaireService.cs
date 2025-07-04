namespace InventaireMcService.Services
{
    /// <summary>
    /// Service pour gérer les opérations d'inventaire, comme le transfert de stock entre magasins.
    /// </summary>
    public interface IInventaireService
    {
        /// <summary>
        /// Transfère une quantité de stock d'un produit du centre de distribution vers un magasin spécifique.
        /// </summary>
        /// <param name="produitId">Le produit qu'on veut restock</param>
        /// <param name="magasinId">Le magasin qui a fait la demande</param>
        /// <param name="quantite">La quantité voulu</param>
        /// <returns></returns>
        Task<bool> TransférerStockAsync(int produitId, int magasinId, int quantite);
    }
}
