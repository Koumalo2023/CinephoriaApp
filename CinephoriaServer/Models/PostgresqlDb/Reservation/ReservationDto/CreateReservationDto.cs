﻿using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class CreateReservationDto
    {
        /// <summary>
        /// Identifiant de l'utilisateur faisant la réservation.
        /// </summary>
        public string AppUserId { get; set; }

        /// <summary>
        /// Identifiant de la séance réservée.
        /// </summary>
        public int ShowtimeId { get; set; }

        /// <summary>
        /// Liste des sièges réservés.
        /// </summary>
        public ICollection<string> SeatNumbers { get; set; } = new List<string>();

        /// <summary>
        /// Prix total de la réservation.
        /// </summary>
        public float TotalPrice { get; set; }

        /// <summary>
        /// Contenu du QR code pour validation à l'entrée.
        /// </summary>
        public string QrCode { get; set; }

        /// <summary>
        /// Statut de la réservation : CONFIRMED, CANCELLED.
        /// </summary>
        public ReservationStatus Status { get; set; }

        /// <summary>
        /// Nombre de sièges réservés.
        /// </summary>
        public int NumberOfSeats { get; set; }
    }
}
