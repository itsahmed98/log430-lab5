namespace MagasinCentral.ViewModels
{
    /// <summary>
    /// Reprsente un produit et sa quantité disponible dans un magasin.
    /// </summary>
    public class ProduitStockViewModel
    {
        /// <summary>
        /// L'identifiant du produit.
        /// </summary>
        public int ProduitId { get; set; }

        /// <summary>
        /// Le nom du produit.
        /// </summary>
        public string? NomProduit { get; set; }

        /// <summary>
        /// Quantité disponible du produit dans le magasin.
        /// </summary>
        public int QuantiteDisponible { get; set; }

        /// <summary>
        /// L'identifiant du magasin où le produit est stocké.
        /// </summary>
        public int MagasinId { get; set; }
    }
}
