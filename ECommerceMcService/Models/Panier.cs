using System.ComponentModel.DataAnnotations;

namespace ECommerceMcService.Models
{
    /// <summary>
    /// Représente un panier d'achats pour un client.
    /// </summary>
    public class Panier
    {
        /// <summary>
        /// L'identifiant unique du panier.
        /// </summary>
        [Key]
        public int PanierId { get; set; }

        /// <summary>
        /// L'identifiant du client auquel appartient le panier.
        /// </summary>
        [Required]
        public int ClientId { get; set; }

        /// <summary>
        /// Les lignes du panier, représentant les articles ajoutés par le client.
        /// </summary>
        public List<LignePanier> Lignes { get; set; } = new();
    }
}
