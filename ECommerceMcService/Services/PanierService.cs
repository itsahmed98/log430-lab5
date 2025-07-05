using ECommerceMcService.Data;
using ECommerceMcService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMcService.Services
{
    /// <summary>
    /// Service pour gérer les opérations liées aux paniers.
    /// </summary>
    public class PanierService : IPanierService
    {
        private readonly ILogger<PanierService> _logger;
        private readonly ECommerceDbContext _context;
        private readonly HttpClient _httpCatalogue;
        private readonly HttpClient _httpInventaire;
        private readonly HttpClient _httpVente;

        public PanierService(ILogger<PanierService> logger, ECommerceDbContext db, IHttpClientFactory client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = db ?? throw new ArgumentNullException(nameof(db));
            _httpCatalogue = client.CreateClient("CatalogueMcService") ?? throw new ArgumentNullException(nameof(client));
            _httpInventaire = client.CreateClient("InventaireMcService") ?? throw new ArgumentNullException(nameof(client));
            _httpVente = client.CreateClient("VenteMcService") ?? throw new ArgumentNullException(nameof(client));
        }

        /// <inheritdoc/>
        public async Task<PanierDto?> GetPanierParClient(int clientId)
        {
            _logger.LogInformation("Récupération du panier pour le client {ClientId}.", clientId);

            try
            {
                var panier = await _context.Paniers
                    .Include(p => p.Lignes)
                    .FirstOrDefaultAsync(p => p.ClientId == clientId);

                if (panier == null)
                {
                    _logger.LogWarning("Aucun panier trouvé pour le client {ClientId}.", clientId);
                    return null;
                }

                var dto = new PanierDto
                {
                    PanierId = panier.PanierId,
                    ClientId = panier.ClientId,
                    Lignes = panier.Lignes.Select(l => new LignePanierDto
                    {
                        ProduitId = l.ProduitId,
                        Quantite = l.Quantite
                    }).ToList()
                };

                _logger.LogInformation("Panier récupéré avec succès pour le client {ClientId}.", clientId);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du panier pour le client {ClientId}.", clientId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task AjouterOuMettreAJourProduit(int clientId, int produitId, int quantite)
        {
            _logger.LogInformation("Ajout/Mise à jour du produit {ProduitId} (quantité {Quantite}) pour le client {ClientId}.", produitId, quantite, clientId);

            try
            {
                var panier = await _context.Paniers
                    .Include(p => p.Lignes)
                    .FirstOrDefaultAsync(p => p.ClientId == clientId);

                if (panier == null)
                {
                    panier = new Panier { ClientId = clientId };
                    _context.Paniers.Add(panier);
                    _logger.LogInformation("Nouveau panier créé pour le client {ClientId}.", clientId);
                }

                var ligne = panier.Lignes.FirstOrDefault(l => l.ProduitId == produitId);
                if (ligne != null)
                {
                    ligne.Quantite = quantite;
                    _logger.LogInformation("Quantité mise à jour pour le produit {ProduitId} dans le panier du client {ClientId}.", produitId, clientId);
                }
                else
                {
                    panier.Lignes.Add(new LignePanier
                    {
                        ProduitId = produitId,
                        Quantite = quantite
                    });
                    _logger.LogInformation("Produit {ProduitId} ajouté au panier du client {ClientId}.", produitId, clientId);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout/mise à jour du produit {ProduitId} pour le client {ClientId}.", produitId, clientId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task SupprimerProduit(int clientId, int produitId)
        {
            _logger.LogInformation("Suppression du produit {ProduitId} du panier du client {ClientId}.", produitId, clientId);

            try
            {
                var panier = await _context.Paniers.Include(p => p.Lignes)
                    .FirstOrDefaultAsync(p => p.ClientId == clientId);

                if (panier == null)
                {
                    _logger.LogWarning("Aucun panier trouvé pour le client {ClientId} lors de la tentative de suppression.", clientId);
                    return;
                }

                var ligne = panier.Lignes.FirstOrDefault(l => l.ProduitId == produitId);
                if (ligne != null)
                {
                    panier.Lignes.Remove(ligne);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Produit {ProduitId} supprimé du panier du client {ClientId}.", produitId, clientId);
                }
                else
                {
                    _logger.LogWarning("Produit {ProduitId} non trouvé dans le panier du client {ClientId}.", produitId, clientId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du produit {ProduitId} pour le client {ClientId}.", produitId, clientId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task ViderPanier(int clientId)
        {
            _logger.LogInformation("Vidage du panier pour le client {ClientId}.", clientId);

            try
            {
                var panier = await _context.Paniers.Include(p => p.Lignes)
                    .FirstOrDefaultAsync(p => p.ClientId == clientId);

                if (panier != null && panier.Lignes.Any())
                {
                    _context.LignesPanier.RemoveRange(panier.Lignes);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Panier vidé pour le client {ClientId}.", clientId);
                }
                else
                {
                    _logger.LogWarning("Aucun panier ou lignes à vider pour le client {ClientId}.", clientId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du vidage du panier pour le client {ClientId}.", clientId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ValiderCommandeAsync(int panierId)
        {
            _logger.LogInformation("Validation de la commande pour le panier ID {PanierId}.", panierId);

            var panier = await _context.Paniers
                .Include(p => p.Lignes)
                .FirstOrDefaultAsync(p => p.PanierId == panierId);

            if (panier == null || !panier.Lignes.Any())
            {
                _logger.LogWarning("Panier ID {PanierId} introuvable ou vide.", panierId);
                return false;
            }
                

            foreach (var ligne in panier.Lignes)
            {
                var stock = await _httpInventaire.GetFromJsonAsync<StockDto>($"{_httpInventaire.BaseAddress}/stocks/1/{ligne.ProduitId}");

                if (stock is null || stock.Quantite < ligne.Quantite)
                {
                    _logger.LogWarning("Produit ID {ProduitId} non disponible en stock pour la quantité demandée.", ligne.ProduitId);
                    return false;
                }
            }

            var payload = new
            {
                MagasinId = 1, // stock central
                ClientId = panier.ClientId,
                IsEnLigne = true,
                Date = DateTime.UtcNow,
                Lignes = panier.Lignes.Select(l => new
                {
                    l.ProduitId,
                    l.Quantite
                }).ToList()
            };

            var response = await _httpVente.PostAsJsonAsync("", payload);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erreur lors de la création de la vente pour la commande du client ID {ClientId}", panier.ClientId);
                return false;
            }

            _logger.LogInformation("Commande validée avec succès pour le client ID {ClientId}", panier.ClientId);
            return true;
        }

        /// <inheritdoc/>
        public async Task<decimal> CalculerTotalAsync(int panierId)
        {
            var panier = await _context.Paniers
                .Include(p => p.Lignes)
                .FirstOrDefaultAsync(p => p.PanierId == panierId);

            if (panier == null || panier.Lignes == null)
                throw new Exception("Panier introuvable.");

            decimal total = 0m;

            foreach (var ligne in panier.Lignes)
            {
                var temp = $"{_httpCatalogue.BaseAddress}/{ligne.ProduitId}";
                var produit = await _httpCatalogue.GetFromJsonAsync<ProduitDto>(
                    $"{_httpCatalogue.BaseAddress}/{ligne.ProduitId}");

                if (produit != null)
                {
                    total += ligne.Quantite * produit.Prix;
                }
            }

            return total;
        }
    }
}
