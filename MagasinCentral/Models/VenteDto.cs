namespace MagasinCentral.Models
{
    public class VenteDto
    {
        public int VenteId { get; set; }
        public int? MagasinId { get; set; }
        public int? ClientId { get; set; }
        public bool IsEnLigne { get; set; }
        public DateTime Date { get; set; }
        public List<LigneVenteDto> Lignes { get; set; } = new();
    }
}
