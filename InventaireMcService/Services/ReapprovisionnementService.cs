using InventaireMcService.Data;
using InventaireMcService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventaireMcService.Services
{
    /// <summary>
    /// Service pour la gestion des demandes de réapprovisionnement.
    /// </summary>
    public class ReapprovisionnementService : IReapprovisionnementService
    {
        private readonly ILogger<ReapprovisionnementService> _logger;
        private readonly InventaireDbContext _context;
        private readonly IInventaireService _inventaire;

        public ReapprovisionnementService(ILogger<ReapprovisionnementService> logger, InventaireDbContext context, IInventaireService inventaire)
        {
            _logger = logger;
            _context = context;
            _inventaire = inventaire;
        }

        /// <inheritdoc/> 
        public async Task<DemandeReapprovisionnement> CreerDemandeAsync(int magasinId, int produitId, int quantite)
        {
            var demande = new DemandeReapprovisionnement
            {
                MagasinId = magasinId,
                ProduitId = produitId,
                QuantiteDemandee = quantite,
                Statut = "EN_ATTENTE"
            };

            _context.Demandes.Add(demande);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Demande de réapprovisionnement créée : Magasin {magasinId}, Produit {produitId}, Quantité {quantite}", magasinId, produitId, quantite);
            return demande;
        }

        /// <inheritdoc/> 
        public async Task<IEnumerable<DemandeReapprovisionnement>> GetDemandesEnAttenteAsync()
        {
            return await _context.Demandes
                .Where(d => d.Statut == "EN_ATTENTE")
                .ToListAsync();
        }

        /// <inheritdoc/> 
        public async Task<bool> ValiderDemandeAsync(int demandeId)
        {
            var demande = await _context.Demandes.FindAsync(demandeId);
            if (demande == null || demande.Statut != "EN_ATTENTE")
            {
                _logger.LogWarning("Demande introuvable ou déjà traitée : {demandeId}", demandeId);
                return false;
            }

            var succès = await _inventaire.TransférerStockAsync(demande.ProduitId, demande.MagasinId, demande.QuantiteDemandee);
            if (!succès)
            {
                _logger.LogWarning("Échec du transfert pour la demande {demandeId}", demandeId);
                return false;
            }

            demande.Statut = "VALIDÉE";
            await _context.SaveChangesAsync();

            _logger.LogInformation("Demande {demandeId} validée et stock transféré", demandeId);
            return true;
        }
    }
}
