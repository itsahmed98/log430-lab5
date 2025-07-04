using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VenteMcService.Models
{
    /// <summary>
    /// Représente une ligne dans une vente.
    /// </summary>
    public class LigneVente
    {
        /// <summary>
        /// L'identifiant unique de la ligne de vente.
        /// </summary>
        [Key]
        public int LigneVenteId { get; set; }

        /// <summary>
        /// L'identifiant de la vente à laquelle cette ligne appartient.
        /// </summary>
        [Required]
        public int VenteId { get; set; }

        /// <summary>
        /// L'identifiant du produit vendu dans cette ligne.
        /// </summary>
        [Required]
        public int ProduitId { get; set; }

        /// <summary>
        /// La quantité de produit vendue dans cette ligne.
        /// </summary>
        [Required]
        public int Quantite { get; set; }

        /// <summary>
        /// Le prix unitaire du produit au moment de la vente.
        /// </summary>
        [Required]
        public decimal PrixUnitaire { get; set; }
    }
}
