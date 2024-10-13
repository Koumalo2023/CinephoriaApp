﻿using CinephoriaServer.Models.PostgresqlDb;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.MongooDb
{
    public class ShowtimeDtO
    {
        /// <summary>
        /// Identifiant unique de la séance.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Identifiant du film projeté durant cette séance.
        /// </summary>
        public string MovieId { get; set; }

        /// <summary>
        /// Identifiant de la salle où a lieu la projection.
        /// </summary>
        public string TheaterId { get; set; }

        /// <summary>
        /// Identifiant du cinéma où a lieu la projection.
        /// </summary>
        public string CinemaId { get; set; }

        /// <summary>
        /// Heure de début de la séance.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Heure de fin de la séance.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Nombre de sièges disponibles.
        /// </summary>
        public int AvailableSeats { get; set; }

        /// <summary>
        /// Qualité de la projection (par exemple : "4DX", "3D").
        /// </summary>
        public ProjectionQuality Quality { get; set; }

        /// <summary>
        /// Qualité de la projection (ex : "4DX", "3D", "4K").
        /// </summary>
        public string ProjectionQuality { get; set; }

        /// <summary>
        /// Prix de la séance en fonction de la qualité.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Navigation vers le film projeté (relation plusieurs-à-un).
        /// </summary>
        public MovieDto Movie { get; set; }  // DTO pour le film

        /// <summary>
        /// Navigation vers la salle où se déroule la séance (relation plusieurs-à-un).
        /// </summary>
        public TheaterDtO Theater { get; set; }

        /// <summary>
        /// Liste des réservations associées à cette séance.
        /// </summary>
        public ICollection<ReservationDto> Reservations { get; set; }
    }
}
