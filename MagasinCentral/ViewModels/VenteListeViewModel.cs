namespace MagasinCentral.ViewModels
{
    /// <summary>
    /// Represente le modèle de vue pour la liste des ventes.
    /// </summary>
    public class VenteListeViewModel
    {
        /// <summary>
        /// L'identifiant de la vente.
        /// </summary>
        public int VenteId { get; set; }

        /// <summary>
        /// La date de la vente.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Le nom du magasin où la vente a eu lieu.
        /// </summary>
        public string NomMagasin { get; set; } = "";

        /// <summary>
        /// Les lignes de détail de la vente.
        /// </summary>
        public List<LigneDetailViewModel> Lignes { get; set; } = new();

        /// <summary>
        /// La somme totale de la vente.
        /// </summary>
        public decimal Total => Lignes.Sum(l => l.Montant);
    }
}
