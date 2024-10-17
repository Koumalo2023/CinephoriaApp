using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        /// <summary>
        /// Récupère tous les films disponibles.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        [HttpGet("all")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> GetAllMovies()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            return Ok(movies);
        }

        /// <summary>
        /// Récupère tous les films disponibles dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste de films disponibles dans ce cinéma.</returns>
        [HttpGet("cinema/{cinemaId}")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> GetMoviesByCinema(int cinemaId)
        {
            var movies = await _movieService.GetMoviesByCinemaAsync(cinemaId);
            return Ok(movies);
        }

        /// <summary>
        /// Récupère les détails d'un film par son identifiant.
        /// </summary>
        /// <param name="filmId">L'identifiant du film.</param>
        /// <returns>Les détails du film.</returns>
        [HttpGet("{filmId}")]
        [Authorize(Roles = RoleConfigurations.AdminEmployeeUser)]
        public async Task<IActionResult> GetMovieById(string filmId)
        {
            var movie = await _movieService.GetMovieByIdAsync(filmId);
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie);
        }

        /// <summary>
        /// Filtre les films par cinéma, genre et date.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma (optionnel).</param>
        /// <param name="genre">Le genre du film (optionnel).</param>
        /// <param name="date">La date des séances (optionnelle).</param>
        /// <returns>Une liste filtrée de films.</returns>
        [HttpGet("filter")]
        [Authorize(Roles = RoleConfigurations.AdminEmployeeUser)]
        public async Task<IActionResult> FilterMovies([FromQuery] int? cinemaId, [FromQuery] string genre, [FromQuery] DateTime? date)
        {
            var movies = await _movieService.FilterMoviesAsync(cinemaId, genre, date);
            return Ok(movies);
        }

        /// <summary>
        /// Crée un nouveau film (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="movieViewModel">Les données du film à créer.</param>
        /// <returns>Le film créé.</returns>
        [HttpPost("create")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> CreateMovie([FromBody] MovieViewModel movieViewModel)
        {
           
            var createdMovie = await _movieService.CreateMovieAsync(movieViewModel); ;
            if (!createdMovie.IsSucceed)
            {
                return StatusCode(createdMovie.StatusCode, createdMovie.Message);
            }

            return Ok(createdMovie.Data);

        }

        /// <summary>
        /// Met à jour un film existant (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="filmId">L'identifiant du film à mettre à jour.</param>
        /// <param name="movieViewModel">Les nouvelles données du film.</param>
        /// <returns>Le film mis à jour.</returns>
        [HttpPut("{filmId}")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> UpdateMovie(string filmId, [FromBody] MovieViewModel movieViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedMovie = await _movieService.UpdateMovieAsync(filmId, movieViewModel);
            if (updatedMovie == null)
            {
                return NotFound();
            }
            return Ok(updatedMovie);
        }

        /// <summary>
        /// Supprime un film existant (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="filmId">L'identifiant du film à supprimer.</param>
        /// <returns>Une tâche représentant l'opération de suppression.</returns>
        [HttpDelete("{filmId}")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> DeleteMovie(string filmId)
        {
            await _movieService.DeleteMovieAsync(filmId);
            return NoContent();
        }

        /// <summary>
        /// Soumet un avis pour un film.
        /// </summary>
        /// <param name="reviewViewModel">Les données de l'avis à soumettre.</param>
        /// <returns>L'avis soumis.</returns>
        [HttpPost("reviews/submit")]
        [Authorize(Roles = RoleConfigurations.User)]
        public async Task<IActionResult> SubmitReview([FromBody] ReviewViewModel reviewViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var review = await _movieService.SubmitReviewAsync(reviewViewModel);
            return Ok(review);
        }

        /// <summary>
        /// Récupère les avis d'un film.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste d'avis sur le film.</returns>
        [HttpGet("{movieId}/reviews")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> GetReviewsByMovieId(string movieId)
        {
            var reviews = await _movieService.GetReviewsByMovieIdAsync(movieId);
            return Ok(reviews);
        }

        /// <summary>
        /// Soumet une note pour un film.
        /// </summary>
        /// <param name="movieRatingViewModel">Les informations de la note à soumettre.</param>
        /// <returns>La note de film nouvellement soumise.</returns>
        [HttpPost("ratings/submit")]
        [Authorize(Roles = RoleConfigurations.User)]
        public async Task<IActionResult> SubmitMovieRating([FromBody] MovieRatingViewModel movieRatingViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var movieRating = await _movieService.SubmitMovieRatingAsync(movieRatingViewModel);
            return Ok(movieRating);
        }

        /// <summary>
        /// Valide une note pour un film.
        /// Cette action est réservée aux administrateurs et aux employés.
        /// </summary>
        /// <param name="movieRatingId">L'identifiant de la note à valider.</param>
        /// <returns>La note de film validée.</returns>
        [HttpPut("ratings/validate/{movieRatingId}")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> ValidateMovieRating(int movieRatingId)
        {
            try
            {
                var validatedMovieRating = await _movieService.ValidateMovieRatingAsync(movieRatingId);
                return Ok(validatedMovieRating);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Supprime une note sur un film.
        /// Cette action est réservée aux administrateurs et aux employés.
        /// </summary>
        /// <param name="movieRatingId">L'identifiant de la note à supprimer.</param>
        /// <returns>Une réponse indiquant que la suppression a réussi.</returns>
        [HttpDelete("ratings/{movieRatingId}")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> DeleteMovieRating(int movieRatingId)
        {
            try
            {
                await _movieService.DeleteMovieRatingAsync(movieRatingId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
