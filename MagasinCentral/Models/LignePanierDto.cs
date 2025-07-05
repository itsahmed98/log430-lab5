namespace MagasinCentral.Models
{
    /// <summary>
    /// represente une ligne de panier dans le système de vente.
    /// </summary>
    public class LignePanierDto
    {
        /// <summary>
        /// Le identifiant du produit dans le panier.
        /// </summary>
        public int ProduitId { get; set; }

        /// <summary>
        /// La quantité du produit dans le panier.
        /// </summary>
        public int Quantite { get; set; }
    }
}
