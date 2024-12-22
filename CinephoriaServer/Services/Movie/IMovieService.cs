using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;

namespace CinephoriaServer.Services
{
    public interface IMovieService
    {
        /// <summary>
        /// Récupère tous les films disponibles.
        /// </summary>
        /// <returns>Une liste de tous les films disponibles.</returns>
        Task<List<MovieViewModel>> GetAllMoviesAsync();

        /// <summary>
        /// Récupère tous les films disponibles dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">Identifiant du cinéma.</param>
        /// <returns>Une liste de films disponibles dans le cinéma spécifié.</returns>
        Task<List<MovieViewModel>> GetMoviesByCinemaAsync(int cinemaId);

        /// <summary>
        /// Récupère les détails d'un film spécifique, y compris les séances disponibles.
        /// </summary>
        /// <param name="filmId">L'identifiant unique du film.</param>
        /// <returns>Les détails du film, y compris les séances associées.</returns>
        Task<MovieViewModel> GetMovieByIdAsync(string filmId);

        /// <summary>
        /// Filtre les films par cinéma, genre ou jour spécifique.
        /// Si un paramètre n'est pas fourni, il n'est pas pris en compte dans le filtrage.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <param name="genre">Le genre du film.</param>
        /// <param name="date">Le jour spécifique des séances.</param>
        /// <returns>Une liste filtrée des films selon les critères fournis.</returns>
        Task<List<MovieViewModel>> FilterMoviesAsync(int? cinemaId, string genre, DateTime? date);

        /// <summary>
        /// Crée un nouveau film.
        /// Cette méthode est réservée aux administrateurs et aux employés.
        /// </summary>
        /// <param name="movieViewModel">Le ViewModel du film à créer.</param>
        /// <returns>Le film nouvellement créé.</returns>
        Task<GeneralServiceResponse> CreateMovieAsync(MovieViewModel movieViewModel);

        /// <summary>
        /// Modifie un film existant.
        /// Cette méthode est réservée aux administrateurs et aux employés.
        /// </summary>
        /// <param name="filmId">L'identifiant unique du film à modifier.</param>
        /// <param name="movieViewModel">Les nouvelles données du film à mettre à jour.</param>
        /// <returns>Le film mis à jour.</returns>
        Task<GeneralServiceResponse> UpdateMovieAsync(string filmId, MovieViewModel movieViewModel);

        /// <summary>
        /// Supprime un film existant.
        /// Cette méthode est réservée aux administrateurs et aux employés.
        /// </summary>
        /// <param name="filmId">L'identifiant unique du film à supprimer.</param>
        /// <returns>Une tâche représentant l'opération de suppression.</returns>
        Task<GeneralServiceResponse> DeleteMovieAsync(string filmId);

        /// <summary>
        /// Permet à un utilisateur de laisser un avis sur un film.
        /// </summary>
        /// <param name="reviewViewModel">Le ViewModel de l'avis à soumettre.</param>
        /// <returns>L'avis nouvellement créé.</returns>
        Task<Review> SubmitReviewAsync(ReviewViewModel reviewViewModel);

        /// <summary>
        /// Récupère tous les avis d'un film.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste d'avis associés au film.</returns>
        Task<GeneralServiceResponseData<List<ReviewDto>>> GetReviewsByMovieIdAsync(string movieId);

        /// <summary>
        /// Permet à un utilisateur de laisser une note sur un film.
        /// </summary>
        /// <param name="movieRatingViewModel">Le ViewModel de la note à soumettre.</param>
        /// <returns>La note nouvellement créée.</returns>
        Task<MovieRating> SubmitMovieRatingAsync(MovieRatingViewModel movieRatingViewModel);

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

        Task<bool> AddPosterToMovieAsync(string movieId, string imageUrl);
    }

}
