using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CinemaController : ControllerBase
    {
        private readonly ICinemaService _cinemaService;

        public CinemaController(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        /// <summary>
        /// Crée un nouveau cinéma dans la base de données.
        /// </summary>
        /// <param name="cinemaDto">Les informations du cinéma à créer.</param>
        /// <returns>Réponse HTTP avec le statut de la création.</returns>
        [HttpPost]
        [Route("create")]
        //[Authorize(Roles = RoleConfigurations.Admin)]
        public async Task<IActionResult> CreateCinema([FromBody] CinemaViewModel cinemaViewModel)
        {
            var response = await _cinemaService.CreateCinemaAsync(cinemaViewModel);
            if (!response.IsSucceed)
            {
                return StatusCode(response.StatusCode, response.Message);
            }

            return Ok(response.Data);
        }

        /// <summary>
        /// Modifie un cinéma existant dans la base de données.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma à modifier.</param>
        /// <param name="cinemaDto">Les nouvelles informations du cinéma.</param>
        /// <returns>Réponse HTTP avec le statut de la mise à jour.</returns>
        [HttpPut]
        [Route("update/{cinemaId}")]
        //[Authorize(Roles = RoleConfigurations.Admin)]
        public async Task<IActionResult> UpdateCinema(int cinemaId, [FromBody] CinemaViewModel cinemaViewModel)
        {
            var response = await _cinemaService.UpdateCinemaAsync(cinemaId, cinemaViewModel);
            if (!response.IsSucceed)
            {
                return StatusCode(response.StatusCode, response.Message);
            }

            return Ok(response.Message);
        }

        /// <summary>
        /// Supprime un cinéma de la base de données.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma à supprimer.</param>
        /// <returns>Réponse HTTP avec le statut de la suppression.</returns>
        [HttpDelete]
        [Route("delete/{cinemaId}")]
        //[Authorize(Roles = RoleConfigurations.Admin)]
        public async Task<IActionResult> DeleteCinema(int cinemaId)
        {
            var response = await _cinemaService.DeleteCinemaAsync(cinemaId);
            if (!response.IsSucceed)
            {
                return StatusCode(response.StatusCode, response.Message);
            }

            return Ok(response.Message);
        }

        /// <summary>
        /// Récupère les informations de tous les cinémas (nom, adresse, téléphone, etc.).
        /// </summary>
        /// <returns>Liste des cinémas.</returns>
        [HttpGet]
        [Route("all")]
        //[Authorize(Roles = RoleConfigurations.Admin)]
        public async Task<IActionResult> GetAllCinemas()
        {
            var cinemas = await _cinemaService.GetAllCinemasAsync();
            return Ok(cinemas);
        }

        [HttpGet("theater/{id}")]
        public async Task<IActionResult> GetTheaterById(string id)
        {
            var response = await _cinemaService.GetTheaterByIdAsync(id);
            if (!response.IsSucceed)
            {
                return StatusCode(response.StatusCode, response.Message);
            }

            return Ok(response.Data);
        }


        /// <summary>
        /// Crée une nouvelle salle de projection pour un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma auquel ajouter la salle.</param>
        /// <param name="theaterDto">Les informations de la salle de projection à créer.</param>
        /// <returns>Réponse HTTP avec le statut de la création de la salle.</returns>
        [HttpPost]
        [Route("{cinemaId}/theater/create")]
        //[Authorize(Roles = RoleConfigurations.Admin)]
        public async Task<IActionResult> CreateTheater(int cinemaId, [FromBody] TheaterViewModel theaterViewModel)
        {
            var response = await _cinemaService.CreateTheaterForCinemaAsync(theaterViewModel);
            if (!response.IsSucceed)
            {
                return StatusCode(response.StatusCode, response.Message);
            }

            return Ok(response.Data);
        }

        /// <summary>
        /// Modifie une salle de projection existante.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle à modifier.</param>
        /// <param name="theaterDto">Les nouvelles informations de la salle.</param>
        /// <returns>Réponse HTTP avec le statut de la mise à jour de la salle.</returns>
        [HttpPut]
        [Route("theater/update/{theaterId}")]
        //[Authorize(Roles = RoleConfigurations.Admin)]
        public async Task<IActionResult> UpdateTheater(string theaterId, [FromBody] TheaterViewModel theaterViewModel)
        {
            var response = await _cinemaService.UpdateTheaterAsync(theaterId, theaterViewModel);
            if (!response.IsSucceed)
            {
                return StatusCode(response.StatusCode, response.Message);
            }

            return Ok(response.Message);
        }

        /// <summary>
        /// Supprime une salle de projection existante.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle à supprimer.</param>
        /// <returns>Réponse HTTP avec le statut de la suppression de la salle.</returns>
        [HttpDelete]
        [Route("theater/delete/{theaterId}")]
        //[Authorize(Roles = RoleConfigurations.Admin)]
        public async Task<IActionResult> DeleteTheater(string theaterId)
        {
            var response = await _cinemaService.DeleteTheaterAsync(theaterId);
            if (!response.IsSucceed)
            {
                return StatusCode(response.StatusCode, response.Message);
            }

            return Ok(response.Message);
        }

        /// <summary>
        /// Récupère toutes les salles d'un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Liste des salles du cinéma.</returns>
        [HttpGet]
        [Route("{cinemaId}/theaters")]
        public async Task<IActionResult> GetTheatersByCinema(int cinemaId)
        {
            var theaters = await _cinemaService.GetTheatersByCinemaAsync(cinemaId);
            return Ok(theaters);
        }
    }

}
