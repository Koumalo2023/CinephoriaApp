using AutoMapper;
using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Services
{
    public class IncidentService : IIncidentService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWorkMongoDb _unitOfWork;
        private readonly IMapper _mapper;

        public IncidentService(UserManager<AppUser> userManager, IUnitOfWorkMongoDb unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Crée un nouvel incident signalé par un employé.
        /// </summary>
        /// <param name="incidentDto">Les détails de l'incident à créer.</param>
        /// <returns>Le DTO de l'incident créé.</returns>
        /// <exception cref="Exception">Si l'employé n'est pas trouvé.</exception>
        public async Task<IncidentDto> CreateIncidentAsync(IncidentDto incidentDto)
        {
            // Vérification du format de l'ID de l'employé dans ReportedBy
            if (!ObjectId.TryParse(incidentDto.ReportedBy, out ObjectId employeeObjectId))
            {
                throw new FormatException("L'ID de l'employé (ReportedBy) n'est pas un ObjectId MongoDB valide.");
            }

            // Création d'un nouvel incident avec un nouvel ObjectId ou celui fourni
            var incident = new Incident
            {
                Id = ObjectId.GenerateNewId(),
                TheaterId = incidentDto.TheaterId,
                Description = incidentDto.Description,
                ReportedBy = incidentDto.ReportedBy,
                ReportedAt = DateTime.Now,
                Status = IncidentStatus.Pending,
                ImageUrls = incidentDto.ImageUrls
            };

            var employeeExists = await _userManager.FindByNameAsync(incidentDto.ReportedBy);
            if (employeeExists != null)
            {
                throw new Exception("L'employé n'existe pas.");
            }

            // Ajout de l'incident dans la base de données
            await _unitOfWork.Incidents.AddAsync(incident);
            await _unitOfWork.SaveChangesAsync();

            // Construction manuelle du DTO à retourner
            var result = new IncidentDto
            {
                Id = incident.Id.ToString(),
                TheaterId = incident.TheaterId,
                Description = incident.Description,
                ReportedBy = incident.ReportedBy,
                ReportedAt = incident.ReportedAt,
                Status = incident.Status,
                ResolvedAt = incident.ResolvedAt
            };

            return result;
        }



        /// <summary>
        /// Récupère un incident spécifique par son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant de l'incident.</param>
        /// <returns>Le DTO de l'incident, ou null si non trouvé.</returns>
        public async Task<IncidentDto> GetIncidentByIdAsync(string id)
        {
            var incident = await _unitOfWork.Incidents.GetByIdAsync(id);
            if (incident == null)
                return null;

            // Retourner l'incident en tant que DTO
            return _mapper.Map<IncidentDto>(incident);
        }

        /// <summary>
        /// Récupère la liste de tous les incidents signalés.
        /// </summary>
        /// <returns>La liste des DTO des incidents.</returns>
        public async Task<List<IncidentDto>> GetAllIncidentsAsync()
        {
            var incidents = await _unitOfWork.Incidents.GetAllAsync();

            // Mapper tous les incidents en DTO et retourner la liste
            return _mapper.Map<List<IncidentDto>>(incidents);
        }

        /// <summary>
        /// Suppression d'un incident par son identifiant.
        /// </summary>
        public async Task<bool> DeleteIncidentAsync(string id)
        {
            var result = await _unitOfWork.Incidents.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        /// <summary>
        /// Mise à jour d'un incident existant.
        /// </summary>
        public async Task<IncidentDto> UpdateIncidentAsync(string id, IncidentDto incidentDto)
        {
            var existingIncident = await _unitOfWork.Incidents.GetByIdAsync(id);
            if (existingIncident == null)
            {
                throw new Exception("Incident non trouvé.");
            }

            // Mettre à jour les champs nécessaires
            existingIncident.Description = incidentDto.Description;
            existingIncident.Status = incidentDto.Status;
            existingIncident.ResolvedAt = incidentDto.ResolvedAt;
            // Mise à jour des URLs d'images si nécessaire
            existingIncident.ImageUrls = incidentDto.ImageUrls;

            // Mettre à jour l'incident dans la base de données
            await _unitOfWork.Incidents.UpdateAsync(existingIncident);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<IncidentDto>(existingIncident);
        }

        /// <summary>
        /// Filtrer les incidents par employé, date, salle ou cinéma.
        /// </summary>
        public async Task<List<IncidentDto>> FilterIncidentsAsync(string employeeId, DateTime? startDate, DateTime? endDate, string theaterId, string cinemaId)
        {
            var filterBuilder = Builders<Incident>.Filter;
            var filters = new List<FilterDefinition<Incident>>();

            // Filtrer par employé (ReportedBy sera un string ou ObjectId)
            if (!string.IsNullOrEmpty(employeeId))
            {
                filters.Add(filterBuilder.Eq(i => i.ReportedBy, employeeId));
            }

            // Filtrer par date de début et de fin
            if (startDate.HasValue)
            {
                filters.Add(filterBuilder.Gte(i => i.ReportedAt, startDate.Value));
            }
            if (endDate.HasValue)
            {
                filters.Add(filterBuilder.Lte(i => i.ReportedAt, endDate.Value));
            }

            // Filtrer par salle
            if (!string.IsNullOrEmpty(theaterId))
            {
                filters.Add(filterBuilder.Eq(i => i.TheaterId, theaterId));
            }

            //// Ajoutez la logique pour filtrer par cinéma si applicable (si vous avez un champ CinemaId)
            //if (!string.IsNullOrEmpty(cinemaId))
            //{
            //    filters.Add(filterBuilder.Eq(i => i.CinemaId, cinemaId));
            //}

            var filter = filterBuilder.And(filters);

            var incidents = await _unitOfWork.Incidents.FilterAsync(filter);
            return _mapper.Map<List<IncidentDto>>(incidents);
        }
        
    }


}
