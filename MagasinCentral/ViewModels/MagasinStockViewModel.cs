namespace MagasinCentral.ViewModels
{
    public class MagasinStockViewModel
    {
        /// <summary>
        /// Nom du magasin.
        /// </summary>
        public string? NomMagasin { get; set; }

        /// <summary>
        /// L'identifiant du magasin.
        /// </summary>
        public int MagasinId { get; set; }

        /// <summary>
        /// Les produits disponibles dans ce magasin.
        /// </summary>
        public List<ProduitStockViewModel>? Produits { get; set; }
    }
}
