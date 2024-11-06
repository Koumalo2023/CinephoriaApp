using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.MongooDb
{
    public class Incident
    {
        /// <summary>
        /// Identifiant unique de l'incident.
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Identifiant de la salle où l'incident a été signalé.
        /// </summary>
        public string TheaterId { get; set; }

        /// <summary>
        /// Description de l'incident.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Identifiant de l'employé qui a signalé l'incident.
        /// </summary>
        public string ReportedBy { get; set; }

        /// <summary>
        /// Statut de l'incident (PENDING, IN_PROGRESS, RESOLVED).
        /// </summary>
        public IncidentStatus Status { get; set; }

        /// <summary>
        /// Date à laquelle l'incident a été signalé.
        /// </summary>
        public DateTime ReportedAt { get; set; }

        /// <summary>
        /// Date de résolution de l'incident.
        /// </summary>
        public DateTime? ResolvedAt { get; set; }


        /// <summary>
        /// Les images de l'incident.
        /// </summary>
        public ICollection<string> ImageUrls { get; set; } = new List<string>();
    }
}
