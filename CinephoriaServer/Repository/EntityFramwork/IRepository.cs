using System.Linq.Expressions;

namespace CinephoriaServer.Repository.EntityFramwork
{
    public interface IRepository<TEntity> where TEntity : class
    {
        // Basic queries
        List<TEntity> GetAll(bool trackChanges = false);
        Task<List<TEntity>> GetAllAsync(bool trackChanges = false);
        TEntity? GetById(int id, bool trackChanges = false);
        Task<TEntity?> GetByIdAsync(int id, bool trackChanges = false);

        // Filtered queries
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false);

        // Basic commands
        void Create(TEntity entity);
        Task CreateAsync(TEntity entity);
        void Update(TEntity entity);
        Task UpdateAsync(TEntity entity);
        void Delete(TEntity entity);
        Task DeleteAsync(TEntity entity);

        // Other utilities
        int Count();
        Task<int> CountAsync();
    }
}
