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
        /// Crée un nouveau cinéma avec les informations fournies.
        /// </summary>
        /// <param name="cinemaViewModel">Les détails du cinéma à créer.</param>
        /// <returns>Un objet avec le résultat de la création du cinéma.</returns>
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
            return Ok(response);
        }

        /// <summary>
        /// Met à jour un cinéma existant avec de nouvelles informations.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma à mettre à jour.</param>
        /// <param name="cinemaViewModel">Les nouvelles informations du cinéma.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de la mise à jour.</returns>
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
        /// Supprime un cinéma spécifique par son identifiant.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma à supprimer.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de la suppression.</returns>
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
        /// Récupère la liste de tous les cinémas.
        /// </summary>
        /// <returns>Une liste de CinemaDto contenant les informations de chaque cinéma.</returns>
        [HttpGet]
        [Route("all")]
        //[Authorize(Roles = RoleConfigurations.Admin)]
        public async Task<IActionResult> GetAllCinemas()
        {
            try
            {
                var cinemas = await _cinemaService.GetAllCinemasAsync();
                return Ok(cinemas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération de la liste des cinémas : {ex.Message}"
                });
            }
        }



        /// <summary>
        /// Crée une nouvelle salle pour un cinéma spécifique.
        /// </summary>
        /// <param name="theaterViewModel">Les informations de la salle à créer.</param>
        /// <returns>Un GeneralServiceResponseData contenant le résultat de la création de la salle.</returns>
        [HttpGet("theater/{id}")]
        public async Task<IActionResult> GetTheaterById(string id)
        {
            try
            {
                var response = await _cinemaService.GetTheaterByIdAsync(id);
                if (!response.IsSucceed)
                {
                    return StatusCode(response.StatusCode, new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = response.StatusCode,
                        Message = response.Message
                    });
                }

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération du théâtre : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Crée une nouvelle salle pour un cinéma spécifique.
        /// </summary>
        /// <param name="theaterViewModel">Les informations de la salle à créer.</param>
        /// <returns>Un objet avec le résultat de la création de la salle.</returns>
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
            return Ok(response);
        }

        /// <summary>
        /// Met à jour une salle existante avec de nouvelles informations.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle à mettre à jour.</param>
        /// <param name="theaterViewModel">Les nouvelles informations de la salle.</param>
        /// <returns>Un message indiquant le succès ou l'échec de la mise à jour.</returns>
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
        /// Supprime une salle spécifique par son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle à supprimer.</param>
        /// <returns>Un message indiquant le succès ou l'échec de la suppression.</returns>
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
        /// Récupère les détails d'une salle spécifique par son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle à récupérer.</param>
        /// <returns>Un GeneralServiceResponseData contenant les informations de la salle.</returns>
        [HttpGet]
        [Route("{cinemaId}/theaters")]
        public async Task<IActionResult> GetTheatersByCinema(int cinemaId)
        {
            try
            {
                var theaters = await _cinemaService.GetTheatersByCinemaAsync(cinemaId);
                return Ok(theaters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération des théâtres pour le cinéma : {ex.Message}"
                });
            }
        }

    }

}
