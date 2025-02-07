using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository.EntityFramwork;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.Repository
{
    public interface IShowtimeRepository : IReadRepository<Showtime>, IWriteRepository<Showtime>
    {
        /// <summary>
        /// Crée une nouvelle séance (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="showtime">La séance à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task CreateSessionAsync(Showtime showtime);

        /// <summary>
        /// Met à jour les informations d'une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="showtime">La séance à mettre à jour.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateSessionAsync(Showtime showtime);

        /// <summary>
        /// Supprime une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task DeleteSessionAsync(int sessionId);

        Task<List<Showtime>> GetAllShowtimesAsync();


        Task<List<Showtime>> GetUpcomingShowtimesAsync();
    }


    public class ShowtimeRepository : EFRepository<Showtime>, IShowtimeRepository
    {
        private readonly DbContext _context;

        public ShowtimeRepository(DbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Crée une nouvelle séance (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="showtime">La séance à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task CreateSessionAsync(Showtime showtime)
        {
            if (showtime == null)
            {
                throw new ArgumentNullException(nameof(showtime));
            }

            showtime.CreatedAt = DateTime.UtcNow;
            showtime.UpdatedAt = DateTime.UtcNow;

            _context.Set<Showtime>().Add(showtime);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Met à jour les informations d'une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="showtime">La séance à mettre à jour.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task UpdateSessionAsync(Showtime showtime)
        {
            if (showtime == null)
            {
                throw new ArgumentNullException(nameof(showtime));
            }

            showtime.UpdatedAt = DateTime.UtcNow;

            _context.Set<Showtime>().Update(showtime);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Supprime une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task DeleteSessionAsync(int sessionId)
        {
            var showtime = await _context.Set<Showtime>().FindAsync(sessionId);
            if (showtime == null)
            {
                throw new ArgumentException("Séance non trouvée.");
            }

            _context.Set<Showtime>().Remove(showtime);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Showtime>> GetAllShowtimesAsync()
        {
            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .Include(s => s.Theater)
                .ToListAsync();
        }

        public async Task<List<Showtime>> GetUpcomingShowtimesAsync()
        {
            var today = DateTime.Today;

            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .Include(s => s.Theater)
                .Where(s => s.StartTime >= today)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

    }
}
