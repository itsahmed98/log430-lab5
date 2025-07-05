using AdministrationMcService.Data;
using AdministrationMcService.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;

namespace AdministrationMcService.Services
{
    public class PerformanceService : IPerformanceService
    {
        private readonly PerformanceDbContext _context;
        private readonly ILogger<PerformanceService> _logger;

        public PerformanceService(ILogger<PerformanceService> logger, PerformanceDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Performance>> GetAllPerformancesAsync()
        {
            _logger.LogInformation("Accès à la base de données pour récupérer les performances.");
            var result = await _context.Performances
                .AsNoTracking()
                .ToListAsync();

            _logger.LogInformation("Récupération terminée : {Count} enregistrements trouvés.", result.Count);
            return result;
        }
    }
}
