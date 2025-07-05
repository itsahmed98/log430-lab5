namespace MagasinCentral.Models
{
    /// <summary>
    /// Représente un DTO des produit plus vendu
    /// </summary>
    public class ProduitLePlusVenduDto
    {
        /// <summary>
        /// L'identifiant du produit
        /// </summary>
        public int ProduitId { get; set; }

        /// <summary>
        /// Le nom du produit
        /// </summary>
        public string Nom { get; set; } = "";

        /// <summary>
        /// La quantité totale vendu de ce produit
        /// </summary>
        public int QuantiteTotaleVendue { get; set; }
    }
}
