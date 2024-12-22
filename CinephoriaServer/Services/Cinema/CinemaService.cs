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

        public async Task<GeneralServiceResponse> CreateCinemaAsync(CinemaViewModel cinemaViewModel)
        {
            try
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
                return new GeneralServiceResponse
                {
                    IsSucceed = true,
                    StatusCode = 201,
                    Message = "Cinéma créé avec succès."
                };
            }
            catch (Exception ex)
            {
                // Affiche le message d'erreur dans la console pour le suivi des erreurs
                Console.WriteLine($"Erreur lors de la création du cinéma : {ex.Message}");

                // Retourne une réponse d'erreur avec les détails de l'exception
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la création du cinéma : {ex.Message}"
                };
            }
        }

        public async Task<GeneralServiceResponse> UpdateCinemaAsync(int cinemaId, CinemaViewModel cinemaViewModel)
        {
            try
            {
                // Récupération du cinéma existant
                var existingCinema = await _unitOfWorkPostgres.Cinemas.GetByIdAsync(cinemaId);

                // Si le cinéma n'existe pas
                if (existingCinema == null)
                {
                    return new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "Cinéma non trouvé."
                    };
                }

                // Mise à jour des informations du cinéma
                existingCinema.Name = cinemaViewModel.Name ?? existingCinema.Name;
                existingCinema.Address = cinemaViewModel.Address ?? existingCinema.Address;
                existingCinema.PhoneNumber = cinemaViewModel.PhoneNumber ?? existingCinema.PhoneNumber;
                existingCinema.City = cinemaViewModel.City ?? existingCinema.City;
                existingCinema.Country = cinemaViewModel.Country ?? existingCinema.Country;
                existingCinema.OpeningHours = cinemaViewModel.OpeningHours ?? existingCinema.OpeningHours;

                // Enregistrement des changements
                _unitOfWorkPostgres.Cinemas.Update(existingCinema);
                await _unitOfWorkPostgres.CompleteAsync();

                return new GeneralServiceResponse
                {
                    IsSucceed = true,
                    StatusCode = 200,
                    Message = "Cinéma mis à jour avec succès."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour du cinéma {cinemaId} : {ex.Message}");

                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la mise à jour du cinéma : {ex.Message}"
                };
            }
        }

        public async Task<GeneralServiceResponse> DeleteCinemaAsync(int cinemaId)
        {
            try
            {
                var existingCinema = await _unitOfWorkPostgres.Cinemas.GetByIdAsync(cinemaId);

                if (existingCinema == null)
                {
                    return new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "Cinéma non trouvé."
                    };
                }

                _unitOfWorkPostgres.Cinemas.Delete(existingCinema);
                await _unitOfWorkPostgres.CompleteAsync();

                return new GeneralServiceResponse
                {
                    IsSucceed = true,
                    StatusCode = 200,
                    Message = "Cinéma supprimé avec succès."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression du cinéma {cinemaId} : {ex.Message}");

                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la suppression du cinéma : {ex.Message}"
                };
            }
        }

        public async Task<IEnumerable<CinemaDto>> GetAllCinemasAsync()
        {
            var cinemas = await _unitOfWorkPostgres.Cinemas.GetAllAsync();
            return cinemas.Select(c => new CinemaDto
            {
                CinemaId = c.CinemaId,
                Name = c.Name,
                Address = c.Address,
                PhoneNumber = c.PhoneNumber,
                City = c.City,
                Country = c.Country
            }).ToList();
        }

        public async Task<GeneralServiceResponse> CreateTheaterForCinemaAsync(TheaterViewModel theaterViewModel)
        {
            try
            {
                var cinemaExists = await _unitOfWorkPostgres.Cinemas.GetByIdAsync(theaterViewModel.CinemaId);
                if (cinemaExists == null)
                {
                    return new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "Cinéma non trouvé."
                    };
                }

                var theater = new Theater
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = theaterViewModel.Name,
                    SeatCount = theaterViewModel.SeatCount,
                    CinemaId = theaterViewModel.CinemaId,
                    IsOperational = theaterViewModel.IsOperational,
                    ProjectionQuality = theaterViewModel.ProjectionQuality
                };

                await _unitOfWorkMongoDb.Theaters.AddAsync(theater);
                await _unitOfWorkMongoDb.SaveChangesAsync();

                return new GeneralServiceResponse
                {
                    IsSucceed = true,
                    StatusCode = 201,
                    Message = "Salle de projection créée avec succès."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création de la salle de projection : {ex.Message}");
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la création de la salle de projection : {ex.Message}"
                };
            }
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
            try
            {
                var existingTheater = await _unitOfWorkMongoDb.Theaters.GetByIdAsync(theaterId);
                if (existingTheater == null)
                {
                    return new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "Salle de projection non trouvée."
                    };
                }

                existingTheater.Name = theaterViewModel.Name ?? existingTheater.Name;
                existingTheater.SeatCount = theaterViewModel.SeatCount > 0 ? theaterViewModel.SeatCount : existingTheater.SeatCount;
                existingTheater.IsOperational = theaterViewModel.IsOperational;
                existingTheater.ProjectionQuality = theaterViewModel.ProjectionQuality != EnumConfig.ProjectionQuality.Standard2D ? theaterViewModel.ProjectionQuality : existingTheater.ProjectionQuality;

                await _unitOfWorkMongoDb.Theaters.UpdateAsync(existingTheater);
                await _unitOfWorkMongoDb.SaveChangesAsync();

                return new GeneralServiceResponse
                {
                    IsSucceed = true,
                    StatusCode = 200,
                    Message = "Salle de projection mise à jour avec succès."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour de la salle de projection : {ex.Message}");
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la mise à jour de la salle de projection : {ex.Message}"
                };
            }
        }

        public async Task<GeneralServiceResponse> DeleteTheaterAsync(string theaterId)
        {
            try
            {
                var existingTheater = await _unitOfWorkMongoDb.Theaters.GetByIdAsync(theaterId);
                if (existingTheater == null)
                {
                    return new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "Salle de projection non trouvée."
                    };
                }

                await _unitOfWorkMongoDb.Theaters.DeleteAsync(existingTheater.Id.ToString());
                await _unitOfWorkMongoDb.SaveChangesAsync();

                return new GeneralServiceResponse
                {
                    IsSucceed = true,
                    StatusCode = 200,
                    Message = "Salle de projection supprimée avec succès."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression de la salle de projection : {ex.Message}");
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = $"Erreur lors de la suppression de la salle de projection : {ex.Message}"
                };
            }
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
