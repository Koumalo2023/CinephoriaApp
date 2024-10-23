using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;

namespace CinephoriaServer.Services
{
    public interface IShowtimeService
    {
        Task<GeneralServiceResponseData<object>> CreateShowtimeAsync(ShowtimeViewModel model);
        Task<GeneralServiceResponseData<List<Showtime>>> GetShowtimesForMovieInCinemaAsync(string movieId, int cinemaId);
        Task<GeneralServiceResponseData<List<Showtime>>> GetShowtimesForAuthenticatedUserAsync(string userId);
        Task<GeneralServiceResponseData<object>> UpdateShowtimeAsync(string showtimeId, ShowtimeViewModel model);
        /// <summary>
        /// Supprime une séance existante par son identifiant.
        /// </summary>
        Task<GeneralServiceResponseData<object>> DeleteShowtimeAsync(string showtimeId);

        /// <summary>
        /// Récupère les séances auxquelles l'utilisateur a un billet pour le jour courant et les jours suivants.
        /// </summary>
        Task<GeneralServiceResponseData<List<Showtime>>> GetUserShowtimesForTodayAndFutureAsync(string userId);

        /// <summary>
        /// Vérifie si une séance chevauche une autre dans la même salle.
        /// </summary>
        Task<GeneralServiceResponseData<bool>> IsShowtimeOverlappingAsync(string theaterId, DateTime startTime, DateTime endTime, string? showtimeId = null);
    }
}
