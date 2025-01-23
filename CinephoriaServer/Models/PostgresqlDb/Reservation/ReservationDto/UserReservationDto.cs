using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class UserReservationDto
    {
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
        /// Prix total de la réservation.
        /// </summary>
        public float TotalPrice { get; set; }

        /// <summary>
        /// Contenu du QR code généré pour la réservation.
        /// </summary>
        public string QrCode { get; set; }

        /// <summary>
        /// Indique si le QR code a été validé par un employé.
        /// </summary>
        public bool IsValidated { get; set; }

        /// <summary>
        /// Statut de la réservation (par exemple, CONFIRMED ou CANCELLED).
        /// </summary>
        public ReservationStatus Status { get; set; }

        /// <summary>
        /// Nombre de sièges réservés.
        /// </summary>
        public int NumberOfSeats { get; set; }
    }
}