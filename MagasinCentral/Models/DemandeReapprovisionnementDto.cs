using System.ComponentModel.DataAnnotations;

namespace MagasinCentral.Models
{
    /// <summary>
    /// Représente une demande de réapprovisionnement d’un magasin.
    /// </summary>
    public class DemandeReapprovisionnementDto
    {
        /// <summary>
        /// Clé primaire de la demande.
        /// </summary>
        [Key]
        public int DemandeId { get; set; }

        /// <summary>
        /// Magasin qui initie la demande.
        /// </summary>
        [Required]
        public int MagasinId { get; set; }

        /// <summary>
        /// Produit concerné par la demande.
        /// </summary>
        [Required]
        public int ProduitId { get; set; }

        /// <summary>
        /// Quantité demandée.
        /// </summary>
        [Required]
        public int QuantiteDemandee { get; set; }

        /// <summary>
        /// Date et heure de la demande.
        /// </summary>
        [Required]
        public DateTime DateDemande { get; set; }

        /// <summary>
        /// Statut de la demande.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Statut { get; set; } = "EnAttente";
    }
}
