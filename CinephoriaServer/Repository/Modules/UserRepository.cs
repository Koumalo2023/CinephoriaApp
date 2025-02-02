using CinephoriaBackEnd.Data;
using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository.EntityFramwork;
using Microsoft.EntityFrameworkCore;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Repository
{
    public interface IUserRepository : IReadRepository<AppUser>, IWriteRepository<AppUser>
    {
        /// <summary>
        /// Récupère le profil d'un utilisateur en fonction de son identifiant.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Le profil de l'utilisateur.</returns>
        Task<AppUser> GetUserProfileAsync(string AppUserId);

        /// <summary>
        /// Récupère la liste des commandes (réservations) d'un utilisateur.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une liste de réservations.</returns>
        Task<List<Reservation>> GetUserOrdersAsync(string AppUserId);

        /// <summary>
        /// Récupère le profil d'un employé avec la liste des incidents qu'il a gérés.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé.</param>
        /// <returns>Le profil de l'employé avec la liste des incidents.</returns>
        Task<AppUser> GetEmployeeProfileAsync(string employeeId);

    }

    public class UserRepository : EFRepository<AppUser>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context) { }

        /// <summary>
        /// Récupère le profil d'un utilisateur en fonction de son identifiant.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Le profil de l'utilisateur.</returns>
        public async Task<AppUser> GetUserProfileAsync(string AppUserId)
        {
            return await _context.Set<AppUser>()
                .Include(u => u.Reservations)
                .Include(u => u.MovieRatings)
                .FirstOrDefaultAsync(u => u.Id == AppUserId);
        }

        /// <summary>
        /// Récupère la liste des commandes (réservations) d'un utilisateur.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une liste de réservations.</returns>
        public async Task<List<Reservation>> GetUserOrdersAsync(string AppUserId)
        {
            return await _context.Set<Reservation>()
                .Where(r => r.AppUserId == AppUserId)
                .Include(r => r.Showtime)
                .ThenInclude(s => s.Movie)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère le profil d'un employé avec la liste des incidents qu'il a gérés.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé.</param>
        /// <returns>Le profil de l'employé avec la liste des incidents.</returns>
        public async Task<AppUser> GetEmployeeProfileAsync(string employeeId)
        {
            // Récupérer l'utilisateur avec les incidents signalés et les informations du théâtre
            var employee = await _context.Set<AppUser>()
                .Include(u => u.ReportedIncidents)
                .Include(u => u.ResolvedByIncidents)
                    .ThenInclude(i => i.Theater)
                .FirstOrDefaultAsync(u => u.Id == employeeId && (u.Role == UserRole.Employee || u.Role == UserRole.Admin));

            if (employee == null)
            {
                throw new NotFoundException("Utilisateur non trouvé ou n'est pas un employé ou un administrateur.");
            }

            return employee;
        }
    }
}
