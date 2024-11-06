using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentController : ControllerBase
    {
        private readonly IIncidentService _incidentService;
        private readonly IImageService _imageService;

        public IncidentController(IIncidentService incidentService, IImageService imageService)
        {
            _incidentService = incidentService;
            _imageService = imageService;
        }

        /// <summary>
        /// Crée un nouvel incident avec les informations fournies.
        /// </summary>
        /// <param name="incidentDto">Les détails de l'incident à créer.</param>
        /// <returns>L'incident créé sous forme d'IncidentDto.</returns>
        [HttpPost("created-incident")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> CreateIncident([FromBody] IncidentDto incidentDto, [FromForm] IFormFileCollection uploadedFiles)
        {
            try
            {
                string folder = "incidents";

                // Liste pour stocker les URLs des images téléchargées
                var imageUrls = new List<string>();

                // Télécharger chaque fichier et ajouter l'URL dans la liste
                foreach (var uploadedFile in uploadedFiles)
                {
                    var imageUrl = await _imageService.UploadImageAsync(uploadedFile, folder);
                    if (imageUrl != null)
                    {
                        imageUrls.Add(imageUrl);
                    }
                }

                // Assigner les URLs des images dans IncidentDto
                incidentDto.ImageUrls = imageUrls;

                var incident = await _incidentService.CreateIncidentAsync(incidentDto);
                if (incident == null)
                {
                    return BadRequest(new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 400,
                        Message = "Incident non créé."
                    });
                }

                return Ok(incident);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la création de l'incident : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Récupère un incident spécifique par son identifiant unique, incluant les URLs d'images.
        /// </summary>
        /// <param name="id">L'identifiant de l'incident à récupérer.</param>
        /// <returns>Un IncidentDto contenant les informations de l'incident, y compris les URLs d'images.</returns>
        [HttpGet("incidents/{id}")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> GetIncidentById(string id)
        {
            try
            {
                var incident = await _incidentService.GetIncidentByIdAsync(id);
                if (incident == null)
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "Incident non trouvé."
                    });
                }


                return Ok(incident);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération de l'incident : {ex.Message}"
                });
            }
        }



        /// <summary>
        /// Récupère la liste de tous les incidents signalés.
        /// </summary>
        /// <returns>Une liste d'IncidentDto contenant les détails de chaque incident.</returns>
        [HttpGet("incidents")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> GetAllIncidents()
        {
            try
            {
                var incidents = await _incidentService.GetAllIncidentsAsync();
                return Ok(incidents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la récupération de la liste des incidents : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Supprime un incident spécifique par son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant de l'incident à supprimer.</param>
        /// <returns>Un booléen indiquant si la suppression a réussi.</returns>
        [HttpDelete("incidents/{id}")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> DeleteIncident(string id)
        {
            try
            {
                var result = await _incidentService.DeleteIncidentAsync(id);
                if (!result)
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "Incident non trouvé"
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la suppression de l'incident : {ex.Message}"
                });
            }
        }


        /// <summary>
        /// Met à jour un incident existant avec de nouvelles informations, y compris les URLs d'images.
        /// </summary>
        /// <param name="id">L'identifiant de l'incident à mettre à jour.</param>
        /// <param name="incidentDto">Les nouvelles informations de l'incident, incluant les mises à jour d'ImageUrls.</param>
        /// <returns>L'incident mis à jour sous forme d'IncidentDto.</returns>
        [HttpPut("incidents/{id}")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> UpdateIncident(string id, [FromBody] IncidentDto incidentDto)
        {
            try
            {
                var updatedIncident = await _incidentService.UpdateIncidentAsync(id, incidentDto);
                if (updatedIncident == null)
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "Incident non trouvé."
                    });
                }

                return Ok(updatedIncident);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la mise à jour de l'incident : {ex.Message}"
                });
            }
        }



        /// <summary>
        /// Filtre les incidents en fonction de différents critères comme l'employé, les dates et le lieu.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé associé aux incidents.</param>
        /// <param name="startDate">La date de début pour filtrer les incidents.</param>
        /// <param name="endDate">La date de fin pour filtrer les incidents.</param>
        /// <param name="theaterId">L'identifiant du théâtre lié aux incidents.</param>
        /// <param name="cinemaId">L'identifiant du cinéma lié aux incidents.</param>
        /// <returns>Une liste d'IncidentDto contenant les incidents correspondant aux critères.</returns>
        [HttpPost("filter")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> FilterIncidents([FromBody] IncidentFilterDto filterDto)
        {
            try
            {
                var incidents = await _incidentService.FilterIncidentsAsync(
                    filterDto.EmployeeId,
                    filterDto.StartDate,
                    filterDto.EndDate,
                    filterDto.TheaterId,
                    filterDto.CinemaId
                );

                return Ok(incidents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors du filtrage des incidents : {ex.Message}"
                });
            }
        }

    }
}
