using System.ComponentModel.DataAnnotations;

namespace MagasinCentral.ViewModels
{
    public class VenteCreateViewModel
    {
        [Required]
        public int MagasinId { get; set; }

        [MinLength(1, ErrorMessage = "Ajoutez au moins une ligne de vente.")]
        public List<LigneViewModel> Lignes { get; set; } = new();
    }
}
