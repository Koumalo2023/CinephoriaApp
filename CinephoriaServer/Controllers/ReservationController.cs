using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances disponibles.</returns>
        [HttpGet("movie/{movieId}/sessions")]
        public async Task<IActionResult> GetMovieSessions(int movieId)
        {
            try
            {
                var sessions = await _reservationService.GetMovieSessionsAsync(movieId);
                return Ok(sessions);
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
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles.</returns>
        [HttpGet("showtime/{showtimeId}/seats")]
        public async Task<IActionResult> GetAvailableSeats(int showtimeId)
        {
            try
            {
                var seats = await _reservationService.GetAvailableSeatsAsync(showtimeId);
                return Ok(seats);
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
        /// Récupère la liste des réservations d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une liste de réservations sous forme de DTO.</returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserReservations(string userId)
        {
            try
            {
                var reservations = await _reservationService.GetUserReservationsAsync(userId);
                return Ok(reservations);
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
        /// Valide un QRCode scanné pour une réservation.
        /// </summary>
        /// <param name="qrCodeData">Les données du QRCode scanné.</param>
        /// <returns>Une réponse indiquant si la validation a réussi.</returns>
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateSession([FromBody] string qrCodeData)
        {
            try
            {
                bool isValid = await _reservationService.ValidatedSession(qrCodeData);

                if (isValid)
                {
                    return Ok(new { Message = "QRCode validé avec succès." });
                }
                else
                {
                    return BadRequest(new { Message = "Validation du QRCode échouée." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }


        /// <summary>
        /// Crée une nouvelle réservation.
        /// </summary>
        /// <param name="createReservationDto">Les données de la réservation à créer.</param>
        /// <returns>Un message indiquant le succès de l'opération.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto createReservationDto)
        {
            try
            {
                var result = await _reservationService.CreateReservationAsync(createReservationDto);
                return Ok(new { Message = result });
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
        /// Annule une réservation existante.
        /// </summary>
        /// <param name="reservationId">L'identifiant de la réservation à annuler.</param>
        /// <returns>Un message indiquant le succès de l'opération.</returns>
        [HttpDelete("cancel/{reservationId}")]
        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            try
            {
                var result = await _reservationService.CancelReservationAsync(reservationId);
                return Ok(new { Message = result });
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
