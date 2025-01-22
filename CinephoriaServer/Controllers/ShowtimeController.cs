using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimeController : ControllerBase
    {
        private readonly IShowtimeService _showtimeService;

        public ShowtimeController(IShowtimeService showtimeService)
        {
            _showtimeService = showtimeService;
        }

        /// <summary>
        /// Crée une nouvelle séance (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="createShowtimeDto">Les données de la séance à créer.</param>
        /// <returns>Une réponse indiquant si la séance a été créée avec succès.</returns>
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateShowtime([FromBody] CreateShowtimeDto createShowtimeDto)
        {
            try
            {
                await _showtimeService.CreateShowtimeAsync(createShowtimeDto);
                return StatusCode(StatusCodes.Status201Created, new { Message = "Séance créée avec succès." });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Met à jour les informations d'une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="updateShowtimeDto">Les données mises à jour de la séance.</param>
        /// <returns>Une réponse indiquant si la séance a été mise à jour avec succès.</returns>
        [Authorize(Roles = "Admin,Employee")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateShowtime([FromBody] UpdateShowtimeDto updateShowtimeDto)
        {
            try
            {
                await _showtimeService.UpdateShowtimeAsync(updateShowtimeDto);
                return Ok(new { Message = "Séance mise à jour avec succès." });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Supprime une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance à supprimer.</param>
        /// <returns>Une réponse indiquant si la séance a été supprimée avec succès.</returns>
        [Authorize(Roles = "Admin,Employee")]
        [HttpDelete("delete/{showtimeId}")]
        public async Task<IActionResult> DeleteShowtime(int showtimeId)
        {
            try
            {
                await _showtimeService.DeleteShowtimeAsync(showtimeId);
                return Ok(new { Message = "Séance supprimée avec succès." });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère la liste de toutes les séances.
        /// </summary>
        /// <returns>Une liste de séances sous forme de DTO.</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllShowtimes()
        {
            try
            {
                var showtimes = await _showtimeService.GetAllShowtimesAsync();
                return Ok(showtimes);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère les détails d'une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Les détails de la séance sous forme de DTO.</returns>
        [HttpGet("{showtimeId}")]
        public async Task<IActionResult> GetShowtimeDetails(int showtimeId)
        {
            try
            {
                var showtimeDetails = await _showtimeService.GetShowtimeDetailsAsync(showtimeId);
                return Ok(showtimeDetails);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

    }
}
