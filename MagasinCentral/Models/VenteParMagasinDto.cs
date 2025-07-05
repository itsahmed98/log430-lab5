namespace MagasinCentral.Models
{
    /// <summary>
    /// Représente les ventes spécifique à un magasin
    /// </summary>
    public class VenteParMagasinDto
    {
        /// <summary>
        /// L'identifiant du magasin
        /// </summary>
        public int MagasinId { get; set; }

        /// <summary>
        /// Le nombre total des ventes
        /// </summary>
        public decimal TotalVentes { get; set; }

        /// <summary>
        /// Le nombre total des transaction (ventes) faits
        /// </summary>
        public int NombreTransactions { get; set; }
    }
}
