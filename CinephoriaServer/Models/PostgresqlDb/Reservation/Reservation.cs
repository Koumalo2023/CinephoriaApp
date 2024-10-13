using CinephoriaServer.Models.MongooDb;
using static CinephoriaServer.Configurations.EnumConfig;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identifiant unique de la réservation.
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur ayant fait la réservation.
        /// </summary>
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }

        /// <summary>
        /// Identifiant de la séance réservée.
        /// </summary>
        [ForeignKey("Showtime")]
        public int ShowtimeId { get; set; }

        /// <summary>
        /// Liste des numéros de sièges réservés.
        /// </summary>
        public string[] SeatNumbers { get; set; }

        /// <summary>
        /// Prix total de la réservation.
        /// </summary>
        public float TotalPrice { get; set; }

        /// <summary>
        /// Contenu du QR code pour validation à l'entrée.
        /// </summary>
        public string QrCode { get; set; }


        /// <summary>
        /// validation du QR  à l'entrée par un employé.
        /// </summary>
        public bool IsValidated { get; set; }


        /// <summary>
        /// Statut de la réservation : CONFIRMED, CANCELLED.
        /// </summary>
        public ReservationStatus Status { get; set; }

        /// <summary>
        /// Date de la réservation.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de la dernière mise à jour des informations sur la réservation.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Nombre de sièges réservés (calculé à partir de SeatNumbers).
        /// </summary>
        public int NumberOfSeats => SeatNumbers?.Length ?? 0;

        /// <summary>
        /// Utilisateur ayant fait la réservation (navigation).
        /// </summary>
        public AppUser AppUser { get; set; }

        /// <summary>
        /// Séance pour laquelle la réservation a été effectuée (navigation).
        /// </summary>
        public Showtime Showtime { get; set; }
    }
}
