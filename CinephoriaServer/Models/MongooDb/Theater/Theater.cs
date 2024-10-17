using CinephoriaServer.Models.PostgresqlDb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.MongooDb
{
    public class Theater
    {
        /// <summary>
        /// Identifiant unique de la salle de projection.
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Nom ou numéro de la salle.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Capacité maximale de la salle (nombre de sièges).
        /// </summary>
        public int SeatCount { get; set; }


        /// <summary>
        /// l'identifiant du Cinema auquel apparient la salle.
        /// </summary>
        public int CinemaId { get; set; }

        // <summary>
        /// Indique si la salle est pleinement fonctionnelle.
        /// </summary>
        public bool IsOperational { get; set; }

        /// <summary>
        /// Qualité de projection disponible dans la salle (ex : "4DX", "3D", "4K").
        /// </summary>
        public ProjectionQuality ProjectionQuality { get; set; }

        /// <summary>
        /// Liste des incidents signalés dans la salle (relation un-à-plusieurs).
        /// </summary>
        public ICollection<Incident> Incidents { get; set; }

        /// <summary>
        /// Liste des séances projetées dans cette salle.
        /// </summary>
        public List<Showtime> Showtimes { get; set; }

        /// <summary>
        /// Navigation vers le cinéma auquel la salle appartient (relation plusieurs-à-un).
        /// </summary>
        public Cinema Cinema { get; set; }
    }
}
