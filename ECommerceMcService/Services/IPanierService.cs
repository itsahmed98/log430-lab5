using ECommerceMcService.Models;

namespace ECommerceMcService.Services
{
    public interface IPanierService
    {
        /// <summary>
        /// Récupère le panier d'un client spécifique.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<PanierDto?> GetPanierParClient(int clientId);

        /// <summary>
        /// Modifier le panier d'un client en ajoutant ou en mettant à jour un produit.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="produitId"></param>
        /// <param name="quantite"></param>
        /// <returns></returns>
        Task AjouterOuMettreAJourProduit(int clientId, int produitId, int quantite);

        /// <summary>
        /// Supprimer un produit spécifique du panier d'un client.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="produitId"></param>
        /// <returns></returns>
        Task SupprimerProduit(int clientId, int produitId);

        /// <summary>
        /// Finaliser le panier d'un client, en enregistrant la commande.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task ViderPanier(int clientId);

        /// <summary>
        /// Calcule le total à payer du panier d'un client.
        /// </summary>
        /// <param name="panierId"></param>
        /// <returns></returns>
        Task<decimal> CalculerTotalAsync(int panierId);

        /// <summary>
        /// Valider une commande en vérifiant les détails et la disponibilité des produits.
        /// </summary>
        /// <param name="panierId"></param>
        /// <returns></returns>
        Task<bool> ValiderCommandeAsync(int panierId);
    }
}
