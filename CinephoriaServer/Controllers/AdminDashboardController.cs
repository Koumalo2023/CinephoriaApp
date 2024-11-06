using CinephoriaServer.Configurations;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminDashboardController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        /// <summary>
        /// Récupère le nombre total de réservations pour un film spécifique
        /// sur une période donnée.
        /// </summary>
        /// <param name="movieId">Identifiant du film</param>
        /// <param name="startDate">Date de début de la période</param>
        /// <param name="endDate">Date de fin de la période</param>
        /// <returns>Nombre de réservations</returns>
        [HttpGet("reservations-count")]
        public async Task<IActionResult> GetReservationsCountByMovieAsync(string movieId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var reservationCount = await _adminDashboardService.GetReservationsCountByMovieAsync(movieId, startDate, endDate);
                return Ok(reservationCount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur interne du serveur lors de la récupération du nombre de réservations : {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Récupère les films les plus réservés sur une période donnée.
        /// </summary>
        /// <param name="startDate">Date de début de la période</param>
        /// <param name="endDate">Date de fin de la période</param>
        /// <param name="topN">Le nombre de films à récupérer</param>
        /// <returns>Liste des films et du nombre de réservations</returns>
        [HttpGet("top-movies")]
        public async Task<IActionResult> GetTopMoviesByReservationsAsync(DateTime startDate, DateTime endDate, int topN)
        {
            try
            {
                var topMovies = await _adminDashboardService.GetTopMoviesByReservationsAsync(startDate, endDate, topN);
                return Ok(topMovies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur interne du serveur lors de la récupération des films les plus réservés : {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Récupère les réservations hebdomadaires pour un film spécifique.
        /// </summary>
        /// <param name="movieId">Identifiant du film</param>
        /// <returns>Un dictionnaire avec la date de début de la semaine et le nombre de réservations</returns>
        [HttpGet("weekly-reservations-count")]
        public async Task<IActionResult> GetWeeklyReservationsCountByMovieAsync(string movieId)
        {
            try
            {
                var weeklyReservations = await _adminDashboardService.GetWeeklyReservationsCountByMovieAsync(movieId);
                return Ok(weeklyReservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur interne du serveur lors de la récupération du nombre de réservations hebdomadaires : {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Récupère la tendance des réservations pour un film donné sur une période.
        /// </summary>
        /// <param name="movieId">Identifiant du film</param>
        /// <param name="startDate">Date de début de la période</param>
        /// <param name="endDate">Date de fin de la période</param>
        /// <returns>Liste des tendances de réservation par date</returns>
        [HttpGet("reservation-trend")]
        public async Task<IActionResult> GetReservationTrendByMovieAsync(string movieId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var reservationTrends = await _adminDashboardService.GetReservationTrendByMovieAsync(movieId, startDate, endDate);
                return Ok(reservationTrends);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur interne du serveur lors de la récupération de la tendance des réservations : {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Récupère un résumé des réservations par cinéma sur une période donnée.
        /// </summary>
        /// <param name="startDate">Date de début de la période</param>
        /// <param name="endDate">Date de fin de la période</param>
        /// <returns>Liste des cinémas avec leur total de réservations</returns>
        [HttpGet("cinema-reservation-summary")]
        public async Task<IActionResult> GetReservationSummaryByCinemaAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var cinemaReservationSummary = await _adminDashboardService.GetReservationSummaryByCinemaAsync(startDate, endDate);
                return Ok(cinemaReservationSummary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur interne du serveur lors de la récupération du résumé des réservations pour les cinémas : {ex.Message}"
                });
            }
        }

    }
}
