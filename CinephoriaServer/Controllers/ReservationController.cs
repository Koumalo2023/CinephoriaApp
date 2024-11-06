using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Authorization;
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
            try
            {
                var cinemas = await _reservationService.GetAllCinemasAsync();
                if (cinemas == null || !cinemas.Any())
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "Aucun cinéma disponible."
                    });
                }

                return Ok(cinemas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération des cinémas : {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Récupère tous les films disponibles dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">Identifiant du cinéma</param>
        [HttpGet("cinemas/{cinemaId}/movies")]
        public async Task<IActionResult> GetMoviesByCinemaAsync(int cinemaId)
        {
            try
            {
                var movies = await _reservationService.GetMoviesByCinemaAsync(cinemaId);
                if (movies == null || !movies.Any())
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "Aucun film disponible pour ce cinéma."
                    });
                }

                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération des films pour le cinéma {cinemaId} : {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Récupère et affiche le QR Code associé à une réservation dans le navigateur.
        /// </summary>
        /// <param name="reservationId">Identifiant de la réservation</param>
        /// <returns>QR Code sous forme d'image PNG</returns>
        [HttpGet("reservations/{reservationId}/qrcode")]
        [Authorize(Roles = RoleConfigurations.AdminEmployeeUser)]
        public async Task<IActionResult> GetReservationQrCodeAsync(int reservationId)
        {
            try
            {
                var result = await _reservationService.GetReservationQrCodeAsync(reservationId);
                if (!result.IsSucceed)
                {
                    return StatusCode(result.StatusCode, new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = result.StatusCode,
                        Message = result.Message
                    });
                }

                return File(result.Data, "image/png");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération du QR code pour la réservation {reservationId} : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Valide une réservation en utilisant un QR Code.
        /// </summary>
        /// <param name="qrCode">Le QR Code de la réservation</param>
        [HttpPost("reservations/validate")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> ValidateReservationAsync([FromBody] string qrCode)
        {
            if (string.IsNullOrEmpty(qrCode))
            {
                return BadRequest(new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Le QR Code ne peut pas être vide."
                });
            }

            try
            {
                var result = await _reservationService.ValidateReservationAsync(qrCode);
                if (!result.IsSucceed)
                {
                    return StatusCode(result.StatusCode, new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = result.StatusCode,
                        Message = result.Message
                    });
                }

                return Ok(new GeneralServiceResponse
                {
                    IsSucceed = true,
                    StatusCode = 200,
                    Message = "Réservation validée avec succès."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la validation de la réservation : {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Récupère toutes les réservations effectuées par un utilisateur donné.
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        [HttpGet("users/{userId}/reservations")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> GetReservationsByUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "L'identifiant de l'utilisateur ne peut pas être vide."
                });
            }

            try
            {
                var reservations = await _reservationService.GetReservationsByUserAsync(userId);
                if (reservations == null || !reservations.Any())
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "Aucune réservation trouvée pour cet utilisateur."
                    });
                }

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération des réservations pour l'utilisateur {userId} : {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Crée une nouvelle réservation pour un utilisateur authentifié.
        /// Vérifie d'abord les disponibilités des sièges avant de confirmer la réservation.
        /// </summary>
        /// <param name="reservationDto">Données de la réservation à créer</param>
        [HttpPost("reservations")]
        [Authorize(Roles = RoleConfigurations.User)]
        public async Task<IActionResult> CreateReservationAsync(string userId, [FromBody] ReservationViewModel reservationViewModel)
        {
            if (reservationViewModel == null)
            {
                return BadRequest(new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Les données de la réservation sont invalides."
                });
            }

            try
            {
                var result = await _reservationService.CreateReservationAsync(userId, reservationViewModel);
                if (!result.IsSucceed)
                {
                    return StatusCode(result.StatusCode, new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = result.StatusCode,
                        Message = result.Message
                    });
                }

                return Ok(new GeneralServiceResponse
                {
                    IsSucceed = true,
                    StatusCode = 201,
                    Message = "Réservation créée avec succès."                    
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la création de la réservation : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Récupère une réservation par son identifiant.
        /// </summary>
        /// <param name="reservationId">Identifiant de la réservation</param>
        [HttpGet("reservations/{reservationId}")]
        [Authorize(Roles = RoleConfigurations.AdminEmployeeUser)]
        public async Task<IActionResult> GetReservationByIdAsync(int reservationId)
        {
            try
            {
                var reservation = await _reservationService.GetReservationByIdAsync(reservationId);
                if (reservation == null)
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "Réservation non trouvée."
                    });
                }

                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération de la réservation {reservationId} : {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Récupère toutes les réservations pour une séance donnée.
        /// </summary>
        /// <param name="showtimeId">Identifiant de la séance</param>
        [HttpGet("showtimes/{showtimeId}/reservations")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> GetReservationsByShowtimeAsync(string showtimeId)
        {
            try
            {
                var reservations = await _reservationService.GetReservationsByShowtimeAsync(showtimeId);
                if (reservations == null || !reservations.Any())
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "Aucune réservation trouvée pour cette séance."
                    });
                }

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération des réservations pour la séance {showtimeId} : {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Supprime une réservation existante et libère les sièges réservés.
        /// </summary>
        /// <param name="reservationId">Identifiant de la réservation à supprimer</param>
        [HttpDelete("reservations/{reservationId}")]
        [Authorize(Roles = RoleConfigurations.AdminEmployeeUser)]
        public async Task<IActionResult> DeleteReservationAsync(int reservationId)
        {
            try
            {
                var result = await _reservationService.DeleteReservationAsync(reservationId);
                if (!result.IsSucceed)
                {
                    return StatusCode(result.StatusCode, new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = result.StatusCode,
                        Message = result.Message
                    });
                }

                return Ok(new GeneralServiceResponse
                {
                    IsSucceed = true,
                    StatusCode = 200,
                    Message = "Réservation supprimée avec succès."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la suppression de la réservation {reservationId} : {ex.Message}"
                });
            }
        }
    }
}
