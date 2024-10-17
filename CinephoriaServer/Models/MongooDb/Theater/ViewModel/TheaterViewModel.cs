using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.MongooDb
{
    public class TheaterViewModel
    {
        /// <summary>
        /// Identifiant de la salle.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Nom ou numéro de la salle.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Capacité maximale de la salle (nombre de sièges).
        /// </summary>
        public int SeatCount { get; set; }

        /// <summary>
        /// Identifiant du cinéma auquel appartient la salle.
        /// </summary>
        public int CinemaId { get; set; }

        /// <summary>
        /// Indique si la salle est pleinement fonctionnelle.
        /// </summary>
        public bool IsOperational { get; set; }

        /// <summary>
        /// Qualité de projection disponible dans la salle.
        /// </summary>
        public ProjectionQuality ProjectionQuality { get; set; }
    }
}
