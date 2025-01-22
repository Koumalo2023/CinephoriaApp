using CinephoriaServer.Models.PostgresqlDb;

namespace CinephoriaServer.Services
{
    public interface IMovieRatingService
    {
        /// <summary>
        /// Soumet un avis sur un film de la part d'un utilisateur.
        /// </summary>
        /// <param name="createMovieRatingDto">Les données de l'avis à soumettre.</param>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        Task<string> SubmitMovieReviewAsync(CreateMovieRatingDto createMovieRatingDto, string userId);

        /// <summary>
        /// Valide un avis sur un film (réservé aux administrateurs et employées).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à valider.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        Task<string> ValidateReviewAsync(int reviewId);

        /// <summary>
        /// Supprime un avis sur un film (réservé aux administrateurs et employées).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à supprimer.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        Task<string> DeleteReviewAsync(int reviewId);
    }
}
