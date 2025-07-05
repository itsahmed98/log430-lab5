namespace MagasinCentral.Models
{
    /// <summary>
    /// Représente un rapport de vente
    /// </summary>
    public class RapportVentesDto
    {
        /// <summary>
        /// La date de création du rapport
        /// </summary>
        public DateTime DateGeneration { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Les ventes par magasin
        /// </summary>
        public List<VenteParMagasinDto> VentesParMagasin { get; set; } = new();

        /// <summary>
        /// Les produits les plus vendus
        /// </summary>
        public List<ProduitLePlusVenduDto> ProduitsLesPlusVendus { get; set; } = new();

        /// <summary>
        /// Les stocks restants
        /// </summary>
        public List<StockDto> StocksRestants { get; set; } = new();
    }
}
