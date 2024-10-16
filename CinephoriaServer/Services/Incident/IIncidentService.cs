using CinephoriaServer.Models.MongooDb;

namespace CinephoriaServer.Services
{
    public interface IIncidentService
    {
        Task<IncidentDto> CreateIncidentAsync(IncidentDto incidentDto);
        Task<IncidentDto> GetIncidentByIdAsync(string id);
        Task<List<IncidentDto>> GetAllIncidentsAsync();
        // Suppression d'un incident
        Task<bool> DeleteIncidentAsync(string id);

        // Mise à jour d'un incident existant
        Task<IncidentDto> UpdateIncidentAsync(string id, IncidentDto incidentDto);

        // Filtrer les incidents par employé, date, salle ou cinéma
        Task<List<IncidentDto>> FilterIncidentsAsync(string employeeId, DateTime? startDate, DateTime? endDate, string theaterId, string cinemaId);
    }
}
