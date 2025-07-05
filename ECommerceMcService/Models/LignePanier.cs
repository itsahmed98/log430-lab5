using System.ComponentModel.DataAnnotations;

namespace ECommerceMcService.Models
{
    /// <summary>
    /// Représente les lignes d'articels dans un panier d'achats.
    /// </summary>
    public class LignePanier
    {
        /// <summary>
        /// L'identifiant unique de la ligne de panier.
        /// </summary>
        [Key]
        public int LignePanierId { get; set; }

        /// <summary>
        /// L'identifiant du panier auquel cette ligne appartient.
        /// </summary>
        public int PanierId { get; set; }

        /// <summary>
        /// L'identifiant du produit ajouté dans cette ligne de panier.
        /// </summary>
        public int ProduitId { get; set; }

        /// <summary>
        /// La quantité de produit ajoutée dans cette ligne de panier.
        /// </summary>
        public int Quantite { get; set; }
    }
}
