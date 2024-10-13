using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }
        /// <summary>
        /// Récupère les informations du cinéma par identifiant.
        /// </summary>
        /// <param name="cinemaId">Identifiant du cinéma.</param>
        /// <returns>Informations du cinéma.</returns>
        [HttpGet("cinema-info/{cinemaId}")]
        public async Task<IActionResult> GetCinemaInfo(int cinemaId)
        {
            try
            {
                var cinemaInfo = await _contactService.GetCinemaInfoAsync(cinemaId);
                if (cinemaInfo == null)
                {
                    return NotFound(new { message = "Cinéma non trouvé." });
                }

                return Ok(cinemaInfo);
            }
            catch (Exception ex)
            {
                // Gestion de l'erreur et retour d'une réponse générique
                return StatusCode(500, new { message = "Erreur lors de la récupération des informations du cinéma.", details = ex.Message });
            }
        }

        /// <summary>
        /// Envoie une demande de contact au cinéma.
        /// </summary>
        /// <param name="contactViewModel">Objet contenant les informations de la demande de contact.</param>
        /// <returns>La demande de contact créée.</returns>
        [HttpPost("contact")]
        public async Task<IActionResult> CreateContact([FromBody] ContactViewModel contactViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdContact = await _contactService.CreateContactAsync(contactViewModel);

                // Retourner un message de succès et les détails du contact créé
                return Ok(new { message = "Contact créé avec succès.", contact = createdContact });
            }
            catch (Exception ex)
            {
                // En cas d'erreur, retourner une réponse d'erreur avec les détails
                return StatusCode(500, new { message = "Erreur lors de la création de la demande de contact.", details = ex.Message });
            }
        }

    }
}
