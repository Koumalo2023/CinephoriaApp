using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Repository.EntityFramwork;

namespace CinephoriaServer.Repository
{
    public interface IUnitOfWorkMongoDb : IDisposable
    {
        
        IMongoRepository<AdminDashboard> AdminDashboards { get; }

        Task SaveChangesAsync();
        Task<bool> ExistsAsync<T>(string id) where T : class;
    }
}
