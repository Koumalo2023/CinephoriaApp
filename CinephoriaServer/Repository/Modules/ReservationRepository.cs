using CinephoriaBackEnd.Data;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository.EntityFramwork;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Repository
{
    public interface IReservationRepository : IReadRepository<Reservation>, IWriteRepository<Reservation>
    {
        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances.</returns>
        Task<List<Showtime>> GetMovieSessionsAsync(int movieId);

        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles.</returns>
        Task<List<Seat>> GetAvailableSeatsAsync(int showtimeId);

        /// <summary>
        /// Calcule le prix total d'une réservation en fonction de la séance et des sièges sélectionnés.
        /// </summary>
        /// <param name="showtime">La séance pour laquelle la réservation est faite.</param>
        /// <param name="seats">La liste des sièges sélectionnés.</param>
        /// <returns>Le prix total de la réservation.</returns>
        Task<decimal> CalculateReservationPriceAsync(Showtime showtime, List<Seat> seats);

        /// <summary>
        /// Bloque des sièges pour une réservation en attente.
        /// </summary>
        /// <param name="showtime">La séance concernée.</param>
        /// <param name="seats">La liste des sièges à bloquer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task HoldSeatsAsync(Showtime showtime, List<Seat> seats);

        /// <summary>
        /// Récupère la liste des réservations d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une liste de réservations.</returns>
        Task<List<Reservation>> GetUserReservationsAsync(string userId);

        /// <summary>
        /// Crée une nouvelle réservation pour une séance spécifique.
        /// </summary>
        /// <param name="reservation">La réservation à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task CreateReservationAsync(Reservation reservation);

        /// <summary>
        /// Confirme une réservation après avoir bloqué des sièges.
        /// </summary>
        /// <param name="reservation">La réservation à confirmer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task ConfirmReservationAsync(Reservation reservation);

        /// <summary>
        /// Annule une réservation en fonction de son identifiant.
        /// </summary>
        /// <param name="reservationId">L'identifiant de la réservation à annuler.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task CancelReservationAsync(int reservationId);
    }


    public class ReservationRepository : EFRepository<Reservation>, IReservationRepository
    {
        private readonly DbContext _context;
        private readonly QRCodeService _qrCodeService;

        public ReservationRepository(DbContext context, QRCodeService qrCodeService) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _qrCodeService = qrCodeService ?? throw new ArgumentNullException(nameof(qrCodeService));
        }

        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances.</returns>
        public async Task<List<Showtime>> GetMovieSessionsAsync(int movieId)
        {
            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .Where(s => s.MovieId == movieId)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="showtimeId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles.</returns>
        public async Task<List<Seat>> GetAvailableSeatsAsync(int showtimeId)
        {
            var showtime = await _context.Set<Showtime>()
                .Include(s => s.Theater)
                .ThenInclude(t => t.Seats)
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId);

            if (showtime == null)
            {
                throw new ArgumentException("Séance non trouvée.");
            }

            return showtime.Theater.Seats
                .Where(s => s.IsAvailable)
                .ToList();
        }

        /// <summary>
        /// Calcule le prix total d'une réservation en fonction de la séance et des sièges sélectionnés.
        /// </summary>
        /// <param name="showtime">La séance pour laquelle la réservation est faite.</param>
        /// <param name="seats">La liste des sièges sélectionnés.</param>
        /// <returns>Le prix total de la réservation.</returns>
        public async Task<decimal> CalculateReservationPriceAsync(Showtime showtime, List<Seat> seats)
        {
            if (showtime == null || seats == null || !seats.Any())
            {
                throw new ArgumentException("La séance et les sièges doivent être valides.");
            }

            // Prix de base de la séance
            decimal basePrice = showtime.Price;

            // Appliquer les ajustements de prix (promotions, majorations, etc.)
            if (showtime.IsPromotion)
            {
                basePrice *= 0.9m; // 10% de réduction
            }

            // Calculer le prix total en fonction du nombre de sièges
            decimal totalPrice = basePrice * seats.Count;

            return totalPrice;
        }

        /// <summary>
        /// Bloque des sièges pour une réservation en attente.
        /// </summary>
        /// <param name="showtime">La séance concernée.</param>
        /// <param name="seats">La liste des sièges à bloquer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task HoldSeatsAsync(Showtime showtime, List<Seat> seats)
        {
            if (showtime == null || seats == null || !seats.Any())
            {
                throw new ArgumentException("La séance et les sièges doivent être valides.");
            }

            // Marquer les sièges comme non disponibles
            foreach (var seat in seats)
            {
                seat.IsAvailable = false;
                _context.Set<Seat>().Update(seat);
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Récupère la liste des réservations d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une liste de réservations.</returns>
        public async Task<List<Reservation>> GetUserReservationsAsync(string userId)
        {
            return await _context.Set<Reservation>()
                .Include(r => r.Showtime)
                .Include(r => r.Seats)
                .Where(r => r.AppUserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Crée une nouvelle réservation pour une séance spécifique.
        /// </summary>
        /// <param name="reservation">La réservation à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task CreateReservationAsync(Reservation reservation)
        {
            if (reservation == null)
            {
                throw new ArgumentNullException(nameof(reservation));
            }

            // Vérifier que la séance et les sièges sont valides
            if (reservation.Showtime == null || reservation.Seats == null || !reservation.Seats.Any())
            {
                throw new ArgumentException("La réservation doit inclure une séance et au moins un siège.");
            }

            // Générer le QRCode
            byte[] qrCodeBytes = _qrCodeService.GenerateQRCode(reservation);

            // Convertir le QRCode en base64 pour le stocker dans la base de données
            reservation.QrCode = Convert.ToBase64String(qrCodeBytes);

            // Enregistrer la réservation
            _context.Set<Reservation>().Add(reservation);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Confirme une réservation après avoir bloqué des sièges.
        /// </summary>
        /// <param name="reservation">La réservation à confirmer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task ConfirmReservationAsync(Reservation reservation)
        {
            if (reservation == null)
            {
                throw new ArgumentNullException(nameof(reservation));
            }

            // Marquer la réservation comme confirmée
            reservation.Status = ReservationStatus.Confirmed;

            // Enregistrer les modifications
            _context.Set<Reservation>().Update(reservation);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Annule une réservation en fonction de son identifiant.
        /// </summary>
        /// <param name="reservationId">L'identifiant de la réservation à annuler.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task CancelReservationAsync(int reservationId)
        {
            var reservation = await _context.Set<Reservation>()
                .Include(r => r.Seats)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation == null)
            {
                throw new ArgumentException("Réservation non trouvée.");
            }

            // Libérer les sièges réservés
            foreach (var seat in reservation.Seats)
            {
                seat.IsAvailable = true;
                _context.Set<Seat>().Update(seat);
            }

            // Supprimer la réservation
            _context.Set<Reservation>().Remove(reservation);
            await _context.SaveChangesAsync();
        }
    }
}
