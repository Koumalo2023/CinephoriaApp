using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentController : ControllerBase
    {
        private readonly IIncidentService _incidentService;

        public IncidentController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        /// <summary>
        /// Crée un nouvel incident signalé par un employé.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateIncident([FromBody] IncidentDto incidentDto)
        {
            var incident = await _incidentService.CreateIncidentAsync(incidentDto);
            if (incident == null)
                return BadRequest("Incident non créé.");

            return Ok(incident);
        }

        /// <summary>
        /// Récupère un incident spécifique par son identifiant.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncidentById(string id)
        {
            var incident = await _incidentService.GetIncidentByIdAsync(id);
            if (incident == null)
                return NotFound("Incident non trouvé.");

            return Ok(incident);
        }

        /// <summary>
        /// Récupère la liste de tous les incidents signalés par les employés.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllIncidents()
        {
            var incidents = await _incidentService.GetAllIncidentsAsync();
            return Ok(incidents);
        }

        /// <summary>
        /// Suppression d'un incident par son identifiant.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncident(string id)
        {
            var result = await _incidentService.DeleteIncidentAsync(id);
            if (!result)
                return NotFound(new { message = "Incident non trouvé" });

            return NoContent();
        }

        /// <summary>
        /// Mise à jour d'un incident existant.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIncident(string id, [FromBody] IncidentDto incidentDto)
        {
            try
            {
                var updatedIncident = await _incidentService.UpdateIncidentAsync(id, incidentDto);
                return Ok(updatedIncident);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Filtrage des incidents.
        /// </summary>
        [HttpPost("filter")]
        public async Task<IActionResult> FilterIncidents([FromBody] IncidentFilterDto filterDto)
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
    }
}
