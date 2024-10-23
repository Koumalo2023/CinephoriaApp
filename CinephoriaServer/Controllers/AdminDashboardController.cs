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

        // 1. GetReservationsCountByMovieAsync
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 2. GetTopMoviesByReservationsAsync
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 3. GetWeeklyReservationsCountByMovieAsync
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 4. GetReservationTrendByMovieAsync
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 5. GetReservationSummaryByCinemaAsync
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
