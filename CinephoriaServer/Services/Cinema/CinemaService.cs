using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository;
using MongoDB.Bson;

namespace CinephoriaServer.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly IUnitOfWorkPostgres _unitOfWorkPostgres;
        private readonly IUnitOfWorkMongoDb _unitOfWorkMongoDb;

        public CinemaService(IUnitOfWorkPostgres unitOfWorkPostgres, IUnitOfWorkMongoDb unitOfWorkMongoDb)
        {
            _unitOfWorkPostgres = unitOfWorkPostgres;
            _unitOfWorkMongoDb = unitOfWorkMongoDb;
        }

        public async Task<GeneralServiceResponseData<object>> CreateCinemaAsync(CinemaViewModel cinemaViewModel)
        {
            // Création d'un objet cinéma à partir du ViewModel
            var cinema = new Cinema
            {
                Name = cinemaViewModel.Name,
                Address = cinemaViewModel.Address,
                PhoneNumber = cinemaViewModel.PhoneNumber,
                City = cinemaViewModel.City,
                Country = cinemaViewModel.Country,
                OpeningHours = cinemaViewModel.OpeningHours
            };

            // Ajouter le cinéma à la base de données
            await _unitOfWorkPostgres.Cinemas.CreateAsync(cinema);
            await _unitOfWorkPostgres.CompleteAsync();

            // Retourner une réponse de succès avec l'ID du cinéma créé
            return new GeneralServiceResponseData<object>
            {
                IsSucceed = true,
                StatusCode = 201,
                Message = "Cinéma créé avec succès.",
                Data = new { Id = cinema.CinemaId }
            };
        }

        public async Task<GeneralServiceResponse> UpdateCinemaAsync(int cinemaId, CinemaViewModel cinemaViewModel)
        {
            var existingCinema = await _unitOfWorkPostgres.Cinemas.GetByIdAsync(cinemaId);

            if (existingCinema == null)
            {
                return new GeneralServiceResponse { IsSucceed = false, StatusCode = 404, Message = "Cinéma non trouvé." };
            }

            existingCinema.Name = cinemaViewModel.Name ?? existingCinema.Name;
            existingCinema.Address = cinemaViewModel.Address ?? existingCinema.Address;
            existingCinema.PhoneNumber = cinemaViewModel.PhoneNumber ?? existingCinema.PhoneNumber;
            existingCinema.City = cinemaViewModel.City ?? existingCinema.City;
            existingCinema.Country = cinemaViewModel.Country ?? existingCinema.Country;
            existingCinema.OpeningHours = cinemaViewModel.OpeningHours ?? existingCinema.OpeningHours;

            _unitOfWorkPostgres.Cinemas.Update(existingCinema);
            await _unitOfWorkPostgres.CompleteAsync();

            return new GeneralServiceResponse { IsSucceed = true, StatusCode = 200, Message = "Cinéma mis à jour avec succès." };
        }

        public async Task<GeneralServiceResponse> DeleteCinemaAsync(int cinemaId)
        {
            var existingCinema = await _unitOfWorkPostgres.Cinemas.GetByIdAsync(cinemaId);

            if (existingCinema == null)
            {
                return new GeneralServiceResponse { IsSucceed = false, StatusCode = 404, Message = "Cinéma non trouvé." };
            }

            _unitOfWorkPostgres.Cinemas.Delete(existingCinema);
            await _unitOfWorkPostgres.CompleteAsync();

            return new GeneralServiceResponse { IsSucceed = true, StatusCode = 200, Message = "Cinéma supprimé avec succès." };
        }

        public async Task<IEnumerable<CinemaDto>> GetAllCinemasAsync()
        {
            var cinemas = await _unitOfWorkPostgres.Cinemas.GetAllAsync();
            return cinemas.Select(c => new CinemaDto
            {
                Id = c.CinemaId,
                Name = c.Name,
                Address = c.Address,
                PhoneNumber = c.PhoneNumber,
                City = c.City,
                Country = c.Country
            }).ToList();
        }

        public async Task<GeneralServiceResponseData<object>> CreateTheaterForCinemaAsync(TheaterViewModel theaterViewModel)
        {
            // Vérifier si le cinéma existe dans PostgreSQL
            var cinemaExists = await _unitOfWorkPostgres.Cinemas.GetByIdAsync(theaterViewModel.CinemaId);
            if (cinemaExists == null)
            {
                return new GeneralServiceResponseData<object>
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Cinéma non trouvé."
                };
            }

            

            // Création d'une salle de projection à partir du ViewModel (MongoDB)
            var theater = new Theater
            {
                Id = ObjectId.GenerateNewId(),
                Name = theaterViewModel.Name,
                SeatCount = theaterViewModel.SeatCount,
                CinemaId = theaterViewModel.CinemaId,
                IsOperational = theaterViewModel.IsOperational,
                ProjectionQuality = theaterViewModel.ProjectionQuality
            };

            // Ajouter la salle dans MongoDB
            await _unitOfWorkMongoDb.Theaters.AddAsync(theater);
            await _unitOfWorkMongoDb.SaveChangesAsync();

            var result = new TheaterDto
            {
                Id = theaterViewModel.Id.ToString(),
                Name = theaterViewModel.Name,
                SeatCount = theaterViewModel.SeatCount,
                CinemaId = theaterViewModel.CinemaId,
                IsOperational = theaterViewModel.IsOperational,
                ProjectionQuality = theaterViewModel.ProjectionQuality
            };

            // Retourner une réponse de succès
            return new GeneralServiceResponseData<object>
            {
                IsSucceed = true,
                StatusCode = 201,
                Message = "Salle de projection créée avec succès.",
                Data = result
            };
        }

        public async Task<GeneralServiceResponseData<object>> GetTheaterByIdAsync(string theaterId)
        {
            // Vérifier si la salle de cinéma existe dans MongoDB
            var theater = await _unitOfWorkMongoDb.Theaters.GetByIdAsync(theaterId);
            if (theater == null)
            {
                return new GeneralServiceResponseData<object>
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Salle de cinéma non trouvée."
                };
            }

            // Retourner une réponse de succès
            var theaterDto = new TheaterDto
            {
                Id = theater.Id.ToString(),
                Name = theater.Name,
                SeatCount = theater.SeatCount,
                CinemaId = theater.CinemaId,
                IsOperational = theater.IsOperational,
                ProjectionQuality = theater.ProjectionQuality
            };

            return new GeneralServiceResponseData<object>
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "Salle de cinéma récupérée avec succès.",
                Data = theaterDto
            };
        }

        public async Task<GeneralServiceResponse> UpdateTheaterAsync(string theaterId, TheaterViewModel theaterViewModel)
        {
            // Vérifie si la salle de projection existe déjà dans la base de données
            var existingTheater = await _unitOfWorkMongoDb.Theaters.GetByIdAsync(theaterId);

            // Si la salle n'est pas trouvée, renvoie une erreur 404
            if (existingTheater == null)
            {
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Salle de projection non trouvée."
                };
            }

            // Mise à jour des champs de la salle de projection uniquement si les nouvelles valeurs sont fournies
            if (!string.IsNullOrEmpty(theaterViewModel.Name))
            {
                existingTheater.Name = theaterViewModel.Name;
            }

            // Mise à jour du nombre de sièges si une valeur est fournie
            if (theaterViewModel.SeatCount > 0)
            {
                existingTheater.SeatCount = theaterViewModel.SeatCount;
            }

            // Mise à jour du statut opérationnel si la valeur est précisée
            existingTheater.IsOperational = theaterViewModel.IsOperational;

            // Mise à jour de la qualité de projection si elle est spécifiée (EnumConfig.ProjectionQuality)
            if (theaterViewModel.ProjectionQuality != EnumConfig.ProjectionQuality.Standard2D)
            {
                existingTheater.ProjectionQuality = theaterViewModel.ProjectionQuality;
            }

            // Appel de la méthode UpdateAsync pour mettre à jour l'objet dans la base de données MongoDB
            await _unitOfWorkMongoDb.Theaters.UpdateAsync(existingTheater);

            // Sauvegarde des changements
            await _unitOfWorkMongoDb.SaveChangesAsync();

            // Retourne une réponse de succès
            return new GeneralServiceResponse
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "Salle de projection mise à jour avec succès."
            };
        }

        public async Task<GeneralServiceResponse> DeleteTheaterAsync(string theaterId)
        {
            var existingTheater = await _unitOfWorkMongoDb.Theaters.GetByIdAsync(theaterId);

            if (existingTheater == null)
            {
                return new GeneralServiceResponse { IsSucceed = false, StatusCode = 404, Message = "Salle de projection non trouvée." };
            }

            _unitOfWorkMongoDb.Theaters.DeleteAsync(existingTheater.Id.ToString());
            await _unitOfWorkMongoDb.SaveChangesAsync();

            return new GeneralServiceResponse { IsSucceed = true, StatusCode = 200, Message = "Salle de projection supprimée avec succès." };
        }

        public async Task<IEnumerable<TheaterDto>> GetTheatersByCinemaAsync(int cinemaId)
        {
            var theaters = await _unitOfWorkMongoDb.Theaters.FindAsync(t => t.CinemaId == cinemaId);
            return theaters.Select(theater => new TheaterDto
            {
                Id = theater.Id.ToString(),
                Name = theater.Name,
                SeatCount = theater.SeatCount,
                ProjectionQuality = theater.ProjectionQuality,
                IsOperational = theater.IsOperational
            }).ToList();
        }

    }

}
