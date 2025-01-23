namespace CinephoriaServer.Models.PostgresqlDb
{
    
    public class HoldSeatsRequestDto
    {
        /// <summary>
        /// Identifiant de la séance pour laquelle les sièges sont mis en attente.
        /// </summary>
        public int ShowtimeId { get; set; }

        /// <summary>
        /// Liste des numéros de sièges à mettre en attente.
        /// </summary>
        public List<string> SeatNumbers { get; set; } = new List<string>();
    }
}