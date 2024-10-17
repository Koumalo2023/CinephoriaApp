using CinephoriaBackEnd.Data;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using MongoDB.Driver;

namespace CinephoriaServer.Repository
{
    public interface IMovieRepository
    {
        /// <summary>
        /// Récupère tous les films disponibles.
        /// </summary>
        /// <returns>Une liste de tous les films disponibles.</returns>
        Task<List<Movie>> GetAllMoviesAsync();

        /// <summary>
        /// Récupère tous les films disponibles dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">Identifiant du cinéma.</param>
        /// <returns>Une liste de films disponibles dans le cinéma spécifié.</returns>
        Task<List<Movie>> GetMoviesByCinemaAsync(int cinemaId);

        /// <summary>
        /// Récupère les détails d'un film spécifique, y compris les séances disponibles.
        /// </summary>
        /// <param name="filmId">L'identifiant unique du film.</param>
        /// <returns>Les détails du film, y compris les séances associées.</returns>
        Task<Movie> GetMovieByIdAsync(string filmId);

        /// <summary>
        /// Filtre les films par cinéma, genre ou jour spécifique.
        /// Si un paramètre n'est pas fourni, il n'est pas pris en compte dans le filtrage.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <param name="genre">Le genre du film.</param>
        /// <param name="date">Le jour spécifique des séances.</param>
        /// <returns>Une liste filtrée des films selon les critères fournis.</returns>
        Task<List<Movie>> FilterMoviesAsync(int? cinemaId, string genre, DateTime? date);

        /// <summary>
        /// Crée un nouveau film.
        /// Cette méthode est réservée aux administrateurs et aux employés.
        /// </summary>
        /// <param name="movie">L'objet film à créer.</param>
        /// <returns>Le film nouvellement créé.</returns>
        Task<Movie> CreateMovieAsync(Movie movie);

        /// <summary>
        /// Modifie un film existant.
        /// Cette méthode est réservée aux administrateurs et aux employés.
        /// </summary>
        /// <param name="filmId">L'identifiant unique du film à modifier.</param>
        /// <param name="movie">Les nouvelles données du film.</param>
        /// <returns>Le film mis à jour.</returns>
        Task<Movie> UpdateMovieAsync(string filmId, Movie movie);

        /// <summary>
        /// Supprime un film existant.
        /// Cette méthode est réservée aux administrateurs et aux employés.
        /// </summary>
        /// <param name="filmId">L'identifiant unique du film à supprimer.</param>
        /// <returns>Une tâche représentant l'opération de suppression.</returns>
        Task DeleteMovieAsync(string filmId);

        /// <summary>
        /// Permet à un utilisateur de laisser un avis sur un film.
        /// </summary>
        /// <param name="review">L'avis à soumettre.</param>
        /// <returns>L'avis nouvellement créé.</returns>
        Task<Review> SubmitReviewAsync(Review review);

        /// <summary>
        /// Récupère tous les avis d'un film.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste d'avis associés au film.</returns>
        Task<List<Review>> GetReviewsByMovieIdAsync(string movieId);

        /// <summary>
        /// Permet à un utilisateur de laisser une note sur un film.
        /// </summary>
        /// <param name="movieRating">La note à soumettre.</param>
        /// <returns>La note nouvellement créée.</returns>
        Task<MovieRating> SubmitMovieRatingAsync(MovieRating movieRating);

        /// <summary>
        /// Valide une note sur un film, action réservée aux employés.
        /// </summary>
        /// <param name="movieRatingId">L'identifiant unique de la note à valider.</param>
        /// <returns>La note validée.</returns>
        Task<MovieRating> ValidateMovieRatingAsync(int movieRatingId);

        /// <summary>
        /// Supprime une note sur un film, action réservée aux employés.
        /// </summary>
        /// <param name="movieRatingId">L'identifiant unique de la note à supprimer.</param>
        /// <returns>Une tâche représentant l'opération de suppression.</returns>
        Task DeleteMovieRatingAsync(int movieRatingId);
    }


    public class MovieRepository : IMovieRepository
    {
        private readonly CinephoriaDbContext _context;
        private readonly IMongoCollection<Movie> _movieCollection;
        private readonly IMongoCollection<Review> _reviewCollection;

        public MovieRepository(CinephoriaDbContext context, IMongoDatabase mongoDatabase)
        {
            _context = context;
            _movieCollection = mongoDatabase.GetCollection<Movie>("Movies");
            _reviewCollection = mongoDatabase.GetCollection<Review>("Reviews");
        }

        /// <summary>
        /// Récupère tous les films disponibles.
        /// </summary>
        /// <returns>Une liste de tous les films disponibles.</returns>
        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            return await _movieCollection.Find(_ => true).ToListAsync();
        }

        public async Task<List<Movie>> GetMoviesByCinemaAsync(int cinemaId)
        {
            // Filtrer les films en fonction du cinéma (en supposant qu'il y a une relation)
            var filter = Builders<Movie>.Filter.Eq(m => m.CinemaId, cinemaId);
            return await _movieCollection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Récupère les détails d'un film spécifique, y compris les séances disponibles.
        /// </summary>
        /// <param name="filmId">L'identifiant unique du film.</param>
        /// <returns>Les détails du film, y compris les séances associées.</returns>
        public async Task<Movie> GetMovieByIdAsync(string filmId)
        {
            return await _movieCollection.Find(m => m.Id.ToString() == filmId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Filtre les films par cinéma, genre ou jour spécifique.
        /// Si un paramètre n'est pas fourni, il n'est pas pris en compte dans le filtrage.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <param name="genre">Le genre du film.</param>
        /// <param name="date">Le jour spécifique des séances.</param>
        /// <returns>Une liste filtrée des films selon les critères fournis.</returns>
        public async Task<List<Movie>> FilterMoviesAsync(int? cinemaId, string genre, DateTime? date)
        {
            var filter = Builders<Movie>.Filter.Empty;

            if (cinemaId.HasValue)
            {
                filter &= Builders<Movie>.Filter.Eq(m => m.CinemaId, cinemaId.Value);
            }

            if (!string.IsNullOrEmpty(genre))
            {
                filter &= Builders<Movie>.Filter.Eq(m => m.Genre, genre);
            }

            if (date.HasValue)
            {
                filter &= Builders<Movie>.Filter.Eq(m => m.ReleaseDate.Date, date.Value.Date);
            }

            return await _movieCollection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Crée un nouveau film.
        /// Cette méthode est réservée aux administrateurs et aux employés.
        /// </summary>
        /// <param name="movie">L'objet film à créer.</param>
        /// <returns>Le film nouvellement créé.</returns>
        public async Task<Movie> CreateMovieAsync(Movie movie)
        {
            await _movieCollection.InsertOneAsync(movie);
            return movie;
        }

        /// <summary>
        /// Modifie un film existant.
        /// Cette méthode est réservée aux administrateurs et aux employés.
        /// </summary>
        /// <param name="filmId">L'identifiant unique du film à modifier.</param>
        /// <param name="movie">Les nouvelles données du film.</param>
        /// <returns>Le film mis à jour.</returns>
        public async Task<Movie> UpdateMovieAsync(string filmId, Movie movie)
        {
            var result = await _movieCollection.ReplaceOneAsync(m => m.Id.ToString() == filmId, movie);
            if (result.MatchedCount == 0)
            {
                throw new KeyNotFoundException("Film non trouvé.");
            }
            return movie;
        }

        /// <summary>
        /// Supprime un film existant.
        /// Cette méthode est réservée aux administrateurs et aux employés.
        /// </summary>
        /// <param name="filmId">L'identifiant unique du film à supprimer.</param>
        /// <returns>Une tâche représentant l'opération de suppression.</returns>
        public async Task DeleteMovieAsync(string filmId)
        {
            var result = await _movieCollection.DeleteOneAsync(m => m.Id.ToString() == filmId);
            if (result.DeletedCount == 0)
            {
                throw new KeyNotFoundException("Film non trouvé.");
            }
        }

        /// <summary>
        /// Permet à un utilisateur de laisser un avis sur un film.
        /// </summary>
        /// <param name="review">L'avis à soumettre.</param>
        /// <returns>L'avis nouvellement créé.</returns>
        public async Task<Review> SubmitReviewAsync(Review review)
        {
            await _reviewCollection.InsertOneAsync(review);
            return review;
        }

        /// <summary>
        /// Récupère tous les avis d'un film.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste d'avis associés au film.</returns>
        public async Task<List<Review>> GetReviewsByMovieIdAsync(string movieId)
        {
            return await _reviewCollection.Find(r => r.MovieId == movieId).ToListAsync();
        }

        /// <summary>
        /// Permet à un utilisateur de laisser une note sur un film.
        /// </summary>
        /// <param name="movieRating">La note à soumettre.</param>
        /// <returns>La note nouvellement créée.</returns>
        public async Task<MovieRating> SubmitMovieRatingAsync(MovieRating movieRating)
        {
            _context.MovieRatings.Add(movieRating);
            await _context.SaveChangesAsync();
            return movieRating;
        }

        /// <summary>
        /// Valide une note sur un film, action réservée aux employés.
        /// </summary>
        /// <param name="movieRatingId">L'identifiant unique de la note à valider.</param>
        /// <returns>La note validée.</returns>
        public async Task<MovieRating> ValidateMovieRatingAsync(int movieRatingId)
        {
            var movieRating = await _context.MovieRatings.FindAsync(movieRatingId);
            if (movieRating == null)
            {
                throw new KeyNotFoundException("Note non trouvée.");
            }

            movieRating.IsValidated = true;
            await _context.SaveChangesAsync();
            return movieRating;
        }

        /// <summary>
        /// Supprime une note sur un film, action réservée aux employés.
        /// </summary>
        /// <param name="movieRatingId">L'identifiant unique de la note à supprimer.</param>
        /// <returns>Une tâche représentant l'opération de suppression.</returns>
        public async Task DeleteMovieRatingAsync(int movieRatingId)
        {
            var movieRating = await _context.MovieRatings.FindAsync(movieRatingId);
            if (movieRating == null)
            {
                throw new KeyNotFoundException("Note non trouvée.");
            }

            _context.MovieRatings.Remove(movieRating);
            await _context.SaveChangesAsync();
        }
    }
}
