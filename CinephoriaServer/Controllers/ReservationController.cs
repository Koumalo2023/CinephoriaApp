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
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une liste de réservations sous forme de DTO.</returns>
        [HttpGet("user/{AppUserId}")]
        public async Task<IActionResult> GetUserReservations(string AppUserId)
        {
            try
            {
                var reservations = await _reservationService.GetUserReservationsAsync(AppUserId);
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
        /// Récupère la liste de toutes les réservations d'une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Une liste de réservations.</returns>
        [HttpGet("showtime/{showtimeId}")]
        public async Task<ActionResult<List<ReservationDto>>> GetReservationsByShowtime(int showtimeId)
        {
            try
            {
                // Appeler le service pour récupérer les réservations
                var reservations = await _reservationService.GetReservationsByShowtimeAsync(showtimeId);

                if (reservations == null || !reservations.Any())
                {
                    return NotFound("Aucune réservation trouvée pour cette séance.");
                }

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
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
                    return Ok("QRCode validé avec succès.");
                }
                else
                {
                    return BadRequest("Validation du QRCode échouée.");
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
