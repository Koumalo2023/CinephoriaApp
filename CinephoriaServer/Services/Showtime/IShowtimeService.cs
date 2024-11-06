using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;

namespace CinephoriaServer.Services
{
    public interface IShowtimeService
    {
        /// <summary>
        /// Crée une nouvelle séance avec les informations fournies.
        /// </summary>
        /// <param name="model">Le modèle contenant les détails de la séance à créer.</param>
        /// <returns>Un GeneralServiceResponseData indiquant le statut de la création de la séance.</returns>
        Task<GeneralServiceResponseData<object>> CreateShowtimeAsync(ShowtimeViewModel model);

        /// <summary>
        /// Récupère les séances programmées pour un film spécifique dans un cinéma donné.
        /// </summary>
        /// <param name="movieId">L'identifiant du film pour lequel récupérer les séances.</param>
        /// <param name="cinemaId">L'identifiant du cinéma où le film est diffusé.</param>
        /// <returns>Un GeneralServiceResponseData contenant une liste de séances correspondant aux critères spécifiés.</returns>
        Task<GeneralServiceResponseData<List<Showtime>>> GetShowtimesForMovieInCinemaAsync(string movieId, int cinemaId);

        /// <summary>
        /// Récupère les séances disponibles pour l'utilisateur authentifié.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur pour lequel récupérer les séances.</param>
        /// <returns>Un GeneralServiceResponseData contenant une liste de séances accessibles par l'utilisateur.</returns>
        Task<GeneralServiceResponseData<List<Showtime>>> GetShowtimesForAuthenticatedUserAsync(string userId);

        /// <summary>
        /// Met à jour une séance existante avec de nouvelles informations.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance à mettre à jour.</param>
        /// <param name="model">Le modèle contenant les nouvelles informations de la séance.</param>
        /// <returns>Un GeneralServiceResponseData indiquant le statut de la mise à jour.</returns>
        Task<GeneralServiceResponseData<object>> UpdateShowtimeAsync(string showtimeId, ShowtimeViewModel model);

        /// <summary>
        /// Supprime une séance spécifique par son identifiant.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance à supprimer.</param>
        /// <returns>Un GeneralServiceResponseData indiquant le statut de la suppression.</returns>
        Task<GeneralServiceResponseData<object>> DeleteShowtimeAsync(string showtimeId);

        /// <summary>
        /// Récupère les séances programmées pour l'utilisateur spécifié pour aujourd'hui et les jours futurs.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur pour lequel récupérer les séances.</param>
        /// <returns>Un GeneralServiceResponseData contenant une liste de séances pour aujourd'hui et le futur.</returns>
        Task<GeneralServiceResponseData<List<Showtime>>> GetUserShowtimesForTodayAndFutureAsync(string userId);

        /// <summary>
        /// Vérifie si une séance se chevauche avec d'autres séances dans le même théâtre.
        /// </summary>
        /// <param name="theaterId">L'identifiant du théâtre où la séance est programmée.</param>
        /// <param name="startTime">L'heure de début de la séance à vérifier.</param>
        /// <param name="endTime">L'heure de fin de la séance à vérifier.</param>
        /// <param name="showtimeId">L'identifiant optionnel de la séance à exclure de la vérification.</param>
        /// <returns>Un GeneralServiceResponseData contenant un booléen indiquant si la séance se chevauche ou non.</returns>
        Task<GeneralServiceResponseData<bool>> IsShowtimeOverlappingAsync(string theaterId, DateTime startTime, DateTime endTime, string? showtimeId = null);

    }
}
