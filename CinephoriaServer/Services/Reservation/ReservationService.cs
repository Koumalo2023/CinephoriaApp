﻿using AutoMapper;
using CinephoriaBackEnd.Data;
using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly CinephoriaDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(
            IUnitOfWorkPostgres unitOfWork,
            IMapper mapper,
            ILogger<ReservationService> logger, CinephoriaDbContext context)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException("context");
        }

        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        public async Task<List<ShowtimeDto>> GetMovieSessionsAsync(int movieId)
        {
            var showtimes = await _unitOfWork.Reservations.GetMovieSessionsAsync(movieId);
            return _mapper.Map<List<ShowtimeDto>>(showtimes);
        }


        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        public async Task<List<SeatDto>> GetAvailableSeatsAsync(int showtimeId)
        {
            var seats = await _unitOfWork.Seats.GetAvailableSeatsAsync(showtimeId);
            return _mapper.Map<List<SeatDto>>(seats);
        }


        /// <summary>
        /// Calcule le prix total d'une réservation en fonction de la séance et des sièges sélectionnés.
        /// </summary>
        public async Task<decimal> CalculateReservationPriceAsync(int showtimeId, List<string> seatNumbers)
        {
            var showtime = await _unitOfWork.Showtimes.GetByIdAsync(showtimeId);
            var seats = await _unitOfWork.Seats.GetSeatsByNumbersAsync(showtimeId, seatNumbers);

            if (showtime == null || seats == null || !seats.Any())
            {
                throw new ApiException("La séance ou les sièges sélectionnés sont invalides.", StatusCodes.Status400BadRequest);
            }

            return await _unitOfWork.Reservations.CalculateReservationPriceAsync(showtime, seats);
        }


        /// <summary>
        /// Bloque des sièges pour une réservation en attente.
        /// </summary>
        public async Task HoldSeatsAsync(int showtimeId, List<string> seatNumbers)
        {
            var showtime = await _unitOfWork.Showtimes.GetByIdAsync(showtimeId);
            var seats = await _unitOfWork.Seats.GetSeatsByNumbersAsync(showtimeId, seatNumbers);

            if (showtime == null || seats == null || !seats.Any())
            {
                throw new ApiException("La séance ou les sièges sélectionnés sont invalides.", StatusCodes.Status400BadRequest);
            }

            await _unitOfWork.Reservations.HoldSeatsAsync(showtime, seats);
            _logger.LogInformation("Sièges bloqués avec succès pour la séance avec l'ID {ShowtimeId}.", showtimeId);
        }

        /// <summary>
        /// Valide un QRCode scanné pour une réservation.
        /// </summary>
        /// <param name="qrCodeData">Les données du QRCode scanné.</param>
        /// <returns>True si la validation est réussie, sinon False.</returns>
        public async Task<bool> ValidatedSession(string qrCodeData)
        {
            if (string.IsNullOrEmpty(qrCodeData))
            {
                throw new ArgumentException("Les données du QRCode sont invalides.");
            }

            // Décoder les données du QRCode
            var qrCodeParts = qrCodeData.Split(';');
            if (qrCodeParts.Length != 3)
            {
                throw new ArgumentException("Format du QRCode invalide.");
            }

            // Extraire les informations du QRCode
            var reservationIdPart = qrCodeParts[0].Split(':');
            var showtimeIdPart = qrCodeParts[1].Split(':');
            var userIdPart = qrCodeParts[2].Split(':');

            if (reservationIdPart.Length != 2 || showtimeIdPart.Length != 2 || userIdPart.Length != 2)
            {
                throw new ArgumentException("Format du QRCode invalide.");
            }

            int reservationId = int.Parse(reservationIdPart[1]);
            int showtimeId = int.Parse(showtimeIdPart[1]);
            string userId = userIdPart[1];

            // Récupérer la réservation correspondante avec une requête personnalisée
            var reservation = await _context.Reservations
                .Include(res => res.Showtime) 
                .FirstOrDefaultAsync(res => res.ReservationId == reservationId);

            if (reservation == null)
            {
                throw new ArgumentException("Réservation non trouvée.");
            }

            // Vérifier si la séance est encore valide (par exemple, la séance n'a pas encore commencé)
            if (reservation.Showtime.StartTime < DateTime.UtcNow)
            {
                throw new InvalidOperationException("La séance a déjà commencé.");
            }

            // Marquer la réservation comme validée
            reservation.IsValidated = true;

            // Mettre à jour la réservation dans la base de données
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Réservation avec l'ID {ReservationId} validée avec succès.", reservationId);
            return true;
        }

    }

}
