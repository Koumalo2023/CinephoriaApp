using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CinephoriaServer.Repository.EntityFramwork
{
    public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;

        public EFRepository(DbContext context)
        {
            _context = context;
        }

        // Helper to choose between tracking and no-tracking
        private IQueryable<TEntity> ApplyTracking(IQueryable<TEntity> query, bool trackChanges)
        {
            return trackChanges ? query : query.AsNoTracking();
        }

        // Basic queries
        public List<TEntity> GetAll(bool trackChanges = false)
        {
            return ApplyTracking(_context.Set<TEntity>(), trackChanges).ToList();
        }

        public async Task<List<TEntity>> GetAllAsync(bool trackChanges = false)
        {
            return await ApplyTracking(_context.Set<TEntity>(), trackChanges).ToListAsync();
        }

        public TEntity? GetById(int id, bool trackChanges = false)
        {
            return ApplyTracking(_context.Set<TEntity>(), trackChanges).FirstOrDefault(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<TEntity?> GetByIdAsync(int id, bool trackChanges = false)
        {
            // Récupérer le nom de la propriété clé primaire
            var keyName = _context.Model.FindEntityType(typeof(TEntity))
                            .FindPrimaryKey()
                            .Properties
                            .Select(p => p.Name)
                            .Single();

            return await ApplyTracking(_context.Set<TEntity>(), trackChanges)
                .FirstOrDefaultAsync(e => EF.Property<int>(e, keyName) == id);
        }



        // Filtered queries
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false)
        {
            return ApplyTracking(_context.Set<TEntity>().Where(predicate), trackChanges).ToList();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false)
        {
            return await ApplyTracking(_context.Set<TEntity>().Where(predicate), trackChanges).ToListAsync();
        }

        // Basic commands
        public void Create(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            _context.SaveChanges();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        // Other utilities
        public int Count()
        {
            return _context.Set<TEntity>().Count();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<TEntity>().CountAsync();
        }
    }
}
