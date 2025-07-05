using AdministrationMcService.Models;

namespace AdministrationMcService.Services
{
    /// <summary>
    /// Service pour la gestion des magasins.
    /// </summary>
    public interface IMagasinService
    {
        /// <summary>
        /// Retourner tous les magasins
        /// </summary>
        /// <returns>Liste des magasins</returns>
        Task<List<Magasin>> GetAllAsync();

        /// <summary>
        /// Retourner un magasin par son ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Un Magasin</returns>
        Task<Magasin?> GetMagasinByIdAsync(int id);
    }
}
