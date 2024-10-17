using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository;
using MongoDB.Bson;
using MongoDB.Driver;

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
                PosterUrl = movie.PosterUrl
            }).ToList();
        }

        // Récupère les films disponibles dans un cinéma spécifique
        public async Task<List<MovieViewModel>> GetMoviesByCinemaAsync(int cinemaId)
        {
            var filter = Builders<Movie>.Filter.Eq(m => m.CinemaId, cinemaId);
            var movies = await _unitOfWorkMongoDb.Movies.FilterAsync(filter);

            return movies.Select(movie => new MovieViewModel
            {
                Title = movie.Title,
                Description = movie.Description,
                Genre = movie.Genre,
                Duration = movie.Duration,
                Director = movie.Director,
                CinemaId = movie.CinemaId,
                ReleaseDate = movie.ReleaseDate,
                MinimumAge = movie.MinimumAge,
                IsFavorite = movie.IsFavorite,
                PosterUrl = movie.PosterUrl
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
                PosterUrl = movie.PosterUrl
            };
        }

        // Filtre les films selon les critères fournis
        public async Task<List<MovieViewModel>> FilterMoviesAsync(int? cinemaId, string genre, DateTime? date)
        {
            var filterBuilder = Builders<Movie>.Filter;
            var filter = filterBuilder.Empty;

            if (cinemaId.HasValue)
            {
                filter &= filterBuilder.Eq(m => m.CinemaId, cinemaId.Value);
            }

            if (!string.IsNullOrEmpty(genre))
            {
                filter &= filterBuilder.Eq(m => m.Genre, genre);
            }

            if (date.HasValue)
            {
                filter &= filterBuilder.Gte(m => m.ReleaseDate, date.Value);
            }

            var movies = await _unitOfWorkMongoDb.Movies.FilterAsync(filter);

            return movies.Select(movie => new MovieViewModel
            {
                Title = movie.Title,
                Description = movie.Description,
                Genre = movie.Genre,
                Duration = movie.Duration,
                Director = movie.Director,
                CinemaId = movie.CinemaId,
                ReleaseDate = movie.ReleaseDate,
                MinimumAge = movie.MinimumAge,
                IsFavorite = movie.IsFavorite,
                PosterUrl = movie.PosterUrl
            }).ToList();
        }

        // Crée un nouveau film
        public async Task<GeneralServiceResponseData<object>> CreateMovieAsync(MovieViewModel movieViewModel)
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
                PosterUrl = movieViewModel.PosterUrl
            };

            await _unitOfWorkMongoDb.Movies.AddAsync(movie);

            var result = new MovieDto
            {
                Id = movieViewModel.Id.ToString(),
                Title = movieViewModel.Title,
                Description = movieViewModel.Description,
                Genre = movieViewModel.Genre,
                Duration = movieViewModel.Duration,
                Director = movieViewModel.Director,
                CinemaId = movieViewModel.CinemaId,
                ReleaseDate = movieViewModel.ReleaseDate,
                MinimumAge = movieViewModel.MinimumAge,
                IsFavorite = movieViewModel.IsFavorite,
                PosterUrl = movieViewModel.PosterUrl
            };

            return new GeneralServiceResponseData<object>
            {
                IsSucceed = true,
                StatusCode = 201,
                Message = "Salle de projection créée avec succès.",
                Data = result
            };
        }

        // Modifie un film existant
        public async Task<MovieViewModel> UpdateMovieAsync(string filmId, MovieViewModel movieViewModel)
        {
            var movie = await _unitOfWorkMongoDb.Movies.GetByIdAsync(filmId);

            if (movie == null)
            {
                throw new Exception("Le film spécifié n'existe pas.");
            }

            movie.Id = ObjectId.GenerateNewId();
            movie.Title = movieViewModel.Title;
            movie.Description = movieViewModel.Description;
            movie.Genre = movieViewModel.Genre;
            movie.Duration = movieViewModel.Duration;
            movie.Director = movieViewModel.Director;
            movie.CinemaId = movieViewModel.CinemaId;
            movie.ReleaseDate = movieViewModel.ReleaseDate;
            movie.MinimumAge = movieViewModel.MinimumAge;
            movie.IsFavorite = movieViewModel.IsFavorite;
            movie.PosterUrl = movieViewModel.PosterUrl;

            await _unitOfWorkMongoDb.Movies.UpdateAsync(movie);

            return movieViewModel;
        }

        // Supprime un film existant
        public async Task DeleteMovieAsync(string filmId)
        {
            var movieExists = await _unitOfWorkMongoDb.ExistsAsync<Movie>(filmId);

            if (!movieExists)
            {
                throw new Exception("Le film spécifié n'existe pas.");
            }

            await _unitOfWorkMongoDb.Movies.DeleteAsync(filmId);
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
