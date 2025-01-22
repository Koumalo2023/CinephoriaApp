namespace CinephoriaServer.Models.PostgresqlDb
{
    public class TheaterWithSeatsDto
    {
        /// <summary>
        /// Identifiant unique de la salle de cinéma.
        /// </summary>
        public int TheaterId { get; set; }

        /// <summary>
        /// Nom ou numéro de la salle.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Capacité maximale de la salle (nombre de sièges).
        /// </summary>
        public int SeatCount { get; set; }

        /// <summary>
        /// Liste des sièges disponibles dans cette salle.
        /// </summary>
        public List<SeatDto> Seats { get; set; } = new List<SeatDto>();
    }
}
