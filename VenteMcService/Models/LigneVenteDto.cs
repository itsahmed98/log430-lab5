using System.ComponentModel.DataAnnotations;

namespace VenteMcService.Models
{
    public class LigneVenteDto
    {
        /// <summary>
        /// L'identifiant du produit vendu dans cette ligne.
        /// </summary>
        public int ProduitId { get; set; }

        /// <summary>
        /// La quantité de produit vendue dans cette ligne.
        /// </summary>
        public int Quantite { get; set; }
    }
}
