namespace InventaireMcService.Models
{
    /// <summary>
    /// DTO pour créer une nouvelle demande de réapprovisionnement.
    /// </summary>
    public class CreerDemandeDto
    {
        /// <summary>
        /// L'identifiant du magasin où la demande est faite.
        /// </summary>
        public int MagasinId { get; set; }

        /// <summary>
        /// L'identifiant du produit à réapprovisionner.
        /// </summary>
        public int ProduitId { get; set; }

        /// <summary>
        /// La quantité demandée pour le réapprovisionnement.
        /// </summary>
        public int QuantiteDemandee { get; set; }
    }
}
