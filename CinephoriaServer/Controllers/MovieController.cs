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
        private readonly IImageService _imageService;

        public MovieController(IMovieService movieService, IImageService imageService)
        {
            _movieService = movieService;
            _imageService = imageService;
        }



        /// <summary>
        /// Récupère tous les films disponibles.
        /// </summary>
        /// <returns>Une liste de tous les films disponibles.</returns>
        [HttpGet("all")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> GetAllMovies()
        {
            try
            {
                var movies = await _movieService.GetAllMoviesAsync();
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération des films : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Récupère tous les films disponibles dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">Identifiant du cinéma.</param>
        /// <returns>Une liste de films disponibles dans le cinéma spécifié.</returns>
        [HttpGet("cinema/{cinemaId}")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> GetMoviesByCinema(int cinemaId)
        {
            try
            {
                var movies = await _movieService.GetMoviesByCinemaAsync(cinemaId);
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
        /// Récupère les détails d'un film spécifique, y compris les séances disponibles.
        /// </summary>
        /// <param name="filmId">L'identifiant unique du film.</param>
        /// <returns>Les détails du film, y compris les séances associées.</returns>
        [HttpGet("{filmId}")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployeeUser)]
        public async Task<IActionResult> GetMovieById(string filmId)
        {
            try
            {
                var movie = await _movieService.GetMovieByIdAsync(filmId);
                if (movie == null)
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "Film non trouvé."
                    });
                }

                return Ok(movie);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération du film {filmId} : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Filtre les films par cinéma, genre ou jour spécifique.
        /// Si un paramètre n'est pas fourni, il n'est pas pris en compte dans le filtrage.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <param name="genre">Le genre du film.</param>
        /// <param name="date">Le jour spécifique des séances.</param>
        /// <returns>Une liste filtrée des films selon les critères fournis.</returns>
        [HttpGet("filter")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployeeUser)]
        public async Task<IActionResult> FilterMovies([FromQuery] int? cinemaId, [FromQuery] string genre = null, [FromQuery] DateTime? date = null)
        {
            try
            {
                var movies = await _movieService.FilterMoviesAsync(cinemaId, genre, date);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors du filtrage des films : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Crée un nouveau film.
        /// Cette méthode est réservée aux administrateurs et aux employés.
        /// </summary>
        /// <param name="movieViewModel">Le ViewModel du film à créer.</param>
        /// <returns>Le film nouvellement créé.</returns>
        [HttpPost("create")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> CreateMovie([FromBody] MovieViewModel movieViewModel)
        {
            var response = await _movieService.CreateMovieAsync(movieViewModel);

            if (!response.IsSucceed)
            {
                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(response.StatusCode, response);
        }



        /// <summary>
        /// Modifie un film existant.
        /// Cette méthode est réservée aux administrateurs et aux employés.
        /// </summary>
        /// <param name="filmId">L'identifiant unique du film à modifier.</param>
        /// <param name="movieViewModel">Les nouvelles données du film à mettre à jour.</param>
        /// <returns>Le film mis à jour.</returns>
        [HttpPut("{filmId}")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> UpdateMovie(string filmId, [FromBody] MovieViewModel movieViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _movieService.UpdateMovieAsync(filmId, movieViewModel);

            if (!response.IsSucceed)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }


        /// <summary>
        /// Supprime un film existant.
        /// Cette méthode est réservée aux administrateurs et aux employés.
        /// </summary>
        /// <param name="filmId">L'identifiant unique du film à supprimer.</param>
        /// <returns>Une tâche représentant l'opération de suppression.</returns>
        [HttpDelete("{filmId}")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> DeleteMovie(string filmId)
        {
            var response = await _movieService.DeleteMovieAsync(filmId);

            if (!response.IsSucceed)
            {
                return StatusCode(response.StatusCode, response);
            }

            return NoContent();
        }



        /// <summary>
        /// Permet à un utilisateur de laisser un avis sur un film.
        /// </summary>
        /// <param name="reviewViewModel">Le ViewModel de l'avis à soumettre.</param>
        /// <returns>L'avis nouvellement créé.</returns>
        [HttpPost("reviews/submit")]
        //[Authorize(Roles = RoleConfigurations.User)]
        public async Task<IActionResult> SubmitReview([FromBody] ReviewViewModel reviewViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var review = await _movieService.SubmitReviewAsync(reviewViewModel);
                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la soumission de la critique : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Récupère tous les avis d'un film.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste d'avis associés au film.</returns>
        [HttpGet("{movieId}/reviews")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> GetReviewsByMovieId(string movieId)
        {
            try
            {
                var reviews = await _movieService.GetReviewsByMovieIdAsync(movieId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération des critiques pour le film {movieId} : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Permet à un utilisateur de laisser une note sur un film.
        /// </summary>
        /// <param name="movieRatingViewModel">Le ViewModel de la note à soumettre.</param>
        /// <returns>La note nouvellement créée.</returns>
        [HttpPost("ratings/submit")]
        //[Authorize(Roles = RoleConfigurations.User)]
        public async Task<IActionResult> SubmitMovieRating([FromBody] MovieRatingViewModel movieRatingViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var movieRating = await _movieService.SubmitMovieRatingAsync(movieRatingViewModel);
                return Ok(movieRating);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la soumission de l'évaluation : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Valide une note sur un film, action réservée aux employés.
        /// </summary>
        /// <param name="movieRatingId">L'identifiant unique de la note à valider.</param>
        /// <returns>La note validée.</returns>
        [HttpPut("ratings/validate/{movieRatingId}")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> ValidateMovieRating(int movieRatingId)
        {
            try
            {
                var validatedMovieRating = await _movieService.ValidateMovieRatingAsync(movieRatingId);
                return Ok(validatedMovieRating);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la validation de l'évaluation {movieRatingId} : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Supprime une note sur un film, action réservée aux employés.
        /// </summary>
        /// <param name="movieRatingId">L'identifiant unique de la note à supprimer.</param>
        /// <returns>Une tâche représentant l'opération de suppression.</returns>
        [HttpDelete("ratings/{movieRatingId}")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> DeleteMovieRating(int movieRatingId)
        {
            try
            {
                await _movieService.DeleteMovieRatingAsync(movieRatingId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la suppression de l'évaluation {movieRatingId} : {ex.Message}"
                });
            }
        }

    }
}
