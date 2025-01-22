using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles sous forme de DTO.</returns>
        [HttpGet("available/{sessionId}")]
        public async Task<IActionResult> GetAvailableSeats(int sessionId)
        {
            try
            {
                var result = await _seatService.GetAvailableSeatsAsync(sessionId);
                return Ok(new { Message = "Liste des sièges disponibles récupérée avec succès.", Data = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la récupération des sièges disponibles." });
            }
        }

        /// <summary>
        /// Ajoute un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="dto">Les données du siège à ajouter.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        [HttpPost("handicap-add-seat")]
        public async Task<IActionResult> AddHandicapSeat([FromBody] AddHandicapSeatDto dto)
        {
            try
            {
                var result = await _seatService.AddHandicapSeatAsync(dto.TheaterId, dto.SeatNumber);
                if (result)
                {
                    return Ok(new { Message = "Siège réservé pour personnes à mobilité réduite ajouté avec succès." });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "L'ajout du siège réservé pour personnes à mobilité réduite a échoué." });
                }
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de l'ajout du siège réservé pour personnes à mobilité réduite." });
            }
        }

        /// <summary>
        /// Supprime un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="dto">Les données du siège à supprimer.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        [HttpDelete("handicap-delete-seat")]
        public async Task<IActionResult> RemoveHandicapSeat([FromBody] RemoveHandicapSeatDto dto)
        {
            try
            {
                var result = await _seatService.RemoveHandicapSeatAsync(dto.TheaterId, dto.SeatNumber);
                if (result)
                {
                    return Ok(new { Message = "Siège réservé pour personnes à mobilité réduite supprimé avec succès." });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "La suppression du siège réservé pour personnes à mobilité réduite a échoué." });
                }
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la suppression du siège réservé pour personnes à mobilité réduite." });
            }
        }
    }
}
