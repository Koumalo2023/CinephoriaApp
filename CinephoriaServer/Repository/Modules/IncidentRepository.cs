using CinephoriaServer.Models.MongooDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CinephoriaServer.Repository
{
    public interface IIncidentRepository
    {
        /// <summary>
        /// Crée un nouvel incident signalé par un employé.
        /// </summary>
        Task CreateIncidentAsync(Incident incident);


        /// <summary>
        /// Récupère un incident spécifique par son identifiant.
        /// </summary>
        Task<Incident> GetIncidentByIdAsync(ObjectId incidentId);

        /// <summary>
        /// Filtre les incidents par employé, date, salle ou cinéma.
        /// </summary>
        //Task<List<Incident>> FilterIncidentsAsync(EmployeeAccount? reportedBy = null, string? theaterId = null, DateTime? reportedAt = null, string? cinemaId = null);

        /// <summary>
        /// Récupère la liste de tous les incidents signalés par les employés.
        /// </summary>
        Task<List<Incident>> GetAllIncidentsAsync();

        /// <summary>
        /// Met à jour un incident signalé par un employé.
        /// </summary>
        Task UpdateIncidentAsync(ObjectId incidentId, Incident updatedIncident);

        /// <summary>
        /// Supprime un incident signalé par un employé.
        /// </summary>
        Task DeleteIncidentAsync(ObjectId incidentId);
    }


    public class IncidentRepository : IIncidentRepository
    {
        private readonly IMongoCollection<Incident> _Incidents;

        public IncidentRepository(IMongoDatabase database)
        {
            _Incidents = database.GetCollection<Incident>("Incidents");
        }

        public async Task CreateIncidentAsync(Incident incident)
        {
            await _Incidents.InsertOneAsync(incident);
        }
        public async Task<List<Incident>> GetAllIncidentsAsync()
        {
            return await _Incidents.Find(_ => true).ToListAsync();
        }

        public async Task<Incident> GetIncidentByIdAsync(ObjectId incidentId)
        {
            var filter = Builders<Incident>.Filter.Eq(i => i.Id, incidentId);
            return await _Incidents.Find(filter).FirstOrDefaultAsync();
        }

        //public async Task<List<Incident>> FilterIncidentsAsync(EmployeeAccount? reportedBy = null, string? theaterId = null, DateTime? reportedAt = null, string? cinemaId = null)
        //{
        //    var filters = new List<FilterDefinition<Incident>>();

        //    // Vérifier si reportedBy est non nul et ensuite utiliser son ID
        //    if (reportedBy != null)
        //    {
        //        filters.Add(Builders<Incident>.Filter.Eq(i => i.ReportedBy.Id, reportedBy.Id));
        //    }

        //    if (!string.IsNullOrEmpty(theaterId))
        //    {
        //        filters.Add(Builders<Incident>.Filter.Eq(i => i.TheaterId, theaterId));
        //    }

        //    if (reportedAt.HasValue)
        //    {
        //        filters.Add(Builders<Incident>.Filter.Gte(i => i.ReportedAt, reportedAt.Value));
        //    }

        //    if (!string.IsNullOrEmpty(cinemaId))
        //    {
        //        // Suppose that TheaterId includes information about the cinema, or you have a way to map TheaterId to CinemaId.
        //        filters.Add(Builders<Incident>.Filter.Regex(i => i.TheaterId, new BsonRegularExpression(cinemaId)));
        //    }

        //    var filter = filters.Count > 0 ? Builders<Incident>.Filter.And(filters) : Builders<Incident>.Filter.Empty;

        //    return await _Incidents.Find(filter).ToListAsync();
        //}


        public async Task UpdateIncidentAsync(ObjectId incidentId, Incident updatedIncident)
        {
            var filter = Builders<Incident>.Filter.Eq(i => i.Id, incidentId);
            await _Incidents.ReplaceOneAsync(filter, updatedIncident);
        }

        public async Task DeleteIncidentAsync(ObjectId incidentId)
        {
            var filter = Builders<Incident>.Filter.Eq(i => i.Id, incidentId);
            await _Incidents.DeleteOneAsync(filter);
        }
    }
}
