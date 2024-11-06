using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Repository;
using MongoDB.Bson;

namespace CinephoriaServer.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IUnitOfWorkMongoDb _unitOfWork;
        private readonly IUnitOfWorkPostgres _unitOfWorkPostgres;

        public ShowtimeService(IUnitOfWorkPostgres unitOfWorkPostgres, IUnitOfWorkMongoDb unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkPostgres = unitOfWorkPostgres;
        }

        public async Task<GeneralServiceResponseData<object>> CreateShowtimeAsync(ShowtimeViewModel model)
        {
            // Vérifier si le film existe
            if (!await _unitOfWork.ExistsAsync<Movie>(model.MovieId))
            {
                return new GeneralServiceResponseData<object>
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Le film spécifié n'existe pas."
                };
            }

            // Conversion de TheaterId en ObjectId (si nécessaire)
            if (!ObjectId.TryParse(model.TheaterId, out ObjectId theaterObjectId))
            {
                return new GeneralServiceResponseData<object>
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "L'identifiant de la salle spécifiée est invalide."
                };
            }

            // Vérifier si la salle existe avec l'ObjectId converti
            if (!await _unitOfWork.ExistsAsync<Theater>(theaterObjectId.ToString()))
            {
                return new GeneralServiceResponseData<object>
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "La salle spécifiée n'existe pas."
                };
            }

            // Créer l'objet Showtime
            var showtime = new Showtime
            {
                Id = ObjectId.GenerateNewId().ToString(),
                MovieId = model.MovieId,
                TheaterId = theaterObjectId.ToString(),
                CinemaId = model.CinemaId,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                ProjectionQuality = model.ProjectionQuality,
                AvailableSeats = model.AvailableSeats,
                Price = model.Price
            };

            // Ajouter à la base de données
            await _unitOfWork.Showtimes.AddAsync(showtime);
            await _unitOfWork.SaveChangesAsync();

            // Retourner une réponse de succès
            var showtimeDto = new ShowtimeDto
            {
                Id = showtime.Id.ToString(),
                MovieId = showtime.MovieId,
                TheaterId = showtime.TheaterId,
                CinemaId = showtime.CinemaId,
                StartTime = showtime.StartTime,
                EndTime = showtime.EndTime,
                ProjectionQuality = showtime.ProjectionQuality.ToString(),
                AvailableSeats = showtime.AvailableSeats,
                Price = showtime.Price
            };

            return new GeneralServiceResponseData<object>
            {
                IsSucceed = true,
                StatusCode = 201,
                Message = "La séance a été créée avec succès.",
                Data = showtimeDto
            };
        }

        public async Task<GeneralServiceResponseData<List<Showtime>>> GetShowtimesForMovieInCinemaAsync(string movieId, int cinemaId)
        {
            var showtimes = await _unitOfWork.Showtimes
                .FindAsync(s => s.MovieId == movieId && s.CinemaId == cinemaId);

            if (showtimes == null || !showtimes.Any())
            {
                return new GeneralServiceResponseData<List<Showtime>>
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Aucune séance disponible pour ce film dans le cinéma spécifié."
                };
            }

            return new GeneralServiceResponseData<List<Showtime>>
            {
                IsSucceed = true,
                StatusCode = 200,
                Data = showtimes.ToList()
            };
        }

        public async Task<GeneralServiceResponseData<List<Showtime>>> GetShowtimesForAuthenticatedUserAsync(string userId)
        {
            var reservations = await _unitOfWorkPostgres.Reservations.FindAsync(r => r.AppUserId == userId);

            if (reservations == null || !reservations.Any())
            {
                return new GeneralServiceResponseData<List<Showtime>>
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Aucune séance trouvée pour cet utilisateur."
                };
            }

            var showtimeIds = reservations.Select(r => r.ShowtimeId).Distinct().ToList();
            var showtimes = await _unitOfWork.Showtimes.FindAsync(s => showtimeIds.Contains(s.Id));

            return new GeneralServiceResponseData<List<Showtime>>
            {
                IsSucceed = true,
                StatusCode = 200,
                Data = showtimes.ToList()
            };
        }

        public async Task<GeneralServiceResponseData<object>> UpdateShowtimeAsync(string showtimeId, ShowtimeViewModel model)
        {
            var existingShowtime = await _unitOfWork.Showtimes.GetByIdAsync(showtimeId);

            if (existingShowtime == null)
            {
                return new GeneralServiceResponseData<object>
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "La séance spécifiée n'existe pas."
                };
            }

            // Vérifier les conflits d'horaires pour les autres séances
            var overlappingShowtimes = await _unitOfWork.Showtimes
                .FindAsync(s => s.TheaterId == model.TheaterId
                                && s.Id != showtimeId
                                && ((model.StartTime >= s.StartTime && model.StartTime < s.EndTime)
                                || (model.EndTime > s.StartTime && model.EndTime <= s.EndTime)));

            if (overlappingShowtimes.Any())
            {
                return new GeneralServiceResponseData<object>
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Les horaires de la séance chevauchent une autre séance."
                };
            }

            // Mettre à jour la séance
            existingShowtime.StartTime = model.StartTime;
            existingShowtime.EndTime = model.EndTime;
            existingShowtime.ProjectionQuality = model.ProjectionQuality;
            existingShowtime.AvailableSeats = model.AvailableSeats;
            existingShowtime.Price = model.Price;

            await _unitOfWork.Showtimes.UpdateAsync(existingShowtime);
            await _unitOfWork.SaveChangesAsync();

            return new GeneralServiceResponseData<object>
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "La séance a été modifiée avec succès."
            };
        }

        public async Task<GeneralServiceResponseData<object>> DeleteShowtimeAsync(string showtimeId)
        {
            var showtime = await _unitOfWork.Showtimes.GetByIdAsync(showtimeId);
            if (showtime == null)
            {
                return new GeneralServiceResponseData<object>
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "La séance spécifiée n'existe pas."
                };
            }

            await _unitOfWork.Showtimes.DeleteAsync(showtimeId);
            await _unitOfWork.SaveChangesAsync();

            return new GeneralServiceResponseData<object>
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "La séance a été supprimée avec succès."
            };
        }


        public async Task<GeneralServiceResponseData<List<Showtime>>> GetUserShowtimesForTodayAndFutureAsync(string userId)
        {
            var today = DateTime.Now.Date;

            // Récupérer les réservations de l'utilisateur à partir d'aujourd'hui
            var reservations = await _unitOfWorkPostgres.Reservations.FindAsync(r => r.AppUserId == userId && r.Showtime.StartTime >= today);

            if (reservations == null || !reservations.Any())
            {
                return new GeneralServiceResponseData<List<Showtime>>
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Aucune séance trouvée pour l'utilisateur à partir d'aujourd'hui."
                };
            }

            // Récupérer les séances correspondantes
            var showtimeIds = reservations.Select(r => r.ShowtimeId).Distinct().ToList();
            var showtimes = await _unitOfWork.Showtimes.FindAsync(s => showtimeIds.Contains(s.Id));

            return new GeneralServiceResponseData<List<Showtime>>
            {
                IsSucceed = true,
                StatusCode = 200,
                Data = showtimes.ToList()
            };
        }

        public async Task<GeneralServiceResponseData<bool>> IsShowtimeOverlappingAsync(string theaterId, DateTime startTime, DateTime endTime, string? showtimeId = null)
        {
            var overlappingShowtimes = await _unitOfWork.Showtimes.FindAsync(s =>
                s.TheaterId == theaterId
                && (showtimeId == null || s.Id != showtimeId) // Exclure l'ID en cas de modification
                && ((startTime >= s.StartTime && startTime < s.EndTime) || (endTime > s.StartTime && endTime <= s.EndTime)));

            var isOverlapping = overlappingShowtimes.Any();

            return new GeneralServiceResponseData<bool>
            {
                IsSucceed = true,
                StatusCode = 200,
                Data = isOverlapping,
                Message = isOverlapping ? "Il existe un chevauchement d'horaires." : "Pas de chevauchement d'horaires."
            };
        }








    }
}
