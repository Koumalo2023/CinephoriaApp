using CinephoriaBackEnd.Data;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace CinephoriaServer.Repository
{
    public interface IReservationRepository
    {
        /// <summary>
        /// Récupère la liste de tous les cinémas disponibles.
        /// </summary>
        /// <returns>Une liste de cinémas.</returns>
        Task<List<Cinema>> GetCinemasAsync();

        /// <summary>
        /// Récupère tous les films disponibles dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">Identifiant du cinéma.</param>
        /// <returns>Une liste de films disponibles dans le cinéma spécifié.</returns>
        Task<List<Movie>> GetMoviesByCinemaAsync(int cinemaId);

        /// <summary>
        /// Récupère toutes les réservations effectuées par un utilisateur donné.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Liste des réservations pour l'utilisateur donné.</returns>
        Task<List<Reservation>> GetReservationsByUserIdAsync(string userId);

        /// <summary>
        /// Récupère les séances à venir pour lesquelles l'utilisateur a réservé des billets.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une liste des séances à venir avec les informations des réservations de l'utilisateur.</returns>
        Task<List<Showtime>> GetUpcomingReservationsByUserAsync(string userId);

        /// <summary>
        /// Crée une nouvelle réservation pour un utilisateur authentifié.
        /// Vérifie les disponibilités avant de confirmer la réservation.
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur.</param>
        /// <param name="showtimeId">Identifiant de la séance.</param>
        /// <param name="seatNumbers">Nombre de places à réserver.</param>
        /// <returns>La réservation créée.</returns>
        Task<Reservation> CreateReservationAsync(string userId, int showtimeId, string[] seatNumbers);

        /// <summary>
        /// Récupère le QR Code associé à une réservation pour un utilisateur mobile.
        /// </summary>
        /// <param name="reservationId">Identifiant de la réservation.</param>
        /// <returns>Le QR Code sous forme de chaîne de caractères.</returns>
        Task<string> GetReservationQRCodeAsync(int reservationId);

        /// <summary>
        /// Récupère une réservation par son identifiant.
        /// </summary>
        /// <param name="reservationId">Identifiant de la réservation.</param>
        /// <returns>La réservation correspondante, avec les détails de la séance et du film.</returns>
        Task<Reservation> GetReservationByIdAsync(int reservationId);

        /// <summary>
        /// Valide une réservation à partir du QR Code.
        /// </summary>
        /// <param name="reservationId">L'identifiant de la réservation à valider.</param>
        /// <returns>True si la réservation a été validée avec succès, sinon False.</returns>
        Task<bool> ValidateReservationQRCodeAsync(int reservationId);

        /// <summary>
        /// Met à jour une réservation existante en modifiant le nombre de places réservées.
        /// Vérifie que les nouvelles places demandées sont disponibles.
        /// </summary>
        /// <param name="reservationId">Identifiant de la réservation.</param>
        /// <param name="seatNumbers">Nouveau nombre de places à réserver.</param>
        /// <returns>La réservation mise à jour.</returns>
        Task<Reservation> UpdateReservationAsync(int reservationId, string[] seatNumbers);

        /// <summary>
        /// Supprime une réservation existante et libère les places réservées.
        /// </summary>
        /// <param name="reservationId">Identifiant de la réservation à supprimer.</param>
        Task DeleteReservationAsync(int reservationId);
    }


    public class ReservationRepository : IReservationRepository
    {
        private readonly CinephoriaDbContext _context;
        private readonly IMongoCollection<Movie> _movieCollection;
        private readonly IMongoCollection<Showtime> _showtimeCollection;

        public ReservationRepository(CinephoriaDbContext context, IMongoDatabase mongoDatabase)
        {
            _context = context;
            _movieCollection = mongoDatabase.GetCollection<Movie>("Movies");
            _showtimeCollection = mongoDatabase.GetCollection<Showtime>("Showtimes");
        }

        /// <summary>
        /// Récupère la liste de tous les cinémas disponibles.
        /// </summary>
        /// <returns>Une liste de cinémas.</returns>
        public async Task<List<Cinema>> GetCinemasAsync()
        {
            return await _context.Cinemas.ToListAsync();
        }

        /// <summary>
        /// Récupère tous les films disponibles dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">Identifiant du cinéma.</param>
        /// <returns>Une liste de films disponibles dans le cinéma spécifié.</returns>
        public async Task<List<Movie>> GetMoviesByCinemaAsync(int cinemaId)
        {
            // Vous devrez peut-être mettre en place une logique supplémentaire pour associer les films à un cinéma.
            return await _movieCollection.Find(m => m.CinemaId == cinemaId).ToListAsync();
        }

        /// <summary>
        /// Récupère toutes les réservations effectuées par un utilisateur donné.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Liste des réservations pour l'utilisateur donné.</returns>
        public async Task<List<Reservation>> GetReservationsByUserIdAsync(string userId)
        {
            return await _context.Reservations
                .Where(r => r.AppUserId == userId)
                .ToListAsync();
        }

        public async Task<List<Showtime>> GetUpcomingReservationsByUserAsync(string userId)
        {
            var today = DateTime.Now.Date;

            // réservations de l'utilisateur à partir de PostgreSQL
            var reservations = await _context.Reservations
                .Where(r => r.AppUserId == userId)
                .ToListAsync();

            // IDs des séances réservées et les convertir en chaînes si nécessaire
            var showtimeIds = reservations
                .Select(r => r.ShowtimeId.ToString())
                .ToList();

            // Si aucun ID de séance n'a été trouvé, retourner une liste vide
            if (!showtimeIds.Any())
            {
                return new List<Showtime>();
            }

            // filtre pour récupérer les séances correspondantes à partir de MongoDB
            var filter = Builders<Showtime>.Filter.And(
                Builders<Showtime>.Filter.In(s => s.Id, showtimeIds),
                Builders<Showtime>.Filter.Gt(s => s.StartTime, today)
            );

            // Récupérer les séances futures depuis MongoDB
            var upcomingShowtimes = await _showtimeCollection.Find(filter).ToListAsync();

            return upcomingShowtimes;
        }

        /// <summary>
        /// Crée une nouvelle réservation pour un utilisateur authentifié.
        /// Vérifie les disponibilités avant de confirmer la réservation.
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur.</param>
        /// <param name="showtimeId">Identifiant de la séance.</param>
        /// <param name="seatNumbers">Nombre de places à réserver.</param>
        /// <returns>La réservation créée.</returns>
        public async Task<Reservation> CreateReservationAsync(string userId, int showtimeId, string[] seatNumbers)
        {
            // Vérifiez la disponibilité des sièges ici (logique à ajouter selon votre modèle de données)

            var reservation = new Reservation
            {
                AppUserId = userId,
                ShowtimeId = showtimeId,
                SeatNumbers = seatNumbers,
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        /// <summary>
        /// Récupère le QR Code associé à une réservation pour un utilisateur mobile.
        /// </summary>
        /// <param name="reservationId">Identifiant de la réservation.</param>
        /// <returns>Le QR Code sous forme de chaîne de caractères.</returns>
        public async Task<string> GetReservationQRCodeAsync(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null)
            {
                throw new KeyNotFoundException("Réservation non trouvée.");
            }

            // Logique pour générer un QR code à partir de l'identifiant de la réservation
            return GenerateQRCode(reservation);
        }

        private string GenerateQRCode(Reservation reservation)
        {
            // Implémentation de la génération de QR Code (utiliser une bibliothèque tierce comme ZXing.Net)
            return "QRCodeString"; // Remplacez par votre implémentation de QR Code
        }

        /// <summary>
        /// Récupère une réservation par son identifiant.
        /// </summary>
        /// <param name="reservationId">Identifiant de la réservation.</param>
        /// <returns>La réservation correspondante, avec les détails de la séance et du film.</returns>
        public async Task<Reservation> GetReservationByIdAsync(int reservationId)
        {
            return await _context.Reservations.FindAsync(reservationId);
        }

        public async Task<bool> ValidateReservationQRCodeAsync(int reservationId)
        {
            // Rechercher la réservation dans PostgreSQL
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation == null)
                return false;

            // Logique de validation du QR Code
            reservation.IsValidated = true;
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Met à jour une réservation existante en modifiant le nombre de places réservées.
        /// Vérifie que les nouvelles places demandées sont disponibles.
        /// </summary>
        /// <param name="reservationId">Identifiant de la réservation.</param>
        /// <param name="seatNumbers">Nouveau nombre de places à réserver.</param>
        /// <returns>La réservation mise à jour.</returns>
        public async Task<Reservation> UpdateReservationAsync(int reservationId, string[] seatNumbers)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null)
            {
                throw new KeyNotFoundException("Réservation non trouvée.");
            }

            // Logique de vérification de disponibilité
            reservation.SeatNumbers = seatNumbers;
            await _context.SaveChangesAsync();
            return reservation;
        }

        /// <summary>
        /// Supprime une réservation existante et libère les places réservées.
        /// </summary>
        /// <param name="reservationId">Identifiant de la réservation à supprimer.</param>
        public async Task DeleteReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null)
            {
                throw new KeyNotFoundException("Réservation non trouvée.");
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }
    }
}
