using CatalogueMcService.Models;

namespace CatalogueMcService.Services
{
    /// <summary>
    /// Interface avec les opérations pour gérer/mettre à jour les produits.
    /// </summary>
    public interface ICatalogueService
    {
        /// <summary>
        /// Retourne la liste de tous les produits.
        /// </summary>
        Task<List<Produit>> GetAllProduitsAsync();

        /// <summary>
        /// Récupère un produit par son ID.
        /// </summary>
        Task<Produit?> GetProduitByIdAsync(int produitId);

        /// <summary>
        /// Met à jour un produit existant.
        /// </summary>
        Task ModifierProduitAsync(int produitId, ProduitDto produit);

        /// <summary>Recherche de produits par identifiant, nom ou catégorie.</summary>
        Task<List<Produit>> RechercherProduitsAsync(string terme);
    }
}
