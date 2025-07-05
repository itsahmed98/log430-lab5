namespace AdministrationMcService.Models
{
    /// <summary>
    /// Représente une ligne de vente
    /// </summary>
    public class LigneVenteDto
    {
        /// <summary>
        /// L'identifiant unique de la ligne de vente.
        /// </summary>
        public int LigneVenteId { get; set; }

        /// <summary>
        /// L'identifiant de la vente à laquelle cette ligne appartient.
        /// </summary>
        public int VenteId { get; set; }

        /// <summary>
        /// L'identifiant du produit vendu dans cette ligne.
        /// </summary>
        public int ProduitId { get; set; }

        /// <summary>
        /// La quantité de produit vendue dans cette ligne.
        /// </summary>
        public int Quantite { get; set; }

        /// <summary>
        /// Le prix unitaire du produit au moment de la vente.
        /// </summary>
        public decimal PrixUnitaire { get; set; }
    }
}
