using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Http;
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
        /// Crée une séance.
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateShowtime([FromBody] ShowtimeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralServiceResponseData<string>
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Les données fournies ne sont pas valides."
                });
            }

            var response = await _showtimeService.CreateShowtimeAsync(model);
            if (!response.IsSucceed)
            {
                return StatusCode(response.StatusCode, response.Message);
            }

            return Ok(response.Data);
        }

        /// <summary>
        /// Récupère toutes les séaces pour un cinema et pour un film spécifique par son identifiant.
        /// </summary>
        [HttpGet("movie/{movieId}/cinema/{cinemaId}")]
        public async Task<IActionResult> GetShowtimesForMovieInCinema(string movieId, int cinemaId)
        {
            var result = await _showtimeService.GetShowtimesForMovieInCinemaAsync(movieId, cinemaId);
            if (!result.IsSucceed)
                return StatusCode(result.StatusCode, result.Message);

            return Ok(result.Data);
        }
        /// <summary>
        /// Récupère les séaces pour un utilisateur par son identifiant.
        /// </summary>
        [HttpGet("user/{userId}/reservations")]
        public async Task<IActionResult> GetShowtimesForAuthenticatedUser(string userId)
        {
            var result = await _showtimeService.GetShowtimesForAuthenticatedUserAsync(userId);
            if (!result.IsSucceed)
                return StatusCode(result.StatusCode, result.Message);

            return Ok(result.Data);
        }
        /// <summary>
        /// met à jour une séance existante par son identifiant.
        /// </summary>
        [HttpPut("{showtimeId}")]
        public async Task<IActionResult> UpdateShowtime(string showtimeId, [FromBody] ShowtimeViewModel model)
        {
            var result = await _showtimeService.UpdateShowtimeAsync(showtimeId, model);
            if (!result.IsSucceed)
                return StatusCode(result.StatusCode, result.Message);

            return Ok(result.Message);
        }

        /// <summary>
        /// Supprime une séance existante par son identifiant.
        /// </summary>
        [HttpDelete("{showtimeId}")]
        public async Task<IActionResult> DeleteShowtime(string showtimeId)
        {
            var result = await _showtimeService.DeleteShowtimeAsync(showtimeId);
            if (!result.IsSucceed)
                return StatusCode(result.StatusCode, result.Message);

            return Ok(result.Message);
        }

        /// <summary>
        /// Récupère les séances auxquelles l'utilisateur a un billet pour le jour courant et les jours suivants.
        /// </summary>
        [HttpGet("user/{userId}/upcoming")]
        public async Task<IActionResult> GetUserShowtimesForTodayAndFuture(string userId)
        {
            var result = await _showtimeService.GetUserShowtimesForTodayAndFutureAsync(userId);
            if (!result.IsSucceed)
                return StatusCode(result.StatusCode, result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Vérifie si une séance chevauche une autre dans la même salle.
        /// </summary>
        [HttpGet("theater/{theaterId}/overlap")]
        public async Task<IActionResult> CheckShowtimeOverlap(string theaterId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime, [FromQuery] string? showtimeId = null)
        {
            var result = await _showtimeService.IsShowtimeOverlappingAsync(theaterId, startTime, endTime, showtimeId);
            if (!result.IsSucceed)
                return StatusCode(result.StatusCode, result.Message);

            return Ok(result.Data);
        }




    }
}
