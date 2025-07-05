using ECommerceMcService.Models;

namespace ECommerceMcService.Services
{
    public interface IClientService
    {
        /// <summary>
        /// Retourner tous les clients disponibles.
        /// </summary>
        /// <returns></returns>
        Task<List<ClientDto>> GetAllAsync();

        /// <summary>
        /// Retourner un client spécifique par son ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ClientDto?> GetByIdAsync(int id);

        /// <summary>
        /// Créer un nouveau client.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<ClientDto> CreateAsync(CreateClientDto dto);
    }
}
