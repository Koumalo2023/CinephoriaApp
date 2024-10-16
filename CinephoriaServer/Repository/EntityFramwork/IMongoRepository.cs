using Amazon.Runtime.Internal;
using CinephoriaServer.Models.MongooDb;
using MongoDB.Driver;

namespace CinephoriaServer.Repository.EntityFramwork
{
    public interface IMongoRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<bool> DeleteAsync(string id);
        Task<List<T>> FilterAsync(FilterDefinition<T> filter);
    }

}
