namespace MagasinCentral.Models
{
    public class CommandeValidationDto
    {
        public int ClientId { get; set; }
        public List<LigneCommandeDto> Lignes { get; set; } = new();
    }
}
