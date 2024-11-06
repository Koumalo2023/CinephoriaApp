using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class ReservationViewModel
    {
        /// <summary>
        /// Identifiant de la séance réservée.
        /// </summary>
        public string ShowtimeId { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur effectuant la réservation.
        /// </summary>
        public string AppUserId { get; set; }

        // <summary>
        /// Statut de la réservation : CONFIRMED, CANCELLED.
        /// </summary>
        public ReservationStatus Status { get; set; }

        /// <summary>
        /// Liste des numéros de sièges réservés.
        /// </summary>
        public string[] SeatNumbers { get; set; }

        /// <summary>
        /// Prix total de la réservation.
        /// </summary>
        public float TotalPrice { get; set; }

        /// <summary>
        /// QR Code généré pour la réservation.
        /// </summary>
        public string QrCode { get; set; }
    }

}
