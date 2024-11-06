using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Repository;
using MongoDB.Driver;

namespace CinephoriaServer.Services
{
    public class AdminDashboardService: IAdminDashboardService
    {
        private readonly IUnitOfWorkMongoDb _unitOfWorkMongoDb;

        public AdminDashboardService(IUnitOfWorkMongoDb unitOfWorkMongoDb)
        {
            _unitOfWorkMongoDb = unitOfWorkMongoDb;
        }

        public async Task<int> GetReservationsCountByMovieAsync(string movieId, DateTime startDate, DateTime endDate)
        {
            var filter = Builders<AdminDashboard>.Filter.Where(dashboard =>
                dashboard.MovieId == movieId &&
                dashboard.StartDate >= startDate &&
                dashboard.EndDate <= endDate);

            var dashboards = await _unitOfWorkMongoDb.AdminDashboards.FilterAsync(filter);

            return dashboards.Sum(dashboard => dashboard.ReservationsCount);
        }

        public async Task<List<MovieStatistics>> GetTopMoviesByReservationsAsync(DateTime startDate, DateTime endDate, int topN)
        {
            var filter = Builders<AdminDashboard>.Filter.Where(dashboard =>
                dashboard.StartDate >= startDate &&
                dashboard.EndDate <= endDate);

            var dashboards = await _unitOfWorkMongoDb.AdminDashboards.FilterAsync(filter);

            return dashboards
                .OrderByDescending(dashboard => dashboard.ReservationsCount)
                .Take(topN)
                .Select(dashboard => new MovieStatistics
                {
                    MovieId = dashboard.MovieId,
                    ReservationsCount = dashboard.ReservationsCount
                }).ToList();
        }

        public async Task<Dictionary<DateTime, int>> GetWeeklyReservationsCountByMovieAsync(string movieId)
        {
            var filter = Builders<AdminDashboard>.Filter.Where(dashboard => dashboard.MovieId == movieId);

            var dashboards = await _unitOfWorkMongoDb.AdminDashboards.FilterAsync(filter);

            var groupedByWeek = dashboards.GroupBy(dashboard => dashboard.DateRange.StartOfWeek());

            return groupedByWeek.ToDictionary(
                group => group.Key,
                group => group.Sum(dashboard => dashboard.ReservationsCount)
            );
        }

        public async Task<List<ReservationTrend>> GetReservationTrendByMovieAsync(string movieId, DateTime startDate, DateTime endDate)
        {
            var filter = Builders<AdminDashboard>.Filter.Where(dashboard =>
                dashboard.MovieId == movieId &&
                dashboard.StartDate >= startDate &&
                dashboard.EndDate <= endDate);

            var dashboards = await _unitOfWorkMongoDb.AdminDashboards.FilterAsync(filter);

            return dashboards.Select(dashboard => new ReservationTrend
            {
                DateRange = dashboard.DateRange,
                ReservationsCount = dashboard.ReservationsCount
            }).ToList();
        }


        public async Task<List<CinemaReservationSummary>> GetReservationSummaryByCinemaAsync(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<AdminDashboard>.Filter.Where(dashboard =>
                dashboard.StartDate >= startDate &&
                dashboard.EndDate <= endDate);

            var dashboards = await _unitOfWorkMongoDb.AdminDashboards.FilterAsync(filter);

            var groupedByCinema = dashboards.GroupBy(dashboard => dashboard.CinemaId);

            return groupedByCinema.Select(group => new CinemaReservationSummary
            {
                CinemaId = group.Key.ToString(),
                TotalReservations = group.Sum(dashboard => dashboard.ReservationsCount)
            }).ToList();
        }

    }
}
