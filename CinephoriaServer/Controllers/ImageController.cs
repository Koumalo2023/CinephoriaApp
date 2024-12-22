using CinephoriaServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IMovieService _movieService;

        public ImageController(IImageService imageService, IMovieService movieService)
        {
            _imageService = imageService;
            _movieService = movieService;
        }

        
        // Upload image de profil utilisateur
        [HttpPost("upload-user-profile/{userId}")]
        public async Task<IActionResult> UploadUserProfile(string userId, [FromForm, Required] IFormFile file)
        {
            string folder = "users";
            var imageUrl = await _imageService.UploadImageAsync(file, folder);
            if (imageUrl == null) return BadRequest("Erreur lors du téléchargement de l'image.");

            return Ok(new { Url = imageUrl });
        }

        // Upload image d'un incident
        [HttpPost("upload-incident-image/{incidentId}")]
        public async Task<IActionResult> UploadIncidentImage(string incidentId, [FromForm] IFormFile file)
        {
            string folder = "incidents";
            var imageUrl = await _imageService.UploadImageAsync(file, folder);
            if (imageUrl == null) return BadRequest("Erreur lors du téléchargement de l'image.");

            // Mettre à jour la collection ImageUrls de l'incident pour ajouter `imageUrl`
            return Ok(new { Url = imageUrl });
        }

        // Upload affiche de film
        [HttpPost("upload-movie-poster/{movieId}")]
        public async Task<IActionResult> UploadMoviePoster(string movieId, [FromForm] IFormFile file)
        {
            try
            {
                string folder = "movies";
                var imageUrl = await _imageService.UploadImageAsync(file, folder);
                if (imageUrl == null) return BadRequest("Erreur lors du téléchargement de l'image.");

                // Mettre à jour le film en ajoutant l'URL de l'image
                var updated = await _movieService.AddPosterToMovieAsync(movieId, imageUrl);
                if (!updated)
                {
                    return NotFound("Film introuvable.");
                }

                return Ok(new { Url = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'ajout de l'affiche : {ex.Message}");
            }
        }


        // Suppression d'une image (exemple générique)
        [HttpDelete("delete-image")]
        public async Task<IActionResult> DeleteImage([FromQuery] string imageUrl)
        {
            var result = await _imageService.DeleteImageAsync(imageUrl);
            if (!result) return NotFound("Image non trouvée.");

            // Suppression de l'URL de l'image de la base de données si nécessaire
            return Ok("Image supprimée avec succès.");
        }
    }
}
