using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository.EntityFramwork;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.Repository
{
    public interface ISeatRepository : IReadRepository<Seat>, IWriteRepository<Seat>
    {
        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles.</returns>
        Task<List<Seat>> GetAvailableSeatsAsync(int sessionId);

        /// <summary>
        /// Ajoute un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="seatNumber">Le numéro du siège à ajouter.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task AddHandicapSeatAsync(int theaterId, string seatNumber);

        /// <summary>
        /// Supprime un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="seatNumber">Le numéro du siège à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task RemoveHandicapSeatAsync(int theaterId, string seatNumber);
    }
    public class SeatRepository : EFRepository<Seat>, ISeatRepository
    {
        private readonly DbContext _context;

        public SeatRepository(DbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles.</returns>
        public async Task<List<Seat>> GetAvailableSeatsAsync(int sessionId)
        {
            // Récupérer les sièges réservés pour cette séance
            var reservedSeats = await _context.Set<Reservation>()
                .Where(r => r.ShowtimeId == sessionId)
                .SelectMany(r => r.Seats)
                .Select(s => s.SeatId)
                .ToListAsync();

            // Récupérer la salle associée à cette séance
            var showtime = await _context.Set<Showtime>()
                .Include(s => s.Theater)
                .FirstOrDefaultAsync(s => s.ShowtimeId == sessionId);

            if (showtime == null)
            {
                throw new ArgumentException("Showtime not found.");
            }

            // Récupérer les sièges disponibles dans la salle
            return await _context.Set<Seat>()
                .Where(s => s.TheaterId == showtime.TheaterId && !reservedSeats.Contains(s.SeatId) && s.IsAvailable)
                .ToListAsync();
        }

        /// <summary>
        /// Ajoute un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="seatNumber">Le numéro du siège à ajouter.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task AddHandicapSeatAsync(int theaterId, string seatNumber)
        {
            var seat = new Seat
            {
                TheaterId = theaterId,
                SeatNumber = seatNumber,
                IsAccessible = true, // Marquer comme accessible
                IsAvailable = true,  // Disponible par défaut
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Set<Seat>().Add(seat);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Supprime un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="seatNumber">Le numéro du siège à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task RemoveHandicapSeatAsync(int theaterId, string seatNumber)
        {
            var seat = await _context.Set<Seat>()
                .FirstOrDefaultAsync(s => s.TheaterId == theaterId && s.SeatNumber == seatNumber && s.IsAccessible);

            if (seat == null)
            {
                throw new ArgumentException("Handicap seat not found.");
            }

            _context.Set<Seat>().Remove(seat);
            await _context.SaveChangesAsync();
        }
    }
}
