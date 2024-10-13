using MongoDB.Bson.Serialization.Attributes;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class ReservationDto
    {
        [BsonId]
        /// <summary>
        /// Identifiant unique de la réservation.
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur ayant effectué la réservation.
        /// </summary>
        public string AppUserId { get; set; }

        /// <summary>
        /// Identifiant de la séance réservée.
        /// </summary>
        public int ShowtimeId { get; set; }

        /// <summary>
        /// Numéro du siège réservé.
        /// </summary>
        public string[] SeatNumbers { get; set; }

        /// <summary>
        /// Prix de la réservation.
        /// </summary>
        public float TotalPrice { get; set; }

        /// <summary>
        /// QR code associé à la réservation.
        /// </summary>
        public string QrCode { get; set; }

        /// <summary>
        /// validation du QR  à l'entrée par un employé.
        /// </summary>
        public bool IsValidated { get; set; }

        /// <summary>
        /// Statut de la réservation.
        /// </summary>
        public ReservationStatus Status { get; set; }


        /// <summary>
        /// Nombre de sièges réservés (calculé à partir de SeatNumbers).
        /// </summary>
        public int NumberOfSeats => SeatNumbers?.Length ?? 0;

        /// <summary>
        /// Date de création de la réservation.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de la dernière mise à jour de la réservation.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
