using ECommerceMcService.Data;
using ECommerceMcService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMcService.Services
{
    /// <summary>
    /// Service pour gérer les opérations liées aux clients.
    /// </summary>
    public class ClientService : IClientService
    {
        private readonly ILogger<ClientService> _logger;
        private readonly ECommerceDbContext _context;

        public ClientService(ILogger<ClientService> logger, ECommerceDbContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc/> 
        public async Task<List<ClientDto>> GetAllAsync()
        {
            _logger.LogInformation("Début de la récupération de tous les clients.");

            try
            {
                var clients = await _context.Clients
                    .Select(c => new ClientDto
                    {
                        ClientId = c.ClientId,
                        Nom = c.Nom,
                        Courriel = c.Courriel,
                        Adresse = c.Adresse
                    }).ToListAsync();

                _logger.LogInformation("Récupération de {Count} clients terminée avec succès.", clients.Count);
                return clients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la liste des clients.");
                throw;
            }
        }

        /// <inheritdoc/> 
        public async Task<ClientDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Récupération du client avec l’ID {ClientId}.", id);

            try
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                {
                    _logger.LogWarning("Aucun client trouvé avec l’ID {ClientId}.", id);
                    return null;
                }

                _logger.LogInformation("Client ID {ClientId} trouvé avec succès.", id);

                return new ClientDto
                {
                    ClientId = client.ClientId,
                    Nom = client.Nom,
                    Courriel = client.Courriel,
                    Adresse = client.Adresse
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du client avec l’ID {ClientId}.", id);
                throw;
            }
        }

        /// <inheritdoc/> 
        public async Task<ClientDto> CreateAsync(CreateClientDto dto)
        {
            _logger.LogInformation("Création d’un nouveau client : {Nom}, {Courriel}.", dto.Nom, dto.Courriel);

            try
            {
                var client = new Client
                {
                    Nom = dto.Nom,
                    Courriel = dto.Courriel,
                    Adresse = dto.Adresse
                };

                _context.Clients.Add(client);
                await _context.SaveChangesAsync();

                var nouvClient = new ClientDto
                {
                    ClientId = client.ClientId,
                    Nom = client.Nom,
                    Courriel = client.Courriel,
                    Adresse = client.Adresse
                };

                _logger.LogInformation("Client créé avec succès. ID assigné : {ClientId}.", nouvClient.ClientId);
                return nouvClient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du client : {Nom}, {Courriel}.", dto.Nom, dto.Courriel);
                throw;
            }
        }
    }
}
