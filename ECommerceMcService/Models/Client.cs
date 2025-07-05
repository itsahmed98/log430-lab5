using System.ComponentModel.DataAnnotations;

namespace ECommerceMcService.Models
{
    /// <summary>
    /// Représente un client dans le système.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// L'identifiant unique du client.
        /// </summary>
        [Key]
        public int ClientId { get; set; }

        /// <summary>
        /// Le nom du client.
        /// </summary>
        [Required]
        public string Nom { get; set; } = null!;

        /// <summary>
        /// Le courriel du client.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Courriel { get; set; } = null!;

        /// <summary>
        /// L'adresse du client.
        /// </summary>
        public string? Adresse { get; set; }
    }
}
