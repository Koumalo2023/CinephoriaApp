using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Services
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWorkPostgres _unitOfWorkPostgres;
        private readonly IUnitOfWorkMongoDb _unitOfWorkMongoDb;

        public MovieService(IUnitOfWorkPostgres unitOfWorkPostgres, IUnitOfWorkMongoDb unitOfWorkMongoDb)
        {
            _unitOfWorkPostgres = unitOfWorkPostgres;
            _unitOfWorkMongoDb = unitOfWorkMongoDb;
        }

        // Récupère tous les films disponibles
        public async Task<List<MovieViewModel>> GetAllMoviesAsync()
        {
            var movies = await _unitOfWorkMongoDb.Movies.GetAllAsync();

            return movies.Select(movie => new MovieViewModel
            {
                Id = movie.Id.ToString(),
                Title = movie.Title,
                Description = movie.Description,
                Genre = movie.Genre,
                Duration = movie.Duration,
                Director = movie.Director,
                CinemaId = movie.CinemaId,
                ReleaseDate = movie.ReleaseDate,
                MinimumAge = movie.MinimumAge,
                IsFavorite = movie.IsFavorite,
                PosterUrls = movie.PosterUrls?.ToList() ?? new List<string>()
            }).ToList();
        }

        // Récupère les films disponibles dans un cinéma spécifique
        public async Task<List<MovieViewModel>> GetMoviesByCinemaAsync(int cinemaId)
        {
            var filter = Builders<Movie>.Filter.Eq(m => m.CinemaId, cinemaId);
            var movies = await _unitOfWorkMongoDb.Movies.FilterAsync(filter);

            return movies.Select(movie => new MovieViewModel
            {
                Id = movie.Id.ToString(),
                Title = movie.Title,
                Description = movie.Description,
                Genre = movie.Genre,
                Duration = movie.Duration,
                Director = movie.Director,
                CinemaId = movie.CinemaId,
                ReleaseDate = movie.ReleaseDate,
                MinimumAge = movie.MinimumAge,
                IsFavorite = movie.IsFavorite,
                PosterUrls = movie.PosterUrls
            }).ToList();
        }

        // Récupère les détails d'un film par son identifiant
        public async Task<MovieViewModel> GetMovieByIdAsync(string filmId)
        {
            var movie = await _unitOfWorkMongoDb.Movies.GetByIdAsync(filmId);

            if (movie == null)
            {
                throw new Exception("Le film spécifié n'existe pas.");
            }

            return new MovieViewModel
            {
                Id = movie.Id.ToString(),
                Title = movie.Title,
                Description = movie.Description,
                Genre = movie.Genre,
                Duration = movie.Duration,
                Director = movie.Director,
                CinemaId = movie.CinemaId,
                ReleaseDate = movie.ReleaseDate,
                MinimumAge = movie.MinimumAge,
                IsFavorite = movie.IsFavorite,
                PosterUrls = movie.PosterUrls
            };
        }

        public async Task<List<MovieViewModel>> FilterMoviesAsync(int? cinemaId, string? genres, DateTime? date)
        {
            var filterBuilder = Builders<Movie>.Filter;
            var filters = new List<FilterDefinition<Movie>>();

            // Filtrer par CinemaId si fourni
            if (cinemaId.HasValue)
            {
                filters.Add(filterBuilder.Eq(m => m.CinemaId, cinemaId.Value));
            }

            // Filtrer par Genres si fourni
            if (!string.IsNullOrWhiteSpace(genres))
            {
                var genreList = genres.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(g => g.Trim())
                                      .ToList();

                if (genreList.Any())
                {
                    filters.Add(filterBuilder.In(m => m.Genre, genreList));
                }
            }

            // Filtrer les films par les séances qui ont lieu à la date spécifiée
            if (date.HasValue)
            {
                var showtimeFilterBuilder = Builders<Showtime>.Filter;
                var showtimeDateFilter = showtimeFilterBuilder.Gte(s => s.StartTime, date.Value.Date) &
                                         showtimeFilterBuilder.Lt(s => s.StartTime, date.Value.Date.AddDays(1));

                if (cinemaId.HasValue)
                {
                    showtimeDateFilter &= showtimeFilterBuilder.Eq(s => s.CinemaId, cinemaId.Value);
                }

                var showtimes = await _unitOfWorkMongoDb.Showtimes.FilterAsync(showtimeDateFilter);

                // Récupérer les MovieIds des séances trouvées et convertir en ObjectId
                var movieIdsForDate = showtimes.Select(s => s.MovieId).Distinct().ToList();

                // Convertir les MovieId (string) en ObjectId
                if (movieIdsForDate.Any())
                {
                    filters.Add(filterBuilder.In(m => m.Id, movieIdsForDate.Select(id => ObjectId.Parse(id)).ToList()));
                }
                else
                {
                    // Si aucune séance n'est trouvée pour cette date, retourner une liste vide
                    return new List<MovieViewModel>();
                }
            }

            // Combiner les filtres avec un "AND" si des filtres existent, sinon utiliser "Empty"
            var filter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;

            // Appliquer le filtre à la collection MongoDB
            var movies = await _unitOfWorkMongoDb.Movies.FilterMovieAsync(filter);

            // Mapper les résultats vers les ViewModels
            return movies.Select(movie => new MovieViewModel
            {
                Id = movie.Id.ToString(),
                Title = movie.Title,
                Description = movie.Description,
                Genre = movie.Genre,
                Duration = movie.Duration,
                Director = movie.Director,
                CinemaId = movie.CinemaId,
                ReleaseDate = movie.ReleaseDate,
                MinimumAge = movie.MinimumAge,
                IsFavorite = movie.IsFavorite,
                PosterUrls = movie.PosterUrls
            }).ToList();
        }



        // Filtre les films selon les critères fournis
        //public async Task<List<MovieViewModel>> FilterMoviesAsync(int? cinemaId, string? genres, DateTime? date)
        //{
        //    var filterBuilder = Builders<Movie>.Filter;
        //    var filters = new List<FilterDefinition<Movie>>();

        //    // Filtrer par CinemaId si fourni
        //    if (cinemaId.HasValue)
        //    {
        //        filters.Add(filterBuilder.Eq(m => m.CinemaId, cinemaId.Value));
        //    }

        //    // Filtrer par Genres si fourni
        //    if (!string.IsNullOrWhiteSpace(genres))
        //    {
        //        var genreList = genres.Split(',', StringSplitOptions.RemoveEmptyEntries)
        //                              .Select(g => g.Trim())
        //                              .ToList();

        //        if (genreList.Any())
        //        {
        //            filters.Add(filterBuilder.In(m => m.Genre, genreList));
        //        }
        //    }

        //    // Filtrer les films par les séances qui ont lieu à la date spécifiée
        //    if (date.HasValue)
        //    {
        //        // Récupérer les séances de la date spécifiée
        //        var showtimeFilterBuilder = Builders<Showtime>.Filter;
        //        var showtimeDateFilter = showtimeFilterBuilder.Gte(s => s.StartTime, date.Value.Date) &
        //                                 showtimeFilterBuilder.Lt(s => s.StartTime, date.Value.Date.AddDays(1));

        //        if (cinemaId.HasValue)
        //        {
        //            showtimeDateFilter &= showtimeFilterBuilder.Eq(s => s.CinemaId, cinemaId.Value);
        //        }

        //        var showtimes = await _unitOfWorkMongoDb.Showtimes.FilterAsync(showtimeDateFilter);

        //        // Récupérer les MovieIds des séances trouvées
        //        var movieIdsForDate = showtimes.Select(s => s.MovieId).Distinct().ToList();

        //        if (movieIdsForDate.Any())
        //        {
        //            filters.Add(filterBuilder.In(m => m.Id, movieIdsForDate));
        //        }
        //        else
        //        {
        //            // Si aucune séance n'est trouvée pour cette date, retourner une liste vide
        //            return new List<MovieViewModel>();
        //        }
        //    }

        //    // Combiner les filtres avec un "AND" si des filtres existent, sinon utiliser "Empty"
        //    var filter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;

        //    // Appliquer le filtre à la collection MongoDB
        //    var movies = await _unitOfWorkMongoDb.Movies.FilterMovieAsync(filter);

        //    // Mapper les résultats vers les ViewModels
        //    return movies.Select(movie => new MovieViewModel
        //    {
        //        Id = movie.Id.ToString(),
        //        Title = movie.Title,
        //        Description = movie.Description,
        //        Genre = movie.Genre,
        //        Duration = movie.Duration,
        //        Director = movie.Director,
        //        CinemaId = movie.CinemaId,
        //        ReleaseDate = movie.ReleaseDate,
        //        MinimumAge = movie.MinimumAge,
        //        IsFavorite = movie.IsFavorite,
        //        PosterUrls = movie.PosterUrls
        //    }).ToList();
        //}


        // Crée un nouveau film
        public async Task<GeneralServiceResponse> CreateMovieAsync(MovieViewModel movieViewModel)
        {
            var movie = new Movie
            {
                Id = ObjectId.GenerateNewId(),
                Title = movieViewModel.Title,
                Description = movieViewModel.Description,
                Genre = movieViewModel.Genre,
                Duration = movieViewModel.Duration,
                Director = movieViewModel.Director,
                CinemaId = movieViewModel.CinemaId,
                ReleaseDate = movieViewModel.ReleaseDate,
                MinimumAge = movieViewModel.MinimumAge,
                IsFavorite = movieViewModel.IsFavorite,
                PosterUrls = movieViewModel.PosterUrls
            };

            await _unitOfWorkMongoDb.Movies.AddAsync(movie);

            return new GeneralServiceResponse
            {
                IsSucceed = true,
                StatusCode = 201,
                Message = "Film créé avec succès."
            };
        }

        public async Task<bool> AddPosterToMovieAsync(string movieId, string imageUrl)
        {
            var movie = await _unitOfWorkMongoDb.Movies.GetByIdAsync(movieId);
            if (movie == null) return false;

            movie.PosterUrls.Add(imageUrl);
            await _unitOfWorkMongoDb.Movies.UpdateAsync(movie);

            return true;
        }

  
        // Modifie un film existant
        public async Task<GeneralServiceResponse> UpdateMovieAsync(string filmId, MovieViewModel movieViewModel)
        {
            var movie = await _unitOfWorkMongoDb.Movies.GetByIdAsync(filmId);

            if (movie == null)
            {
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Le film spécifié n'existe pas."
                };
            }

            movie.Title = movieViewModel.Title;
            movie.Description = movieViewModel.Description;
            movie.Genre = movieViewModel.Genre;
            movie.Duration = movieViewModel.Duration;
            movie.Director = movieViewModel.Director;
            movie.CinemaId = movieViewModel.CinemaId;
            movie.ReleaseDate = movieViewModel.ReleaseDate;
            movie.MinimumAge = movieViewModel.MinimumAge;
            movie.IsFavorite = movieViewModel.IsFavorite;
            movie.PosterUrls = movieViewModel.PosterUrls;

            await _unitOfWorkMongoDb.Movies.UpdateAsync(movie);

            return new GeneralServiceResponse
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "Film mis à jour avec succès."
            };
        }


        // Supprime un film existant
        public async Task<GeneralServiceResponse> DeleteMovieAsync(string filmId)
        {
            var movieExists = await _unitOfWorkMongoDb.ExistsAsync<Movie>(filmId);

            if (!movieExists)
            {
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Le film spécifié n'existe pas."
                };
            }

            await _unitOfWorkMongoDb.Movies.DeleteAsync(filmId);

            return new GeneralServiceResponse
            {
                IsSucceed = true,
                StatusCode = 204,
                Message = "Film supprimé avec succès."
            };
        }


        // Soumet un avis pour un film
        public async Task<Review> SubmitReviewAsync(ReviewViewModel reviewViewModel)
        {
            // Si MovieId est un string, on garde cette version. Si vous utilisez ObjectId dans votre modèle, convertissez-le correctement.
            var review = new Review
            {
                MovieId = reviewViewModel.MovieId,  // MovieId est un string ici
                UserId = reviewViewModel.UserId,
                Comment = reviewViewModel.Comment,
                CreatedAt = DateTime.Now
            };

            await _unitOfWorkMongoDb.Reviews.AddAsync(review);
            return review;
        }

        // Récupère les avis d'un film
        public async Task<GeneralServiceResponseData<List<ReviewDto>>> GetReviewsByMovieIdAsync(string movieId)
        {
            // Vérifiez si l'ID du film est valide
            if (!ObjectId.TryParse(movieId, out ObjectId objectId))
            {
                return new GeneralServiceResponseData<List<ReviewDto>>
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "ID de film invalide."
                };
            }

            // Recherchez les avis associés au film dans MongoDB
            var filter = Builders<Review>.Filter.Eq("MovieId", objectId);
            var reviews = await _unitOfWorkMongoDb.Reviews.FilterAsync(filter);

            // Vérifiez si des avis sont trouvés
            if (reviews == null || !reviews.Any())
            {
                return new GeneralServiceResponseData<List<ReviewDto>>
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Aucun avis trouvé pour ce film."
                };
            }

            // Transformation des avis en ReviewDto
            var reviewDtos = reviews.Select(review => new ReviewDto
            {
                Id = review.Id.ToString(),
                MovieId = review.MovieId.ToString(),
                UserId = review.UserId,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            }).ToList();

            // Retourner les avis avec succès
            return new GeneralServiceResponseData<List<ReviewDto>>
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "Avis récupérés avec succès.",
                Data = reviewDtos
            };
        }
        // Soumet une note pour un film
        public async Task<MovieRating> SubmitMovieRatingAsync(MovieRatingViewModel movieRatingViewModel)
        {
            var movieRating = new MovieRating
            {
                MovieId = movieRatingViewModel.MovieId.ToString(),
                AppUserId = movieRatingViewModel.AppUserId,
                Rating = movieRatingViewModel.Rating,
                Comment = movieRatingViewModel.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWorkPostgres.MovieRatings.CreateAsync(movieRating);
            await _unitOfWorkPostgres.CompleteAsync();

            return movieRating;
        }

        // Valide une note pour un film
        public async Task<MovieRating> ValidateMovieRatingAsync(int movieRatingId)
        {
            var movieRating = await _unitOfWorkPostgres.MovieRatings.GetByIdAsync(movieRatingId);
            if (movieRating == null)
                throw new KeyNotFoundException("Movie rating not found");

            movieRating.IsValidated = true;
            await _unitOfWorkPostgres.MovieRatings.UpdateAsync(movieRating);
            await _unitOfWorkPostgres.CompleteAsync();

            return movieRating;
        }

        // Supprime une note sur un film
        public async Task DeleteMovieRatingAsync(int movieRatingId)
        {
            var movieRating = await _unitOfWorkPostgres.MovieRatings.GetByIdAsync(movieRatingId);
            if (movieRating == null)
                throw new KeyNotFoundException("Movie rating not found");

            await _unitOfWorkPostgres.MovieRatings.DeleteAsync(movieRating);
            await _unitOfWorkPostgres.CompleteAsync();
        }
    }

}
