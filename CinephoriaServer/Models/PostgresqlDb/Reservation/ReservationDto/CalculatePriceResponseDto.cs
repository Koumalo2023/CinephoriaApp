namespace CinephoriaServer.Models.PostgresqlDb
{
    
    public class CalculatePriceResponseDto
    {
        /// <summary>
        /// Prix total calculé pour les sièges sélectionnés.
        /// </summary>
        public decimal TotalPrice { get; set; }
    }
}