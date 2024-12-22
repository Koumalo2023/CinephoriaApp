using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Authorization;
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
        /// Crée une nouvelle séance avec les informations fournies.
        /// </summary>
        /// <param name="model">Le modèle contenant les détails de la séance à créer.</param>
        /// <returns>Un GeneralServiceResponseData indiquant le statut de la création de la séance.</returns>
        [HttpPost("create")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployee)]
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

            try
            {
                var response = await _showtimeService.CreateShowtimeAsync(model);
                if (!response.IsSucceed)
                {
                    return StatusCode(response.StatusCode, new GeneralServiceResponseData<string>
                    {
                        IsSucceed = false,
                        StatusCode = response.StatusCode,
                        Message = response.Message
                    });
                }

                return Ok(new GeneralServiceResponseData<ShowtimeViewModel>
                {
                    IsSucceed = true,
                    StatusCode = 201,
                    //Data = response.Data,
                    Message = "Création de la séance réussie."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponseData<string>
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la création de la séance : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Récupère les séances programmées pour un film spécifique dans un cinéma donné.
        /// </summary>
        /// <param name="movieId">L'identifiant du film pour lequel récupérer les séances.</param>
        /// <param name="cinemaId">L'identifiant du cinéma où le film est diffusé.</param>
        /// <returns>Un GeneralServiceResponseData contenant une liste de séances correspondant aux critères spécifiés.</returns>
        [HttpGet("movie/{movieId}/cinema/{cinemaId}")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> GetShowtimesForMovieInCinema(string movieId, int cinemaId)
        {
            try
            {
                var result = await _showtimeService.GetShowtimesForMovieInCinemaAsync(movieId, cinemaId);
                if (!result.IsSucceed)
                    return StatusCode(result.StatusCode, new GeneralServiceResponseData<string>
                    {
                        IsSucceed = false,
                        StatusCode = result.StatusCode,
                        Message = result.Message
                    });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponseData<string>
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération des séances pour le film {movieId} dans le cinéma {cinemaId} : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Récupère les séances disponibles pour l'utilisateur authentifié.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur pour lequel récupérer les séances.</param>
        /// <returns>Un GeneralServiceResponseData contenant une liste de séances accessibles par l'utilisateur.</returns>
        [HttpGet("user/{userId}/reservations")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> GetShowtimesForAuthenticatedUser(string userId)
        {
            try
            {
                var result = await _showtimeService.GetShowtimesForAuthenticatedUserAsync(userId);
                if (!result.IsSucceed)
                    return StatusCode(result.StatusCode, new GeneralServiceResponseData<string>
                    {
                        IsSucceed = false,
                        StatusCode = result.StatusCode,
                        Message = result.Message
                    });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponseData<string>
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération des séances pour l'utilisateur {userId} : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Met à jour une séance existante avec de nouvelles informations.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance à mettre à jour.</param>
        /// <param name="model">Le modèle contenant les nouvelles informations de la séance.</param>
        /// <returns>Un GeneralServiceResponseData indiquant le statut de la mise à jour.</returns>
        [HttpPut("{showtimeId}")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> UpdateShowtime(string showtimeId, [FromBody] ShowtimeViewModel model)
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

            try
            {
                var result = await _showtimeService.UpdateShowtimeAsync(showtimeId, model);
                if (!result.IsSucceed)
                    return StatusCode(result.StatusCode, new GeneralServiceResponseData<string>
                    {
                        IsSucceed = false,
                        StatusCode = result.StatusCode,
                        Message = result.Message
                    });

                return Ok(new GeneralServiceResponseData<string>
                {
                    IsSucceed = true,
                    StatusCode = 200,
                    Message = "Séance mise à jour avec succès."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponseData<string>
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la mise à jour de la séance {showtimeId} : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Supprime une séance spécifique par son identifiant.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance à supprimer.</param>
        /// <returns>Un GeneralServiceResponseData indiquant le statut de la suppression.</returns>
        [HttpDelete("{showtimeId}")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> DeleteShowtime(string showtimeId)
        {
            try
            {
                var result = await _showtimeService.DeleteShowtimeAsync(showtimeId);
                if (!result.IsSucceed)
                    return StatusCode(result.StatusCode, new GeneralServiceResponseData<string>
                    {
                        IsSucceed = false,
                        StatusCode = result.StatusCode,
                        Message = result.Message
                    });

                return Ok(new GeneralServiceResponseData<string>
                {
                    IsSucceed = true,
                    StatusCode = 200,
                    Message = "Séance supprimée avec succès."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponseData<string>
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la suppression de la séance {showtimeId} : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Récupère les séances programmées pour l'utilisateur spécifié pour aujourd'hui et les jours futurs.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur pour lequel récupérer les séances.</param>
        /// <returns>Un GeneralServiceResponseData contenant une liste de séances pour aujourd'hui et le futur.</returns>
        [HttpGet("user/{userId}/upcoming")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> GetUserShowtimesForTodayAndFuture(string userId)
        {
            try
            {
                var result = await _showtimeService.GetUserShowtimesForTodayAndFutureAsync(userId);
                if (!result.IsSucceed)
                    return StatusCode(result.StatusCode, new GeneralServiceResponseData<string>
                    {
                        IsSucceed = false,
                        StatusCode = result.StatusCode,
                        Message = result.Message
                    });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponseData<string>
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération des séances pour l'utilisateur {userId} : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Vérifie si une séance se chevauche avec d'autres séances dans le même théâtre.
        /// </summary>
        /// <param name="theaterId">L'identifiant du théâtre où la séance est programmée.</param>
        /// <param name="startTime">L'heure de début de la séance à vérifier.</param>
        /// <param name="endTime">L'heure de fin de la séance à vérifier.</param>
        /// <param name="showtimeId">L'identifiant optionnel de la séance à exclure de la vérification.</param>
        /// <returns>Un GeneralServiceResponseData contenant un booléen indiquant si la séance se chevauche ou non.</returns>
        [HttpGet("theater/{theaterId}/overlap")]
        //[Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> CheckShowtimeOverlap(string theaterId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime, [FromQuery] string? showtimeId = null)
        {
            try
            {
                var result = await _showtimeService.IsShowtimeOverlappingAsync(theaterId, startTime, endTime, showtimeId);
                if (!result.IsSucceed)
                    return StatusCode(result.StatusCode, new GeneralServiceResponseData<string>
                    {
                        IsSucceed = false,
                        StatusCode = result.StatusCode,
                        Message = result.Message
                    });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponseData<string>
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la vérification de chevauchement des séances pour le théâtre {theaterId} : {ex.Message}"
                });
            }
        }

    }
}
