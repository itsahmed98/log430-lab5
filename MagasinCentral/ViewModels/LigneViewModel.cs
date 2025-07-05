using System.ComponentModel.DataAnnotations;

namespace MagasinCentral.ViewModels
{
    public class LigneViewModel
    {
        [Required]
        public int ProduitId { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantite { get; set; }
    }
}
