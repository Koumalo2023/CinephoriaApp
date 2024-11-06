using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;

namespace CinephoriaServer.Services
{
    public interface IReservationService
    {
        /// <summary>
        /// Récupère la liste complète des cinémas.
        /// </summary>
        /// <returns>Une liste de cinémas disponibles dans la base de données.</returns>
        Task<List<Cinema>> GetAllCinemasAsync();

        /// <summary>
        /// Récupère les films programmés dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma pour lequel récupérer les films.</param>
        /// <returns>Une liste de films diffusés dans le cinéma spécifié.</returns>
        Task<List<Movie>> GetMoviesByCinemaAsync(int cinemaId);

        /// <summary>
        /// Génère et renvoie un code QR pour une réservation spécifique.
        /// </summary>
        /// <param name="reservationId">L'identifiant de la réservation pour laquelle générer le code QR.</param>
        /// <returns>Un GeneralServiceResponseData contenant le code QR en format byte array.</returns>
        Task<GeneralServiceResponseData<byte[]>> GetReservationQrCodeAsync(int reservationId);

        /// <summary>
        /// Valide une réservation à partir de son code QR.
        /// </summary>
        /// <param name="qrCode">Le code QR de la réservation à valider.</param>
        /// <returns>Un GeneralServiceResponseData indiquant le statut de la validation de la réservation.</returns>
        Task<GeneralServiceResponseData<object>> ValidateReservationAsync(string qrCode);

        /// <summary>
        /// Récupère les réservations effectuées par un utilisateur spécifique.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur pour lequel récupérer les réservations.</param>
        /// <returns>Une liste d'objets ReservationDto correspondant aux réservations de l'utilisateur.</returns>
        Task<List<ReservationDto>> GetReservationsByUserAsync(string userId);

        /// <summary>
        /// Crée une nouvelle réservation pour un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur pour lequel la réservation est créée.</param>
        /// <param name="model">Le modèle contenant les détails de la réservation.</param>
        /// <returns>Un GeneralServiceResponseData contenant les informations de la réservation créée.</returns>
        Task<GeneralServiceResponseData<ReservationDto>> CreateReservationAsync(string userId, ReservationViewModel model);

        /// <summary>
        /// Récupère une réservation spécifique par son identifiant.
        /// </summary>
        /// <param name="reservationId">L'identifiant de la réservation à récupérer.</param>
        /// <returns>Un objet ReservationDto contenant les informations de la réservation.</returns>
        Task<ReservationDto> GetReservationByIdAsync(int reservationId);

        /// <summary>
        /// Récupère les réservations associées à une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance pour laquelle récupérer les réservations.</param>
        /// <returns>Une liste d'objets ReservationDto correspondant aux réservations pour la séance spécifiée.</returns>
        Task<List<ReservationDto>> GetReservationsByShowtimeAsync(string showtimeId);

        /// <summary>
        /// Supprime une réservation spécifique par son identifiant.
        /// </summary>
        /// <param name="reservationId">L'identifiant de la réservation à supprimer.</param>
        /// <returns>Un GeneralServiceResponseData indiquant le statut de l'opération de suppression.</returns>
        Task<GeneralServiceResponseData<object>> DeleteReservationAsync(int reservationId);

    }

}
