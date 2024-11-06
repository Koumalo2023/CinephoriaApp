using CinephoriaServer.Models.MongooDb;

namespace CinephoriaServer.Services
{
    public interface IIncidentService
    {
        /// <summary>
        /// Crée un nouvel incident avec les informations fournies.
        /// </summary>
        /// <param name="incidentDto">Les détails de l'incident à créer.</param>
        /// <returns>L'incident créé sous forme d'IncidentDto.</returns>
        Task<IncidentDto> CreateIncidentAsync(IncidentDto incidentDto);

        /// <summary>
        /// Récupère un incident spécifique par son identifiant unique.
        /// </summary>
        /// <param name="id">L'identifiant de l'incident à récupérer.</param>
        /// <returns>Un IncidentDto contenant les informations de l'incident, ou null s'il n'est pas trouvé.</returns>
        Task<IncidentDto> GetIncidentByIdAsync(string id);

        /// <summary>
        /// Récupère la liste de tous les incidents signalés.
        /// </summary>
        /// <returns>Une liste d'IncidentDto contenant les détails de chaque incident.</returns>
        Task<List<IncidentDto>> GetAllIncidentsAsync();

        /// <summary>
        /// Supprime un incident spécifique par son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant de l'incident à supprimer.</param>
        /// <returns>Un booléen indiquant si la suppression a réussi.</returns>
        Task<bool> DeleteIncidentAsync(string id);

        /// <summary>
        /// Met à jour un incident existant avec de nouvelles informations.
        /// </summary>
        /// <param name="id">L'identifiant de l'incident à mettre à jour.</param>
        /// <param name="incidentDto">Les nouvelles informations de l'incident.</param>
        /// <returns>L'incident mis à jour sous forme d'IncidentDto.</returns>
        Task<IncidentDto> UpdateIncidentAsync(string id, IncidentDto incidentDto);

        /// <summary>
        /// Filtre les incidents en fonction de différents critères comme l'employé, les dates et le lieu.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé associé aux incidents.</param>
        /// <param name="startDate">La date de début pour filtrer les incidents.</param>
        /// <param name="endDate">La date de fin pour filtrer les incidents.</param>
        /// <param name="theaterId">L'identifiant du théâtre lié aux incidents.</param>
        /// <param name="cinemaId">L'identifiant du cinéma lié aux incidents.</param>
        /// <returns>Une liste d'IncidentDto contenant les incidents correspondant aux critères.</returns>
        Task<List<IncidentDto>> FilterIncidentsAsync(string employeeId, DateTime? startDate, DateTime? endDate, string theaterId, string cinemaId);

    }
}
