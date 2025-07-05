namespace MagasinCentral.Models
{
    /// <summary>
    /// Un data transfer object (DTO) représentant un produit dans le système pour passer au view.
    /// </summary>
    public class ProduitDto
    {
        /// <summary>
        /// l'identifiant unique du produit.
        /// </summary>
        public int ProduitId { get; set; }

        /// <summary>
        /// Le nom du produit.
        /// </summary>
        public string Nom { get; set; } = string.Empty;

        /// <summary>
        /// La catégorie du produit.
        /// </summary>
        public string? Categorie { get; set; }

        /// <summary>
        /// Le prix du produit.
        /// </summary>
        public decimal Prix { get; set; }

        /// <summary>
        /// La description du produit.
        /// </summary>
        public string? Description { get; set; }
    }
}
