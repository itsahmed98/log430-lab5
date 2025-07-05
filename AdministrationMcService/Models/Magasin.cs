using System.ComponentModel.DataAnnotations;

namespace AdministrationMcService.Models
{
    /// <summary>
    /// Représente un magasin physique.
    /// </summary>
    public class Magasin
    {
        /// <summary>
        /// Clé primaire du magasin.
        /// </summary>
        [Key]
        public int MagasinId { get; set; }

        /// <summary>
        /// Nom du magasin.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        /// <summary>
        /// Adresse du magasin.
        /// </summary>
        [MaxLength(200)]
        public string? Adresse { get; set; }
    }
}
