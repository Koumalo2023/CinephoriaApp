using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWorkPostgres _unitOfWorkPostgres;
        private readonly IUnitOfWorkMongoDb _unitOfWorkMongooBb;

        public ReservationService(IUnitOfWorkPostgres unitOfWorkPostgres, IUnitOfWorkMongoDb unitOfWorkMongooDb)
        {
            _unitOfWorkPostgres = unitOfWorkPostgres;
            _unitOfWorkMongooBb = unitOfWorkMongooDb;
        }

        /// <summary>
        /// Récupère la liste de tous les cinémas disponibles.
        /// </summary>
        public async Task<List<Cinema>> GetAllCinemasAsync()
        {
            return await _unitOfWorkPostgres.Cinemas.GetAllAsync();
        }

        /// <summary>
        /// Récupère tous les films disponibles dans un cinéma spécifique.
        /// </summary>
        public async Task<List<Movie>> GetMoviesByCinemaAsync(int cinemaId)
        {
            var cinema = await _unitOfWorkPostgres.Cinemas.GetByIdAsync(cinemaId);
            if (cinema == null) return null;

            var movies = await _unitOfWorkMongooBb.Showtimes
                .FindAsync(s => s.CinemaId == cinemaId && s.Movie != null);

            return movies.Select(s => s.Movie).Distinct().ToList();
        }

        /// <summary>
        /// Récupère toutes les réservations effectuées par un utilisateur donné.
        /// </summary>
        public async Task<List<ReservationDto>> GetReservationsByUserAsync(string userId)
        {
            var reservations = await _unitOfWorkPostgres.Reservations
                .FindAsync(r => r.AppUserId == userId);

            return reservations.Select(r => new ReservationDto
            {
                ReservationId = r.ReservationId,
                AppUserId = r.AppUserId,
                ShowtimeId = r.ShowtimeId,
                SeatNumbers = r.SeatNumbers,
                TotalPrice = r.TotalPrice,
                QrCode = r.QrCode,
                IsValidated = r.IsValidated,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList();
        }

        /// <summary>
        /// Récupère et génère le QR Code sous forme d'image pour une réservation donnée.
        /// </summary>
        /// <param name="reservationId">Identifiant de la réservation</param>
        /// <returns>Image du QR code au format Base64</returns>
        public async Task<GeneralServiceResponseData<byte[]>> GetReservationQrCodeAsync(int reservationId)
        {
            // Récupérer la réservation par son identifiant
            var reservation = await _unitOfWorkPostgres.Reservations.GetByIdAsync(reservationId);

            if (reservation == null)
            {
                return new GeneralServiceResponseData<byte[]>
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Réservation non trouvée."
                };
            }

            // Vérifier si le QR code est disponible
            if (string.IsNullOrEmpty(reservation.QrCode))
            {
                return new GeneralServiceResponseData<byte[]>
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Aucun QR code trouvé pour cette réservation."
                };
            }

            // Convertir le QR code stocké en Base64 (dans la base de données) en tableau de bytes
            var qrCodeBytes = Convert.FromBase64String(reservation.QrCode);

            return new GeneralServiceResponseData<byte[]>
            {
                IsSucceed = true,
                StatusCode = 200,
                Data = qrCodeBytes
            };
        }

        /// <summary>
        /// Crée une nouvelle réservation pour un utilisateur après avoir vérifié la disponibilité des places.
        /// </summary>
        public async Task<GeneralServiceResponseData<ReservationDto>> CreateReservationAsync(string userId, ReservationViewModel model)
        {
            // Récupérer la séance
            var showtime = await _unitOfWorkMongooBb.Showtimes.GetByIdAsync(model.ShowtimeId);
            if (showtime == null)
            {
                return new GeneralServiceResponseData<ReservationDto>
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "La séance spécifiée n'existe pas."
                };
            }

            // Vérifier la disponibilité des places
            var reservedSeats = await _unitOfWorkPostgres.Reservations
                .FindAsync(r => r.ShowtimeId == model.ShowtimeId && r.SeatNumbers.Intersect(model.SeatNumbers).Any());

            if (reservedSeats.Any())
            {
                return new GeneralServiceResponseData<ReservationDto>
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Les places demandées ne sont pas disponibles."
                };
            }

            // Générer le contenu du QR Code (par exemple, un identifiant unique ou des informations sur la réservation)
            var qrContent = $"ReservationId:{Guid.NewGuid()},User:{userId},Showtime:{model.ShowtimeId}";
            var qrCodeBase64 = GenerateQrCode(qrContent);

            // Créer la réservation
            var reservation = new Reservation
            {
                AppUserId = userId,
                ShowtimeId = model.ShowtimeId,
                SeatNumbers = model.SeatNumbers,
                TotalPrice = model.TotalPrice,
                QrCode = qrCodeBase64,
                Status = ReservationStatus.Confirmed,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Sauvegarder la réservation
            await _unitOfWorkPostgres.Reservations.CreateAsync(reservation);
            await _unitOfWorkPostgres.CompleteAsync();

            var reservationDto = new ReservationDto
            {
                ReservationId = reservation.ReservationId,
                AppUserId = reservation.AppUserId,
                ShowtimeId = reservation.ShowtimeId,
                SeatNumbers = reservation.SeatNumbers,
                TotalPrice = reservation.TotalPrice,
                QrCode = reservation.QrCode, // Inclure le QR code dans la réponse
                IsValidated = reservation.IsValidated,
                Status = reservation.Status,
                CreatedAt = reservation.CreatedAt,
                UpdatedAt = reservation.UpdatedAt
            };

            return new GeneralServiceResponseData<ReservationDto>
            {
                IsSucceed = true,
                StatusCode = 201,
                Data = reservationDto
            };
        }

        /// <summary>
        /// Valide une réservation à partir du QR Code.
        /// </summary>
        public async Task<GeneralServiceResponseData<object>> ValidateReservationAsync(string qrCode)
        {
            var reservation = await _unitOfWorkPostgres.Reservations
                .FindAsync(r => r.QrCode == qrCode && r.Status == ReservationStatus.Confirmed);

            var validReservation = reservation.FirstOrDefault();
            if (validReservation == null)
            {
                return new GeneralServiceResponseData<object>
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Aucune réservation valide trouvée pour ce QR Code."
                };
            }

            validReservation.IsValidated = true;
            validReservation.UpdatedAt = DateTime.Now;
            await _unitOfWorkPostgres.CompleteAsync();

            return new GeneralServiceResponseData<object>
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "Réservation validée avec succès."
            };
        }

        /// <summary>
        /// Récupère une réservation par son identifiant.
        /// </summary>
        public async Task<ReservationDto> GetReservationByIdAsync(int reservationId)
        {
            var reservation = await _unitOfWorkPostgres.Reservations.GetByIdAsync(reservationId);
            if (reservation == null) return null;

            return new ReservationDto
            {
                ReservationId = reservation.ReservationId,
                AppUserId = reservation.AppUserId,
                ShowtimeId = reservation.ShowtimeId,
                SeatNumbers = reservation.SeatNumbers,
                TotalPrice = reservation.TotalPrice,
                QrCode = reservation.QrCode,
                IsValidated = reservation.IsValidated,
                Status = reservation.Status,
                CreatedAt = reservation.CreatedAt,
                UpdatedAt = reservation.UpdatedAt
            };
        }

        /// <summary>
        /// Récupère les réservations pour une séance donnée.
        /// </summary>
        public async Task<List<ReservationDto>> GetReservationsByShowtimeAsync(string showtimeId)
        {
            var reservations = await _unitOfWorkPostgres.Reservations
                .FindAsync(r => r.ShowtimeId == showtimeId);

            return reservations.Select(r => new ReservationDto
            {
                ReservationId = r.ReservationId,
                AppUserId = r.AppUserId,
                ShowtimeId = r.ShowtimeId,
                SeatNumbers = r.SeatNumbers,
                TotalPrice = r.TotalPrice,
                QrCode = r.QrCode,
                IsValidated = r.IsValidated,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList();
        }

        /// <summary>
        /// Supprime une réservation existante et libère les places réservées.
        /// </summary>
        public async Task<GeneralServiceResponseData<object>> DeleteReservationAsync(int reservationId)
        {
            var reservation = await _unitOfWorkPostgres.Reservations.GetByIdAsync(reservationId);
            if (reservation == null)
            {
                return new GeneralServiceResponseData<object>
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "La réservation spécifiée n'existe pas."
                };
            }

            _unitOfWorkPostgres.Reservations.DeleteAsync(reservation);
            await _unitOfWorkPostgres.CompleteAsync();

            return new GeneralServiceResponseData<object>
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "La réservation a été supprimée avec succès."
            };
        }



        private string GenerateQrCode(string content)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

                // Utilisation de SkiaSharp pour créer une image QR Code
                using (var qrCode = new PngByteQRCode(qrCodeData)) // PngByteQRCode génère directement des PNG en bytes
                {
                    var qrCodeImage = qrCode.GetGraphic(20); // Génère un QR Code en tant que byte[]
                    return Convert.ToBase64String(qrCodeImage); // Retourne le QR Code sous forme de base64 pour le stockage
                }
            }
        }
    }

}
