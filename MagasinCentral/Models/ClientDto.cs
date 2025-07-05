namespace MagasinCentral.Models
{
    /// <summary>
    /// Represente un client dans le système.
    /// </summary>
    public class ClientDto
    {
        /// <summary>
        /// L'identifiant unique du client.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Le nom du client.
        /// </summary>
        public string Nom { get; set; } = string.Empty;

        /// <summary>
        /// Le couriel du client.
        /// </summary>
        public string Courriel { get; set; } = string.Empty;

        /// <summary>
        /// L'adresse du client.
        /// </summary>
        public string Adresse { get; set; } = string.Empty;
    }
}
