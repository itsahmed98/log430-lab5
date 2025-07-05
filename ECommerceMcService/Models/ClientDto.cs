namespace ECommerceMcService.Models
{
    public class ClientDto
    {
        public int ClientId { get; set; }
        public string Nom { get; set; } = null!;
        public string Courriel { get; set; } = null!;
        public string? Adresse { get; set; }
    }
}
