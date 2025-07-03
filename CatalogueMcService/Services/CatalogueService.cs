using CatalogueMcService.Data;
using CatalogueMcService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CatalogueMcService.Services;

/// <summary>
/// Service pour gérer les opérations liées aux produits.
/// </summary>
public class CatalogueService : ICatalogueService
{
    private readonly ILogger<CatalogueService> _logger;
    private readonly CatalogueDbContext _context;
    private readonly IMemoryCache _cache;

    public CatalogueService(ILogger<CatalogueService> logger, CatalogueDbContext context, IMemoryCache cache)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <inheritdoc />
    public async Task<List<Produit>> GetAllProduitsAsync()
    {
        try
        {
            _logger.LogInformation("Récupération de tous les produits depuis la base de données.");
            return await _context.Produits.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de tous les produits.");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Produit?> GetProduitByIdAsync(int produitId)
    {
        if (produitId <= 0)
        {
            _logger.LogWarning("ID de produit invalide fourni : {ProduitId}", produitId);
            throw new ArgumentException("L'identifiant du produit est invalide.");
        }

        string cacheKey = $"produit_{produitId}";

        if (_cache.TryGetValue(cacheKey, out Produit? produit))
        {
            _logger.LogInformation("Produit {ProduitId} trouvé dans le cache.", produitId);
            return produit;
        }

        try
        {
            _logger.LogInformation("Produit {ProduitId} non trouvé dans le cache, interrogation de la base de données.", produitId);
            produit = await _context.Produits.FirstOrDefaultAsync(p => p.ProduitId == produitId);

            if (produit != null)
            {
                var options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                _cache.Set(cacheKey, produit, options);
                _logger.LogInformation("Produit {ProduitId} mis en cache.", produitId);
            }
            else
            {
                _logger.LogWarning("Produit {ProduitId} non trouvé dans la base de données.", produitId);
            }

            return produit;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du produit {ProduitId}.", produitId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task ModifierProduitAsync(int produitId, ProduitDto produitDto)
    {
        if (produitDto == null)
        {
            _logger.LogWarning("Objet ProduitDto nul fourni pour la modification.");
            throw new ArgumentNullException(nameof(produitDto));
        }

        try
        {
            var produit = await _context.Produits.FirstOrDefaultAsync(p => p.ProduitId == produitId);
            if (produit == null)
            {
                _logger.LogWarning("Produit {ProduitId} introuvable pour la modification.", produitId);
                throw new KeyNotFoundException($"Produit avec ID {produitId} introuvable.");
            }

            produit.Nom = produitDto.Nom;
            produit.Categorie = produitDto.Categorie;
            produit.Prix = produitDto.Prix;
            produit.Description = produitDto.Description;

            _context.Produits.Update(produit);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Produit {ProduitId} modifié avec succès.", produitId);

            // Invalider le cache
            _cache.Remove($"produit_{produitId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la modification du produit {ProduitId}.", produitId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<Produit>> RechercherProduitsAsync(string terme)
    {
        try
        {
            terme = terme?.Trim().ToLower() ?? "";

            _logger.LogInformation("Recherche de produits avec le terme : {Terme}", terme);

            var resultats = await _context.Produits
                .AsNoTracking()
                .Where(p =>
                    p.ProduitId.ToString() == terme ||
                    p.Nom.ToLower().Contains(terme) ||
                    (p.Categorie != null && p.Categorie.ToLower().Contains(terme))
                )
                .ToListAsync();

            _logger.LogInformation("{Count} produit(s) trouvé(s) pour le terme '{Terme}'.", resultats.Count, terme);
            return resultats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la recherche de produits avec le terme '{Terme}'.", terme);
            throw;
        }
    }
}
