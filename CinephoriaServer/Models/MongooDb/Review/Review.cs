using MongoDB.Bson.Serialization.Attributes;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.MongooDb
{
    public class Review
    {
        /// <summary>
        /// Identifiant unique de l'avis.
        /// </summary>
        [BsonId]
        public string Id { get; set; }

        /// <summary>
        /// Identifiant du film sur lequel l'utilisateur a déposé un avis.
        /// </summary>
        public string MovieId { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur ayant soumis l'avis.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Contenu textuel de l'avis laissé par l'utilisateur.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Note donnée par l'utilisateur (sur 5).
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Date de l'avis.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de mise à jour de l'avis.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Date de soumission de l'avis.
        /// </summary>
        public DateTime SubmittedAt { get; set; }

        /// <summary>
        /// Statut de l'avis (ex : "En attente", "Validé", "Refusé").
        /// </summary>
        public IncidentStatus Status { get; set; }
    }
}
