using AdministrationMcService.Models;

namespace AdministrationMcService.Services
{
    public interface IRapportService
    {
        /// <summary>
        /// Obtient un rapport consolidé de tous les magasins.
        /// </summary>
        /// <returns></returns>
        Task<RapportVentesDto> ObtenirRapportConsolideAsync();
    }
}
