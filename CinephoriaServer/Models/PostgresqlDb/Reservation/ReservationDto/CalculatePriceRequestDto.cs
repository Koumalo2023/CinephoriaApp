namespace CinephoriaServer.Models.PostgresqlDb
{
    
    public class CalculatePriceRequestDto
    {
        /// <summary>
        /// Identifiant de la séance pour laquelle le prix est calculé.
        /// </summary>
        public int ShowtimeId { get; set; }

        /// <summary>
        /// Liste des numéros de sièges pour lesquels le prix est calculé.
        /// </summary>
        public List<string> SeatNumbers { get; set; } = new List<string>();
    }
}