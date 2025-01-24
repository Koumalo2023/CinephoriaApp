using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheaterController : ControllerBase
    {
        private readonly ITheaterService _theaterService;

        public TheaterController(ITheaterService theaterService)
        {
            _theaterService = theaterService;
        }

        /// <summary>
        /// Récupère la liste des salles de cinéma associées à un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste de salles sous forme de DTO.</returns>
        [HttpGet("by-cinema/{cinemaId}")]
        public async Task<IActionResult> GetTheatersByCinema(int cinemaId)
        {
            try
            {
                var result = await _theaterService.GetTheatersByCinemaAsync(cinemaId);
                return Ok(new { Message = "Liste des salles de cinéma récupérée avec succès.", Data = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la récupération des salles de cinéma." });
            }
        }

        /// <summary>
        /// Récupère une salle de cinéma par son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle.</param>
        /// <returns>La salle correspondante sous forme de DTO.</returns>
        [HttpGet("{theaterId}")]
        public async Task<IActionResult> GetTheaterById(int theaterId)
        {
            try
            {
                var result = await _theaterService.GetTheaterByIdAsync(theaterId);
                return Ok(new { Message = "Salle de cinéma récupérée avec succès.", Data = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la récupération de la salle de cinéma." });
            }
        }

        /// <summary>
        /// Crée une nouvelle salle de cinéma NB: Le nom de salle doit contenir un suffixe ex "Salle A" ou "Salle B".
        /// </summary>
        /// <param name="createTheaterDto">Les données de la salle à créer.</param>
        /// <returns>La salle créée sous forme de DTO.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateTheater([FromBody] CreateTheaterDto createTheaterDto)
        {
            try
            {
                var result = await _theaterService.CreateTheaterAsync(createTheaterDto);
                return StatusCode(StatusCodes.Status201Created, new { Message = "Salle de cinéma créée avec succès.", Data = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la création de la salle de cinéma." });
            }
        }

        /// <summary>
        /// Met à jour les informations d'une salle de cinéma existante.
        /// </summary>
        /// <param name="updateTheaterDto">Les données de la salle à mettre à jour.</param>
        /// <returns>La salle mise à jour sous forme de DTO.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateTheater([FromBody] UpdateTheaterDto updateTheaterDto)
        {
            try
            {
                var result = await _theaterService.UpdateTheaterAsync(updateTheaterDto);
                return Ok(new { Message = "Salle de cinéma mise à jour avec succès.", Data = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la mise à jour de la salle de cinéma." });
            }
        }

        /// <summary>
        /// Supprime une salle de cinéma en fonction de son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle à supprimer.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        [HttpDelete("{theaterId}")]
        public async Task<IActionResult> DeleteTheater(int theaterId)
        {
            try
            {
                var result = await _theaterService.DeleteTheaterAsync(theaterId);
                if (result)
                {
                    return Ok(new { Message = "Salle de cinéma supprimée avec succès." });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "La suppression de la salle de cinéma a échoué." });
                }
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la suppression de la salle de cinéma." });
            }
        }

        /// <summary>
        /// Récupère la liste des incidents associés à une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <returns>Une liste d'incidents sous forme de DTO.</returns>
        [HttpGet("{theaterId}/incidents")]
        public async Task<IActionResult> GetTheaterIncidents(int theaterId)
        {
            try
            {
                var result = await _theaterService.GetTheaterIncidentsAsync(theaterId);
                return Ok(new { Message = "Liste des incidents de la salle de cinéma récupérée avec succès.", Data = result });
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la récupération des incidents de la salle de cinéma." });
            }
        }
    }
}
