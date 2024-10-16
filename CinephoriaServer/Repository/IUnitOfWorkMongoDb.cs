using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Repository.EntityFramwork;

namespace CinephoriaServer.Repository
{
    public interface IUnitOfWorkMongoDb : IDisposable
    {
        IMongoRepository<Movie> Movies { get; }
        IMongoRepository<Review> Reviews { get; }
        IMongoRepository<Incident> Incidents { get; }
        IMongoRepository<Theater> Theaters { get; }
        IMongoRepository<Showtime> Showtimes { get; }
        IMongoRepository<EmployeeAccount> EmployeeAccounts { get; }
        IMongoRepository<AdminDashboard> AdminDashboards { get; }

        Task SaveChangesAsync();
        Task<bool> ExistsAsync<T>(string id) where T : class;
    }
}
