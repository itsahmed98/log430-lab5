namespace InventaireMcService.Models
{
    /// <summary>
    /// DTO pour exposer le stock via l’API.
    /// </summary>
    public class StockDto
    {
        /// <summary>
        /// L'identifiant du magasin où le stock est situé.
        /// </summary>
        public int MagasinId { get; set; }

        /// <summary>
        /// L'identifiant du produit dans le stock.
        /// </summary>
        public int ProduitId { get; set; }

        /// <summary>
        /// La quantité de produit disponible en stock dans le magasin.
        /// </summary>
        public int Quantite { get; set; }
    }
}
