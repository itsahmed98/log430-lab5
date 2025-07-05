namespace ECommerceMcService.Models
{
    public class PanierDto
    {
        public int PanierId { get; set; }
        public int ClientId { get; set; }
        public List<LignePanierDto> Lignes { get; set; } = new();
    }
}
