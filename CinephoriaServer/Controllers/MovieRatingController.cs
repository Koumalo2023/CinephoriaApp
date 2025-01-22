using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieRatingController : ControllerBase
    {
        private readonly IMovieRatingService _movieRatingService;

        public MovieRatingController(IMovieRatingService movieRatingService)
        {
            _movieRatingService = movieRatingService;
        }

        /// <summary>
        /// Soumet un nouvel avis sur un film.
        /// </summary>
        /// <param name="createMovieRatingDto">Les données de l'avis à soumettre.</param>
        /// <returns>Une réponse indiquant si l'avis a été soumis avec succès.</returns>
        [Authorize]
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitMovieReview([FromBody] CreateMovieRatingDto createMovieRatingDto)
        {
            try
            {
                // Récupérer l'identifiant de l'utilisateur connecté
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Message = "Utilisateur non authentifié." });
                }

                // Appeler la méthode SubmitMovieReviewAsync avec l'identifiant de l'utilisateur connecté
                await _movieRatingService.SubmitMovieReviewAsync(createMovieRatingDto, userId);

                return StatusCode(StatusCodes.Status201Created, new { Message = "Avis soumis avec succès." });
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
        /// Valide un avis sur un film (réservé aux administrateurs).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à valider.</param>
        /// <returns>Une réponse indiquant si l'avis a été validé avec succès.</returns>
        [Authorize(Roles = "Admin, Employee")]
        [HttpPut("validate/{reviewId}")]
        public async Task<IActionResult> ValidateReview(int reviewId)
        {
            try
            {
                await _movieRatingService.ValidateReviewAsync(reviewId);
                return Ok(new { Message = "Avis validé avec succès." });
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
