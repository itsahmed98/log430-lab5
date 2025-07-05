using MagasinCentral.Models;
using MagasinCentral.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace MagasinCentral.Controllers;

/// <summary>
/// Controller pour gérer les ventes.
/// </summary>
public class VenteController : Controller
{
    private readonly ILogger<VenteController> _logger;
    private readonly HttpClient _httpClientVente;
    private readonly HttpClient _httpClientProduit;
    private readonly HttpClient _httpClientMagasin;


    /// <summary>
    /// Constructeur pour initialiser les services nécessaires à la gestion des ventes.
    /// </summary>
    /// <param name="venteService"></param>
    /// <param name="produitService"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public VenteController(ILogger<VenteController> logger, 
        IHttpClientFactory httpClientFactoryVente,
        IHttpClientFactory httpClientFactoryProduit)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClientVente = httpClientFactoryVente?.CreateClient("VenteMcService") ?? throw new ArgumentNullException(nameof(httpClientFactoryVente));
        _httpClientProduit = httpClientFactoryProduit?.CreateClient("ProduitMcService") ?? throw new ArgumentNullException(nameof(httpClientFactoryProduit));
        _httpClientMagasin = httpClientFactoryVente?.CreateClient("MagasinMcService") ?? throw new ArgumentNullException(nameof(httpClientFactoryVente));
    }

    /// <summary>
    /// Affiche le formulaire pour enregistrer une vente dans un magasin spécifique.
    /// </summary>
    /// <param name="magasinId"></param>
    public async Task<IActionResult> Enregistrer()
    {
        var magasins = await _httpClientMagasin.GetFromJsonAsync<List<MagasinDto>>("");
        var produits = await _httpClientProduit.GetFromJsonAsync<List<ProduitDto>>("");

        ViewBag.Magasins = new SelectList(magasins, "MagasinId", "Nom");
        ViewBag.Produits = new SelectList(produits, "ProduitId", "Nom");

        var vm = new VenteCreateViewModel
        {
            Lignes = new List<LigneViewModel> { new() } // Commence avec une ligne vide
        };

        return View(vm);
    }

    /// <summary>
    /// Enregistre une vente pour un magasin donné avec les produits et quantités spécifiés.
    /// </summary>
    /// <param name="magasinId"></param>
    /// <param name="produitId"></param>
    /// <param name="quantite"></param>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Enregistrer(VenteCreateViewModel vm, string? action)
    {
        var magasins = await _httpClientMagasin.GetFromJsonAsync<List<MagasinDto>>("");
        var produits = await _httpClientProduit.GetFromJsonAsync<List<ProduitDto>>("");

        ViewBag.Magasins = new SelectList(magasins, "MagasinId", "Nom");
        ViewBag.Produits = new SelectList(produits, "ProduitId", "Nom");

        if (action == "add")
        {
            vm.Lignes.Add(new LigneViewModel());
            return View(vm);
        }

        if (!ModelState.IsValid)
            return View(vm);

        var payload = new
        {
            MagasinId = vm.MagasinId,
            ClientId = 0,
            IsEnLigne = false,
            Date = DateTime.UtcNow,
            Lignes = vm.Lignes
                .Where(l => l.Quantite > 0)
                .Select(l => new { l.ProduitId, l.Quantite })
                .ToList()
        };

        var response = await _httpClientVente.PostAsJsonAsync("", payload);

        if (response.IsSuccessStatusCode)
        {
            var created = await response.Content.ReadFromJsonAsync<JsonElement>();
            int venteId = created.GetProperty("venteId").GetInt32();
            TempData["Succès"] = $"Vente #{venteId} enregistrée.";
            return RedirectToAction("Liste");
        }

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Erreur API : {error}");
        return View(vm);
    }

    /// <summary>
    /// Annule une vente existante et restitue le stock au magasin.
    /// </summary>
    /// <param name="venteId"></param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Retour(int venteId)
    {
        var response = await _httpClientVente.DeleteAsync($"{_httpClientVente.BaseAddress}/{venteId}");

        if (response.IsSuccessStatusCode)
        {
            TempData["Succès"] = "Vente annulée avec succès.";
        }
        else
        {
            TempData["Erreur"] = "Erreur lors de l’annulation de la vente.";
        }

        return RedirectToAction(nameof(Liste));
    }

    /// <summary>
    /// Affiche la liste de toutes les ventes enregistrées, incluant les informations sur le magasin et les produits.
    /// </summary>
    public async Task<IActionResult> Liste()
    {
        var ventesAffichage = new List<VenteListeViewModel>();

        try
        {
            _logger.LogInformation("Récupération des ventes...");
            var ventes = await _httpClientVente.GetFromJsonAsync<List<VenteDto>>("");

            if (ventes == null || !ventes.Any())
                return View(ventesAffichage);

            foreach (var vente in ventes)
            {
                // Récupérer le nom du magasin
                var magasin = await _httpClientMagasin.GetFromJsonAsync<MagasinDto>($"{_httpClientMagasin.BaseAddress}/{vente.MagasinId}");

                var lignes = new List<LigneDetailViewModel>();

                foreach (var ligne in vente.Lignes)
                {
                    // Récupérer le nom du produit
                    var produit = await _httpClientProduit.GetFromJsonAsync<ProduitDto>($"{_httpClientProduit.BaseAddress}/{ligne.ProduitId}");

                    lignes.Add(new LigneDetailViewModel
                    {
                        NomProduit = produit?.Nom ?? $"Produit #{ligne.ProduitId}",
                        Quantite = ligne.Quantite,
                        PrixUnitaire = ligne.PrixUnitaire
                    });
                }

                ventesAffichage.Add(new VenteListeViewModel
                {
                    VenteId = vente.VenteId,
                    Date = vente.Date,
                    NomMagasin = magasin?.Nom ?? $"Magasin #{vente.MagasinId}",
                    Lignes = lignes
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des ventes.");
            ModelState.AddModelError("", "Erreur lors de la récupération des ventes.");
        }

        return View(ventesAffichage);
    }

}
