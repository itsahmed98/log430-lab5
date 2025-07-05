using AdministrationMcService.Models;

namespace AdministrationMcService.Services
{
    public interface IPerformanceService
    {
        /// <summary>
        /// Récupère tous les indicateurs de performance.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Performance>> GetAllPerformancesAsync();
    }
}
