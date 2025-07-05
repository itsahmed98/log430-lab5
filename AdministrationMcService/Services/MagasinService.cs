using AdministrationMcService.Data;
using AdministrationMcService.Models;
using Microsoft.EntityFrameworkCore;

namespace AdministrationMcService.Services
{
    /// <summary>
    /// Service pour la gestion des magasins.
    /// </summary>
    public class MagasinService : IMagasinService
    {
        private readonly ILogger<MagasinService> _logger;
        private readonly AdminDbContext _context;

        /// <summary>
        /// Constructeur du service MagasinService.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MagasinService(ILogger<MagasinService> logger, AdminDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public Task<List<Magasin>> GetAllAsync()
        {
            _logger.LogInformation("Récupération de tous les magasins.");
            return _context.Magasins.AsNoTracking().ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Magasin?> GetMagasinByIdAsync(int id)
        {
            _logger.LogInformation("Récupération du magasin avec ID {Id}.", id);
            var magasin = await _context.Magasins.FindAsync(id);

            if (magasin == null)
            {
                _logger.LogWarning("Magasin avec ID {Id} non trouvé.", id);
            }
            return magasin;
        }
    }
}
