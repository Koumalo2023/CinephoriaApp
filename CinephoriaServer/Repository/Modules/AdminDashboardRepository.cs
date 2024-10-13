using CinephoriaServer.Models.MongooDb;
using MongoDB.Driver;

namespace CinephoriaServer.Repository
{
    public interface IAdminDashboardRepository
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
        /// Récupère le nombre total de réservations effectuées
        /// sur une période donnée, tous films confondus.
        /// </summary>
        /// <param name="startDate">Date de début de la période</param>
        /// <param name="endDate">Date de fin de la période</param>
        /// <returns>Nombre total de réservations</returns>
        Task<int> GetTotalReservationsCountAsync(DateTime startDate, DateTime endDate);

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
        Task<List<string>> GetMoviesWithZeroReservationsAsync(DateTime startDate, DateTime endDate);

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
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly IMongoCollection<Showtime> _showtimes;
        private readonly IMongoCollection<Movie> _movies;

        public AdminDashboardRepository(IMongoDatabase database)
        {
            _showtimes = database.GetCollection<Showtime>("Showtimes");
            _movies = database.GetCollection<Movie>("Movies");
        }

        /// <summary>
        /// Récupère le nombre total de réservations pour un film spécifique sur une période donnée.
        /// </summary>
        public async Task<int> GetReservationsCountByMovieAsync(string movieId, DateTime startDate, DateTime endDate)
        {
            var filter = Builders<Showtime>.Filter.And(
                Builders<Showtime>.Filter.Eq(s => s.MovieId, movieId),
                Builders<Showtime>.Filter.Gte(s => s.StartTime, startDate),
                Builders<Showtime>.Filter.Lte(s => s.StartTime, endDate)
            );

            var showtimes = await _showtimes.Find(filter).ToListAsync();
            return showtimes.Sum(s => s.Reservations.Count);
        }

        /// <summary>
        /// Récupère le nombre total de réservations effectuées sur une période donnée (tous films confondus).
        /// </summary>
        public async Task<int> GetTotalReservationsCountAsync(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<Showtime>.Filter.And(
                Builders<Showtime>.Filter.Gte(s => s.StartTime, startDate),
                Builders<Showtime>.Filter.Lte(s => s.StartTime, endDate)
            );

            var showtimes = await _showtimes.Find(filter).ToListAsync();
            return showtimes.Sum(s => s.Reservations.Count);
        }

        /// <summary>
        /// Récupère les films les plus réservés sur une période donnée.
        /// </summary>
        public async Task<List<MovieStatistics>> GetTopMoviesByReservationsAsync(DateTime startDate, DateTime endDate, int topN)
        {
            var filter = Builders<Showtime>.Filter.And(
                Builders<Showtime>.Filter.Gte(s => s.StartTime, startDate),
                Builders<Showtime>.Filter.Lte(s => s.StartTime, endDate)
            );

            var showtimes = await _showtimes.Find(filter).ToListAsync();

            var movieReservations = showtimes
                .GroupBy(s => s.MovieId)
                .Select(g => new MovieStatistics
                {
                    MovieId = g.Key,
                    ReservationsCount = g.Sum(s => s.Reservations.Count)
                })
                .OrderByDescending(m => m.ReservationsCount)
                .Take(topN)
                .ToList();

            return movieReservations;
        }

        /// <summary>
        /// Récupère les réservations hebdomadaires pour un film spécifique.
        /// </summary>
        public async Task<Dictionary<DateTime, int>> GetWeeklyReservationsCountByMovieAsync(string movieId)
        {
            var startDate = DateTime.UtcNow.AddDays(-7);
            var filter = Builders<Showtime>.Filter.And(
                Builders<Showtime>.Filter.Eq(s => s.MovieId, movieId),
                Builders<Showtime>.Filter.Gte(s => s.StartTime, startDate)
            );

            var showtimes = await _showtimes.Find(filter).ToListAsync();

            var weeklyReservations = showtimes
                .GroupBy(s => new DateTime(s.StartTime.Year, s.StartTime.Month, s.StartTime.Day))
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(s => s.Reservations.Count)
                );

            return weeklyReservations;
        }

        /// <summary>
        /// Récupère la liste des films sans réservations sur une période donnée.
        /// </summary>
        public async Task<List<string>> GetMoviesWithZeroReservationsAsync(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<Showtime>.Filter.And(
                Builders<Showtime>.Filter.Gte(s => s.StartTime, startDate),
                Builders<Showtime>.Filter.Lte(s => s.StartTime, endDate)
            );

            var showtimes = await _showtimes.Find(filter).ToListAsync();
            var movieIdsWithReservations = showtimes
                .Where(s => s.Reservations.Count > 0)
                .Select(s => s.MovieId)
                .Distinct()
                .ToList();

            var allMovies = await _movies.Find(Builders<Movie>.Filter.Empty).ToListAsync();
            var moviesWithoutReservations = allMovies
                .Where(m => !movieIdsWithReservations.Contains(m.Id))
                .Select(m => m.Id)
                .ToList();

            return moviesWithoutReservations;
        }

        /// <summary>
        /// Récupère la tendance des réservations pour un film donné sur une période.
        /// </summary>
        public async Task<List<ReservationTrend>> GetReservationTrendByMovieAsync(string movieId, DateTime startDate, DateTime endDate)
        {
            var filter = Builders<Showtime>.Filter.And(
                Builders<Showtime>.Filter.Eq(s => s.MovieId, movieId),
                Builders<Showtime>.Filter.Gte(s => s.StartTime, startDate),
                Builders<Showtime>.Filter.Lte(s => s.StartTime, endDate)
            );

            var showtimes = await _showtimes.Find(filter).ToListAsync();

            var reservationTrends = showtimes
                .GroupBy(s => new DateTime(s.StartTime.Year, s.StartTime.Month, s.StartTime.Day))
                .Select(g => new ReservationTrend
                {
                    DateRange = g.Key,
                    ReservationsCount = g.Sum(s => s.Reservations.Count)
                })
                .OrderBy(rt => rt.DateRange)
                .ToList();

            return reservationTrends;
        }

        /// <summary>
        /// Récupère un résumé des réservations par cinéma sur une période donnée.
        /// </summary>
        public async Task<List<CinemaReservationSummary>> GetReservationSummaryByCinemaAsync(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<Showtime>.Filter.And(
                Builders<Showtime>.Filter.Gte(s => s.StartTime, startDate),
                Builders<Showtime>.Filter.Lte(s => s.StartTime, endDate)
            );

            var showtimes = await _showtimes.Find(filter).ToListAsync();

            var cinemaReservations = showtimes
                .GroupBy(s => s.CinemaId)
                .Select(g => new CinemaReservationSummary
                {
                    CinemaId = g.Key,
                    TotalReservations = g.Sum(s => s.Reservations.Count)
                })
                .OrderByDescending(cr => cr.TotalReservations)
                .ToList();

            return cinemaReservations;
        }
    }
}
