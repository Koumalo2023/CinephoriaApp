using CinephoriaBackEnd.Data;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository.EntityFramwork;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace CinephoriaServer.Repository
{
    public interface IReservationRepository : IReadRepository<Reservation>, IWriteRepository<Reservation>
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
        /// Récupère la liste des réservations d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une liste de réservations.</returns>
        Task<List<Reservation>> GetUserReservationsAsync(string userId);

        /// <summary>
        /// Crée une nouvelle réservation pour une séance spécifique.
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance.</param>
        /// <param name="seats">La liste des sièges réservés.</param>
        /// <param name="quality">La qualité de projection.</param>
        /// <param name="reservationHoldId">L'identifiant de la réservation en attente.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task CreateReservationAsync(int sessionId, List<string> seats, string quality, Guid reservationHoldId);

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
        /// Annule une réservation en fonction de son identifiant.
        /// </summary>
        /// <param name="reservationId">L'identifiant de la réservation à annuler.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task CancelReservationAsync(int reservationId);
    }


    //public class ReservationRepository : IReservationRepository
    //{
        
    //}
}
