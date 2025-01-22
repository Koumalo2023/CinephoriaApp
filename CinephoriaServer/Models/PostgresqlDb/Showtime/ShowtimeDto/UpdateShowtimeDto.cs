using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class UpdateShowtimeDto
    {
        /// <summary>
        /// Identifiant unique de la séance à mettre à jour.
        /// </summary>
        public int ShowtimeId { get; set; }

        /// <summary>
        /// Identifiant du film projeté durant cette séance.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Identifiant de la salle où a lieu la projection.
        /// </summary>
        public int TheaterId { get; set; }

        /// <summary>
        /// Identifiant du cinéma où a lieu la projection.
        /// </summary>
        public int CinemaId { get; set; }

        /// <summary>
        /// Nouvelle heure de début de la séance.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Nouvelle qualité de la projection (4DX, 3D, etc.).
        /// </summary>
        public ProjectionQuality Quality { get; set; }

        /// <summary>
        /// Nouvelle heure de fin de la séance.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Nouveau prix de la séance en fonction de la qualité.
        /// </summary>
        public decimal Price { get; set; }
    }
}
