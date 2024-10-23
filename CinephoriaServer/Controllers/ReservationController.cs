using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Http;
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
        /// Récupère la liste de tous les cinémas disponibles.
        /// </summary>
        [HttpGet("cinemas")]
        public async Task<IActionResult> GetAllCinemasAsync()
        {
            var cinemas = await _reservationService.GetAllCinemasAsync();
            if (cinemas == null || !cinemas.Any())
            {
                return NotFound(new { Message = "Aucun cinéma disponible." });
            }

            return Ok(cinemas);
        }

        /// <summary>
        /// Récupère tous les films disponibles dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">Identifiant du cinéma</param>
        [HttpGet("cinemas/{cinemaId}/movies")]
        public async Task<IActionResult> GetMoviesByCinemaAsync(int cinemaId)
        {
            var movies = await _reservationService.GetMoviesByCinemaAsync(cinemaId);
            if (movies == null || !movies.Any())
            {
                return NotFound(new { Message = "Aucun film disponible pour ce cinéma." });
            }

            return Ok(movies);
        }

        /// <summary>
        /// Récupère et affiche le QR Code associé à une réservation dans le navigateur.
        /// </summary>
        /// <param name="reservationId">Identifiant de la réservation</param>
        /// <returns>QR Code sous forme d'image PNG</returns>
        [HttpGet("reservations/{reservationId}/qrcode")]
        public async Task<IActionResult> GetReservationQrCodeAsync(int reservationId)
        {
            // Appeler le service pour récupérer les données du QR code
            var result = await _reservationService.GetReservationQrCodeAsync(reservationId);

            // Si la réservation n'est pas trouvée ou s'il y a une erreur
            if (!result.IsSucceed)
            {
                return StatusCode(result.StatusCode, new { Message = result.Message });
            }

            // Retourner l'image du QR code sous forme de fichier avec le bon type MIME (image/png)
            return File(result.Data, "image/png");
        }


        /// <summary>
        /// Valide une réservation en utilisant un QR Code.
        /// </summary>
        /// <param name="qrCode">Le QR Code de la réservation</param>
        [HttpPost("reservations/validate")]
        public async Task<IActionResult> ValidateReservationAsync([FromBody] string qrCode)
        {
            if (string.IsNullOrEmpty(qrCode))
            {
                return BadRequest(new { Message = "Le QR Code ne peut pas être vide." });
            }

            var result = await _reservationService.ValidateReservationAsync(qrCode);
            if (!result.IsSucceed)
            {
                return StatusCode(result.StatusCode, new { Message = result.Message });
            }

            return Ok(new { Message = "Réservation validée avec succès." });
        }

        /// <summary>
        /// Récupère toutes les réservations effectuées par un utilisateur donné.
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        [HttpGet("users/{userId}/reservations")]
        public async Task<IActionResult> GetReservationsByUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { Message = "L'identifiant de l'utilisateur ne peut pas être vide." });
            }

            var reservations = await _reservationService.GetReservationsByUserAsync(userId);
            if (reservations == null || !reservations.Any())
            {
                return NotFound(new { Message = "Aucune réservation trouvée pour cet utilisateur." });
            }

            return Ok(reservations);
        }

        /// <summary>
        /// Crée une nouvelle réservation pour un utilisateur authentifié.
        /// Vérifie d'abord les disponibilités des sièges avant de confirmer la réservation.
        /// </summary>
        /// <param name="reservationDto">Données de la réservation à créer</param>
        [HttpPost("reservations")]
        public async Task<IActionResult> CreateReservationAsync(string userId, [FromBody] ReservationViewModel reservationViewModel)
        {
            if (reservationViewModel == null)
            {
                return BadRequest(new { Message = "Les données de la réservation sont invalides." });
            }

            // Assurez-vous que le service prend en compte le bon type (ReservationViewModel) 
            var result = await _reservationService.CreateReservationAsync(userId, reservationViewModel);
            if (!result.IsSucceed)
            {
                return StatusCode(result.StatusCode, new { Message = result.Message });
            }

            return Ok(new { Message = "Réservation créée avec succès.", Reservation = result.Data });
        }


        /// <summary>
        /// Récupère une réservation par son identifiant.
        /// </summary>
        /// <param name="reservationId">Identifiant de la réservation</param>
        [HttpGet("reservations/{reservationId}")]
        public async Task<IActionResult> GetReservationByIdAsync(int reservationId)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(reservationId);
            if (reservation == null)
            {
                return NotFound(new { Message = "Réservation non trouvée." });
            }

            return Ok(reservation);
        }

        /// <summary>
        /// Récupère toutes les réservations pour une séance donnée.
        /// </summary>
        /// <param name="showtimeId">Identifiant de la séance</param>
        [HttpGet("showtimes/{showtimeId}/reservations")]
        public async Task<IActionResult> GetReservationsByShowtimeAsync(string showtimeId)
        {
            var reservations = await _reservationService.GetReservationsByShowtimeAsync(showtimeId);
            if (reservations == null || !reservations.Any())
            {
                return NotFound(new { Message = "Aucune réservation trouvée pour cette séance." });
            }

            return Ok(reservations);
        }

        /// <summary>
        /// Supprime une réservation existante et libère les sièges réservés.
        /// </summary>
        /// <param name="reservationId">Identifiant de la réservation à supprimer</param>
        [HttpDelete("reservations/{reservationId}")]
        public async Task<IActionResult> DeleteReservationAsync(int reservationId)
        {
            var result = await _reservationService.DeleteReservationAsync(reservationId);
            if (!result.IsSucceed)
            {
                return StatusCode(result.StatusCode, new { Message = result.Message });
            }

            return Ok(new { Message = "Réservation supprimée avec succès." });
        }
    }
}
