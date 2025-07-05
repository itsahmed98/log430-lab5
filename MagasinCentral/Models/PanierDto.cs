namespace MagasinCentral.Models
{
    /// <summary>
    /// Represente un panier de vente dans le système de vente.
    /// </summary>
    public class PanierDto
    {
        /// <summary>
        /// L'identifiant unique du panier.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Les lignes de panier associées à ce panier.
        /// </summary>
        public List<LignePanierDto> Lignes { get; set; } = new();
    }
}
