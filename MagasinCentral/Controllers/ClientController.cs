using MagasinCentral.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagasinCentral.Controllers
{
    /// <summary>
    /// Controller pour gérer les clients dans le magasin central.
    /// </summary>
    public class ClientController : Controller
    {
        private readonly HttpClient _http;
        private readonly ILogger<ClientController> _logger;

        public ClientController(IHttpClientFactory factory, ILogger<ClientController> logger)
        {
            _http = factory.CreateClient("ClientMcService");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Affiche la liste des clients.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Requête GET /Client : récupération de la liste des clients.");

            try
            {
                var clients = await _http.GetFromJsonAsync<List<ClientDto>>("");

                if (clients == null)
                {
                    _logger.LogWarning("Aucun client récupéré depuis ClientMcService.");
                    return View(new List<ClientDto>());
                }

                _logger.LogInformation("Récupération de {Count} clients réussie.", clients.Count);
                return View(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des clients depuis ClientMcService.");
                return View("Error");
            }
        }

        /// <summary>
        /// Affiche le formulaire de création de client.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("Requête GET /Client/Create : affichage du formulaire de création.");
            return View();
        }

        /// <summary>
        /// Traite la soumission du formulaire de création.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(ClientDto client)
        {
            _logger.LogInformation("Requête POST /Client/Create : tentative de création du client {Nom}, {Courriel}.", client.Nom, client.Courriel);

            try
            {
                var response = await _http.PostAsJsonAsync("", client);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Client créé avec succès.");
                    return RedirectToAction("Index");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Échec de la création du client. Code: {StatusCode}. Détails: {Content}", response.StatusCode, errorContent);

                ModelState.AddModelError("", "Erreur lors de la création du client.");
                return View(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l’envoi de la requête de création du client.");
                ModelState.AddModelError("", "Erreur interne. Veuillez réessayer plus tard.");
                return View(client);
            }
        }
    }
}
