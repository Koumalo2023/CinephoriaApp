using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository.EntityFramwork;
using MongoDB.Driver;

namespace CinephoriaServer.Repository
{
    public interface IShowtimeRepository : IReadRepository<Showtime>, IWriteRepository<Showtime>
    {
        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances.</returns>
        Task<List<Showtime>> GetMovieSessionsAsync(int movieId);

        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles.</returns>
        Task<List<Seat>> GetAvailableSeatsAsync(int sessionId);

        /// <summary>
        /// Calcule le prix total d'une réservation en fonction de la séance, des sièges sélectionnés et de la qualité de projection.
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance.</param>
        /// <param name="seats">La liste des sièges sélectionnés.</param>
        /// <param name="quality">La qualité de projection (par exemple, "4K", "IMAX").</param>
        /// <returns>Le prix total de la réservation.</returns>
        Task<decimal> CalculateReservationPriceAsync(int sessionId, List<string> seats, string quality);

        /// <summary>
        /// Bloque des sièges pour une réservation en attente.
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance.</param>
        /// <param name="seats">La liste des sièges à bloquer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task HoldSeatsAsync(int sessionId, List<string> seats);

        /// <summary>
        /// Confirme une réservation après avoir bloqué des sièges.
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance.</param>
        /// <param name="seats">La liste des sièges réservés.</param>
        /// <param name="quality">La qualité de projection.</param>
        /// <param name="reservationHoldId">L'identifiant de la réservation en attente.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task ConfirmReservationAsync(int sessionId, List<string> seats, string quality, Guid reservationHoldId);

        /// <summary>
        /// Crée une nouvelle séance (réservé aux administrateurs).
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <param name="theaterId">L'identifiant de la salle.</param>
        /// <param name="startTime">L'heure de début de la séance.</param>
        /// <param name="endTime">L'heure de fin de la séance.</param>
        /// <param name="quality">La qualité de projection.</param>
        /// <param name="price">Le prix de la séance.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task CreateSessionAsync(int movieId, int theaterId, DateTime startTime, DateTime endTime, string quality, decimal price);

        /// <summary>
        /// Met à jour les informations d'une séance existante (réservé aux administrateurs).
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance à mettre à jour.</param>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <param name="theaterId">L'identifiant de la salle.</param>
        /// <param name="startTime">La nouvelle heure de début.</param>
        /// <param name="endTime">La nouvelle heure de fin.</param>
        /// <param name="quality">La nouvelle qualité de projection.</param>
        /// <param name="price">Le nouveau prix.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateSessionAsync(int sessionId, int movieId, int theaterId, DateTime startTime, DateTime endTime, string quality, decimal price);

        /// <summary>
        /// Supprime une séance existante (réservé aux administrateurs).
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task DeleteSessionAsync(int sessionId);
    }


    //public class ShowtimeRepository : IShowtimeRepository
    //{
        
    //}
}
