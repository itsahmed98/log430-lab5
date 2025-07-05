using System.ComponentModel.DataAnnotations;

namespace MagasinCentral.Models
{
    /// <summary>
    /// Représente un magasin physique.
    /// </summary>
    public class MagasinDto
    {
        /// <summary>
        /// Clé primaire du magasin.
        /// </summary>
        public int MagasinId { get; set; }

        /// <summary>
        /// Nom du magasin.
        /// </summary>
        [Required]
        public string Nom { get; set; } = string.Empty;

        /// <summary>
        /// Adresse du magasin.
        /// </summary>
        public string? Adresse { get; set; }
    }
}
