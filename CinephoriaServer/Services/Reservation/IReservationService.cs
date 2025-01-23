
using CinephoriaServer.Models.PostgresqlDb;

namespace CinephoriaServer.Services
{
    public interface IReservationService
    {

        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances disponibles.</returns>
        Task<List<ShowtimeDto>> GetMovieSessionsAsync(int movieId);

        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles.</returns>
        Task<List<SeatDto>> GetAvailableSeatsAsync(int showtimeId);


        /// <summary>
        /// Valide un QRCode scanné pour une réservation.
        /// </summary>
        /// <param name="qrCodeData">Les données du QRCode scanné.</param>
        /// <returns>True si la validation est réussie, sinon False.</returns>
        Task<bool> ValidatedSession(string qrCodeData);
    }

}
