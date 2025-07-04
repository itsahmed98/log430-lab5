using InventaireMcService.Models;

namespace InventaireMcService.Services
{
    /// <summary>
    /// Service pour la gestion des demandes de réapprovisionnement de produits dans les magasins.
    /// </summary>
    public interface IReapprovisionnementService
    {
        /// <summary>
        /// Crée une nouvelle demande de réapprovisionnement pour un produit dans un magasin.
        /// </summary>
        /// <param name="magasinId"></param>
        /// <param name="produitId"></param>
        /// <param name="quantite"></param>
        /// <returns></returns>
        Task<DemandeReapprovisionnement> CreerDemandeAsync(int magasinId, int produitId, int quantite);

        /// <summary>
        /// Valide une demande de réapprovisionnement existante.
        /// </summary>
        /// <param name="demandeId"></param>
        /// <returns></returns>
        Task<bool> ValiderDemandeAsync(int demandeId);

        /// <summary>
        /// Retourne toutes les demandes de réapprovisionnement en attente.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DemandeReapprovisionnement>> GetDemandesEnAttenteAsync();
    }
}
