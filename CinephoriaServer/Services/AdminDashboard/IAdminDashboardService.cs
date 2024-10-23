using CinephoriaServer.Models.MongooDb;

namespace CinephoriaServer.Services
{
    public interface IAdminDashboardService
    {
        /// <summary>
        /// Récupère le nombre total de réservations pour un film spécifique
        /// sur une période donnée.
        /// </summary>
        /// <param name="movieId">Identifiant du film</param>
        /// <param name="startDate">Date de début de la période</param>
        /// <param name="endDate">Date de fin de la période</param>
        /// <returns>Nombre de réservations</returns>
        Task<int> GetReservationsCountByMovieAsync(string movieId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Récupère les films les plus réservés sur une période donnée.
        /// </summary>
        /// <param name="startDate">Date de début de la période</param>
        /// <param name="endDate">Date de fin de la période</param>
        /// <param name="topN">Le nombre de films à récupérer</param>
        /// <returns>Liste des films et du nombre de réservations</returns>
        Task<List<MovieStatistics>> GetTopMoviesByReservationsAsync(DateTime startDate, DateTime endDate, int topN);

        /// <summary>
        /// Récupère les réservations hebdomadaires pour un film spécifique.
        /// </summary>
        /// <param name="movieId">Identifiant du film</param>
        /// <returns>Un dictionnaire avec la date de début de la semaine et le nombre de réservations</returns>
        Task<Dictionary<DateTime, int>> GetWeeklyReservationsCountByMovieAsync(string movieId);

        /// <summary>
        /// Récupère la liste des films sans réservations sur une période donnée.
        /// </summary>
        /// <param name="startDate">Date de début de la période</param>
        /// <param name="endDate">Date de fin de la période</param>
        /// <returns>Liste des identifiants des films sans réservation</returns>
        //Task<List<string>> GetMoviesWithZeroReservationsAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Récupère la tendance des réservations pour un film donné sur une période.
        /// </summary>
        /// <param name="movieId">Identifiant du film</param>
        /// <param name="startDate">Date de début de la période</param>
        /// <param name="endDate">Date de fin de la période</param>
        /// <returns>Liste des tendances de réservation par date</returns>
        Task<List<ReservationTrend>> GetReservationTrendByMovieAsync(string movieId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Récupère un résumé des réservations par cinéma sur une période donnée.
        /// </summary>
        /// <param name="startDate">Date de début de la période</param>
        /// <param name="endDate">Date de fin de la période</param>
        /// <returns>Liste des cinémas avec leur total de réservations</returns>
        Task<List<CinemaReservationSummary>> GetReservationSummaryByCinemaAsync(DateTime startDate, DateTime endDate);
    }
}
