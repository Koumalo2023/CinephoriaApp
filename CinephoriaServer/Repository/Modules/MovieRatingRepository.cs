using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository.EntityFramwork;

namespace CinephoriaServer.Repository
{
    public interface IMovieRatingRepository : IReadRepository<MovieRating>, IWriteRepository<MovieRating>
    {
        /// <summary>
        /// Soumet un avis sur un film de la part d'un utilisateur.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="rating">La note attribuée au film.</param>
        /// <param name="description">La description de l'avis.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task SubmitMovieReviewAsync(int movieId, string userId, int rating, string description);

        /// <summary>
        /// Valide un avis sur un film (réservé aux administrateurs).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à valider.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task ValidateReviewAsync(int reviewId);

        /// <summary>
        /// Supprime un avis sur un film (réservé aux administrateurs).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à supprimer.</param>
        /// <returns>Une tâche asynchrone.</returns>
        Task DeleteReviewAsync(int reviewId);

        /// <summary>
        /// Récupère la liste des avis associés à un film.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste d'avis.</returns>
        Task<List<MovieRating>> GetMovieReviewsAsync(int movieId);
    }
    public class MovieRatingRepository
    {
    }
}
