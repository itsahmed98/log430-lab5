using System.ComponentModel.DataAnnotations;

namespace CatalogueMcService.Models
{
    /// <summary>
    /// Représente un produit vendu dans le système.
    /// </summary>
    public class Produit
    {
        /// <summary>
        /// l'identifiant unique du produit.
        /// </summary>
        [Key]
        public int ProduitId { get; set; }

        /// <summary>
        /// Le nom du produit.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        /// <summary>
        /// La catégorie du produit.
        /// </summary>
        [MaxLength(50)]
        public string? Categorie { get; set; }

        /// <summary>
        /// Le prix du produit.
        /// </summary>
        [Required]
        public decimal Prix { get; set; }

        /// <summary>
        /// La description du produit.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }
    }
}