namespace MagasinCentral.Models
{
    /// <summary>
    /// Represente le stock
    /// </summary>
    public class StockDto
    {
        /// <summary>
        /// L'identifiant du magasin
        /// </summary>
        public int MagasinId { get; set; }

        /// <summary>
        /// L'identifiant du produit
        /// </summary>
        public int ProduitId { get; set; }

        /// <summary>
        /// Le nom du produit
        /// </summary>
        public string NomProduit { get; set; } = string.Empty;

        /// <summary>
        /// La quantité du produit
        /// </summary>
        public int Quantite { get; set; }
    }
}
