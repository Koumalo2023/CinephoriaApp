using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using CinephoriaServer.Models.PostgresqlDb;

namespace CinephoriaServer.Models.MongooDb
{
    public class AdminDashboardDto
    {
        [BsonId]
        /// <summary>
        /// Identifiant unique du tableau de bord.
        /// </summary>
        public ObjectId Id { get; set; }

        /// <summary>
        /// Plage de dates de visualisation (7 jours).
        /// </summary>
        public DateTime DateRange { get; set; }

        /// <summary>
        /// Nombre total de réservations effectuées pour un film spécifique sur une période de 7 jours.
        /// </summary>
        public int ReservationsCount { get; set; }

        /// <summary>
        /// Liste des réservations par film avec les détails associés.
        /// </summary>
        public List<ReservationDto> Reservations { get; set; }

        /// <summary>
        /// Identifiant du film concerné par les données.
        /// </summary>
        public string MovieId { get; set; }

        /// <summary>
        /// Période de début des statistiques (date).
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Période de fin des statistiques (date).
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
