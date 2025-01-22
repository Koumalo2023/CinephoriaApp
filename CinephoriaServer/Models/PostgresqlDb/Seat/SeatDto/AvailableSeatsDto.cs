namespace CinephoriaServer.Models.PostgresqlDb
{
    public class AvailableSeatsDto
    {
        /// <summary>
        /// Identifiant de la séance.
        /// </summary>
        public int ShowtimeId { get; set; }
        /// <summary>
        /// Numéro ou identifiant du siège dans la salle (ex: "A1", "B2").
        /// </summary>
        public string SeatNumber { get; set; }

        /// <summary>
        /// Indique si le siège est réservé pour les personnes à mobilité réduite.
        /// </summary>
        public bool IsAccessible { get; set; }

        /// <summary>
        /// Indique si le siège est disponible pour réservation.
        /// </summary>
        public bool IsAvailable { get; set; }
    }
}
