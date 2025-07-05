namespace MagasinCentral.ViewModels
{
    /// <summary>
    /// Ligne de détail d'une vente, représentant un produit vendu avec sa quantité et son prix unitaire.
    /// </summary>
    public class LigneDetailViewModel
    {
        /// <summary>
        /// Nom du produit vendu.
        /// </summary>
        public string NomProduit { get; set; } = "";

        /// <summary>
        /// La quantité de produit vendue.
        /// </summary>
        public int Quantite { get; set; }

        /// <summary>
        /// Le prix unitaire du produit vendu.
        /// </summary>
        public decimal PrixUnitaire { get; set; }

        /// <summary>
        /// Le montant total pour cette ligne de vente, calculé comme Quantite * PrixUnitaire.
        /// </summary>
        public decimal Montant => Quantite * PrixUnitaire;
    }
}
