using System.ComponentModel.DataAnnotations;

namespace InventaireMcService.Models
{
    /// <summary>
    /// Représente une demande de réapprovisionnement d'un produit dans un magasin.
    /// </summary>
    public class DemandeReapprovisionnement
    {
        /// <summary>
        /// L'identifiant unique de la demande de réapprovisionnement.
        /// </summary>
        [Key]
        public int DemandeId { get; set; }

        /// <summary>
        /// L'identifiant du magasin où la demande est faite.
        /// </summary>
        [Required]
        public int MagasinId { get; set; }

        /// <summary>
        /// L'identifiant du produit à réapprovisionner.
        /// </summary>
        [Required]
        public int ProduitId { get; set; }

        /// <summary>
        /// La quantité demandée pour le réapprovisionnement.
        /// </summary>
        [Required]
        public int QuantiteDemandee { get; set; }

        /// <summary>
        /// Le statut de la demande de réapprovisionnement.
        /// </summary>
        [Required]
        public string Statut { get; set; } = "EN_ATTENTE"; // "VALIDÉE", "REJETÉE"
    }
}
