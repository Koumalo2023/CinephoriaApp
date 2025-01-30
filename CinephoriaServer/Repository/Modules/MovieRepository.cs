using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository.EntityFramwork;
using Microsoft.EntityFrameworkCore;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Repository
{
    public interface IMovieRepository : IReadRepository<Movie>, IWriteRepository<Movie>
    {
        /// <summary>
        /// Récupère la liste des derniers films ajoutés.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        Task<List<Movie>> GetRecentMoviesAsync();


        /// <summary>
        /// Récupère la liste de tous les films dans tous les Cinemas.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        Task<List<Movie>> GetAllMoviesAsync();

        /// <summary>
        /// Récupère les détails d'un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Un objet Movie contenant les détails du film.</returns>
        Task<Movie> GetMovieDetailsAsync(int movieId);

        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances.</returns>
        Task<List<Showtime>> GetMovieSessionsAsync(int movieId);

        /// <summary>
        /// Soumet un avis sur un film de la part d'un utilisateur.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="rating">La note attribuée au film.</param>
        /// <param name="description">La description de l'avis.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task SubmitMovieReviewAsync(MovieReviewDto reviewDto);

        /// <summary>
        /// Filtre les films en fonction du cinéma, du genre et de la date.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma (optionnel).</param>
        /// <param name="genre">Le genre du film (optionnel).</param>
        /// <param name="date">La date de projection (optionnelle).</param>
        /// <returns>Une liste de films correspondant aux critères de filtrage.</returns>
        Task<List<Movie>> FilterMoviesAsync(int? cinemaId, MovieGenre? genre, DateTime? date);

        /// <summary>
        /// Crée un nouveau film.
        /// </summary>
        /// <param name="movie">L'objet Movie représentant le film à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task CreateMovieAsync(Movie movie);

        /// <summary>
        /// Met à jour les informations d'un film existant.
        /// </summary>
        /// <param name="movie">L'objet Movie avec les nouvelles informations.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task UpdateMovieAsync(Movie movie);

        /// <summary>
        /// Supprime un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task DeleteMovieAsync(int movieId);


        /// <summary>
        /// Récupère la liste des films qui ont une séance dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste de films.</returns>
        Task<List<Movie>> GetMoviesByCinemaIdAsync(int cinemaId);

    }


    public class MovieRepository : EFRepository<Movie>, IMovieRepository
    {
        private readonly DbContext _context;

        public MovieRepository(DbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Récupère la liste des derniers films ajoutés.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        public async Task<List<Movie>> GetRecentMoviesAsync()
        {
            // Calculer la date du dernier mercredi
            DateTime lastWednesday = GetLastWednesday();

            // Récupérer les films ajoutés depuis le dernier mercredi
            var recentMovies = await _context.Set<Movie>()
                .Where(m => m.CreatedAt >= lastWednesday)
                .OrderByDescending(m => m.CreatedAt)
                .Take(20)
                .ToListAsync();

            return recentMovies;
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            return await _context.Set<Movie>()
                                 .ToListAsync();
        }

        /// <summary>
        /// Récupère les détails d'un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Un objet Movie contenant les détails du film.</returns>
        public async Task<Movie> GetMovieDetailsAsync(int movieId)
        {
            return await _context.Set<Movie>()
                .Include(m => m.Showtimes) // Inclure les séances
                .Include(m => m.MovieRatings) // Inclure les notations
                .FirstOrDefaultAsync(m => m.MovieId == movieId);
        }

        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances.</returns>
        public async Task<List<Showtime>> GetMovieSessionsAsync(int movieId)
        {
            return await _context.Set<Showtime>()
                .Where(s => s.MovieId == movieId)
                .ToListAsync();
        }

        /// <summary>
        /// Soumet un avis sur un film de la part d'un utilisateur.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="rating">La note attribuée au film.</param>
        /// <param name="description">La description de l'avis.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task SubmitMovieReviewAsync(MovieReviewDto reviewDto)
        {
            var review = new MovieRating
            {
                MovieId = reviewDto.MovieId,
                AppUserId = reviewDto.UserId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Set<MovieRating>().Add(review);
            await _context.SaveChangesAsync();

            // Mettre à jour la note moyenne du film
            await UpdateMovieAverageRatingAsync(reviewDto.MovieId);
        }

        /// <summary>
        /// Filtre les films en fonction du cinéma, du genre et de la date.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma (optionnel).</param>
        /// <param name="genre">Le genre du film (optionnel).</param>
        /// <param name="date">La date de projection (optionnelle).</param>
        /// <returns>Une liste de films correspondant aux critères de filtrage.</returns>
        public async Task<List<Movie>> FilterMoviesAsync(int? cinemaId, MovieGenre? genre, DateTime? date)
        {
            var query = _context.Set<Movie>()
                .Include(m => m.Showtimes)
                .AsQueryable();

            // Filtrer par cinéma
            if (cinemaId.HasValue)
            {
                query = query.Where(m => m.Showtimes.Any(s => s.CinemaId == cinemaId.Value));
            }

            // Filtrer par genre
            if (genre.HasValue)
            {
                query = query.Where(m => m.Genre == genre.Value);
            }

            // Filtrer par jour
            if (date.HasValue)
            {
                query = query.Where(m => m.Showtimes.Any(s => s.StartTime.Date == date.Value.Date));
            }

            // Limiter le nombre de résultats (ex: 20 films)
            return await query.Take(20).ToListAsync();
        }

        /// <summary>
        /// Crée un nouveau film.
        /// </summary>
        /// <param name="movie">L'objet Movie représentant le film à créer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task CreateMovieAsync(Movie movie)
        {
            movie.CreatedAt = DateTime.UtcNow;
            movie.UpdatedAt = DateTime.UtcNow;

            _context.Set<Movie>().Add(movie);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Met à jour les informations d'un film existant.
        /// </summary>
        /// <param name="movie">L'objet Movie avec les nouvelles informations.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task UpdateMovieAsync(Movie movie)
        {
            movie.UpdatedAt = DateTime.UtcNow;

            _context.Set<Movie>().Update(movie);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Supprime un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        public async Task DeleteMovieAsync(int movieId)
        {
            var movie = await _context.Set<Movie>().FindAsync(movieId);
            if (movie == null)
            {
                throw new ArgumentException("Film non trouvé.");
            }

            _context.Set<Movie>().Remove(movie);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Met à jour la note moyenne d'un film.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une tâche asynchrone.</returns>
        private async Task UpdateMovieAverageRatingAsync(int movieId)
        {
            var movie = await _context.Set<Movie>()
                .Include(m => m.MovieRatings)
                .FirstOrDefaultAsync(m => m.MovieId == movieId);

            if (movie != null && movie.MovieRatings.Any())
            {
                movie.AverageRating = movie.MovieRatings.Average(r => r.Rating);
                _context.Set<Movie>().Update(movie);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Récupère la liste des films qui ont une séance dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste de films.</returns>
        public async Task<List<Movie>> GetMoviesByCinemaIdAsync(int cinemaId)
        {
            return await _context.Set<Showtime>()
                .Where(s => s.CinemaId == cinemaId) 
                .Select(s => s.Movie) 
                .Distinct()
                .ToListAsync();
        }

        private DateTime GetLastWednesday()
        {
            DateTime today = DateTime.UtcNow.Date;
            int daysSinceWednesday = ((int)today.DayOfWeek - (int)DayOfWeek.Wednesday + 7) % 7;
            DateTime lastWednesday = today.AddDays(-daysSinceWednesday);
            return lastWednesday;
        }

    }
}
