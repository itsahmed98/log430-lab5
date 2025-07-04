using VenteMcService.Models;

namespace VenteMcService.Services
{
    public interface IVenteService
    {
        /// <summary>
        /// Récupère toutes les ventes.
        /// </summary>
        /// <returns></returns>
        Task<List<Vente>> GetAllAsync();

        /// <summary>
        /// Crée une nouvelle vente.
        /// </summary>
        /// <param name="vente"></param>
        /// <returns></returns>
        Task<Vente> CreateAsync(Vente vente);

        /// <summary>
        /// Supprime une vente par son identifiant.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// Récupère une vente par son identifiant.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Vente?> GetByIdAsync(int id);

        /// <summary>
        /// Récupère toutes les ventes pour un magasin spécifique.
        /// </summary>
        /// <param name="magasinId"></param>
        /// <returns></returns>
        Task<List<Vente>> GetByMagasinAsync(int magasinId);

    }
}
