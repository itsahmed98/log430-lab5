namespace AdministrationMcService.Models
{
    /// <summary>
    /// Représente un produit
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
    }
}
