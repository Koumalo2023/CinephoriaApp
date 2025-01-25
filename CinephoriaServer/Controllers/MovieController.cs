using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MovieController> _logger;
        private readonly IImageService _imageService;

        public MovieController(IMovieService movieService, ILogger<MovieController> logger, IImageService imageService)
        {
            _movieService = movieService;
            _logger = logger;
            _imageService = imageService;
        }

        /// <summary>
        /// Récupère la liste des derniers films ajoutés.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentMovies()
        {
            try
            {
                var movies = await _movieService.GetRecentMoviesAsync();
                return Ok(movies);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération des derniers films.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }


        /// <summary>
        /// Récupère la liste de tous les films.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllMovies()
        {
            try
            {
                var movies = await _movieService.GetAllMoviesAsync();
                return Ok(movies);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération de tous les films.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }


        /// <summary>
        /// Récupère les détails d'un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Un objet Movie contenant les détails du film.</returns>
        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetMovieDetails(int movieId)
        {
            try
            {
                var movieDetails = await _movieService.GetMovieDetailsAsync(movieId);
                return Ok(movieDetails);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération des détails du film.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances.</returns>
        [HttpGet("{movieId}/sessions")]
        public async Task<IActionResult> GetMovieSessions(int movieId)
        {
            try
            {
                var sessions = await _movieService.GetMovieSessionsAsync(movieId);
                return Ok(sessions);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la récupération des séances du film.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Soumet un avis sur un film de la part d'un utilisateur.
        /// </summary>
        /// <param name="reviewDto">Les données de l'avis.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        [Authorize(Roles = "Admin, Employee, User")]
        [HttpPost("review")]
        public async Task<IActionResult> SubmitMovieReview([FromBody] MovieReviewDto reviewDto)
        {
            try
            {
                var result = await _movieService.SubmitMovieReviewAsync(reviewDto);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la soumission de l'avis.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Filtre les films en fonction du cinéma, du genre et de la date.
        /// </summary>
        /// <param name="filterDto">Les critères de filtrage.</param>
        /// <returns>Une liste de films correspondant aux critères.</returns>
        [HttpPost("filter")]
        public async Task<IActionResult> FilterMovies([FromBody] FilterMoviesRequestDto filterDto)
        {
            try
            {
                var movies = await _movieService.FilterMoviesAsync(filterDto.CinemaId, filterDto.Genre, filterDto.Date);
                return Ok(movies);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors du filtrage des films.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Crée un nouveau film.
        /// </summary>
        /// <param name="createMovieDto">Les données du film à créer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        [Authorize(Roles = "Admin, Employee")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateMovie([FromBody] CreateMovieDto createMovieDto)
        {
            try
            {
                var result = await _movieService.CreateMovieAsync(createMovieDto);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la création du film.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }


        /// <summary>
        /// Ajouter une image à un film.
        /// </summary>
        /// <param name="createMovieDto">image du film à créer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        [Authorize(Roles = "Admin, Employee")]
        [HttpPost("upload-movie-poster/{movieId}")]
        public async Task<IActionResult> UploadMoviePoster(int movieId, [FromForm] IFormFile file)
        {
            try
            {
                string folder = "movies";
                var imageUrl = await _imageService.UploadImageAsync(file, folder);
                if (imageUrl == null)
                {
                    throw new ApiException("Erreur lors du téléchargement de l'image.", StatusCodes.Status400BadRequest);
                }

                var result = await _movieService.AddPosterToMovieAsync(movieId, imageUrl);
                return Ok(new { Message = result, Url = imageUrl });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors du téléchargement de l'affiche du film.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }


        /// <summary>
        /// Supprime l'image d'un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film à supprimer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        [Authorize(Roles = "Admin, Employee")]
        [HttpDelete("delete-movie-poster/{movieId}")]
        public async Task<IActionResult> DeleteMoviePoster(int movieId, [FromQuery] string posterUrl)
        {
            try
            {
                var result = await _movieService.RemovePosterFromMovieAsync(movieId, posterUrl);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la suppression de l'affiche du film.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Met à jour les informations d'un film existant.
        /// </summary>
        /// <param name="updateMovieDto">Les nouvelles données du film.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        [Authorize(Roles = "Admin, Employee")]
        [HttpPut]
        public async Task<IActionResult> UpdateMovie([FromBody] UpdateMovieDto updateMovieDto)
        {
            try
            {
                var result = await _movieService.UpdateMovieAsync(updateMovieDto);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la mise à jour du film.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }

        /// <summary>
        /// Supprime un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film à supprimer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        [Authorize(Roles = "Admin, Employee")]
        [HttpDelete("movie/{movieId}")]
        public async Task<IActionResult> DeleteMovie(int movieId)
        {
            try
            {
                var result = await _movieService.DeleteMovieAsync(movieId);
                return Ok(new { Message = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la suppression du film.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite." });
            }
        }
    }
}
