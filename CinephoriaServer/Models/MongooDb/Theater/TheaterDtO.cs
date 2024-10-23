using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.MongooDb
{
    public class TheaterDto
    {
        /// <summary>
        /// Identifiant unique de la salle de projection.
        /// </summary>
        public string Id { get; set; }

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
        /// Qualité de projection disponible dans la salle (ex : "4DX", "3D", "4K").
        /// </summary>
        public ProjectionQuality ProjectionQuality { get; set; }

        /// <summary>
        /// Liste des incidents signalés dans la salle (relation un-à-plusieurs).
        /// </summary>
        public ICollection<IncidentDto> Incidents { get; set; }

        /// <summary>
        /// Liste des séances projetées dans cette salle (relation un-à-plusieurs).
        /// </summary>
        public List<ShowtimeDto> Showtimes { get; set; }
    }
}
