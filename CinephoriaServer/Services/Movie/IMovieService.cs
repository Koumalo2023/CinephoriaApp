using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Services
{
    public interface IMovieService
    {
        /// <summary>
        /// Récupère la liste des derniers films ajoutés.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        Task<List<MovieDto>> GetRecentMoviesAsync();

        /// <summary>
        /// Récupère la liste de tous les films.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        Task<List<MovieDto>> GetAllMoviesAsync();

        /// <summary>
        /// Récupère les détails d'un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Un objet Movie contenant les détails du film.</returns>
        Task<MovieDetailsDto> GetMovieDetailsAsync(int movieId);


        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances.</returns>
        Task<List<ShowtimeDto>> GetMovieSessionsAsync(int movieId);

        /// <summary>
        /// Soumet un avis sur un film de la part d'un utilisateur.
        /// </summary>
        /// <param name="reviewDto">Les données de l'avis.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        Task<bool> SubmitMovieReviewAsync(MovieReviewDto reviewDto);

        /// <summary>
        /// Filtre les films en fonction du cinéma, du genre et de la date.
        /// </summary>
        /// <param name="filterDto">Les critères de filtrage.</param>
        /// <returns>Une liste de films correspondant aux critères.</returns>
        Task<List<MovieDto>> FilterMoviesAsync(int? cinemaId, MovieGenre? genre, DateTime? date);

        /// <summary>
        /// Crée un nouveau film.
        /// </summary>
        /// <param name="createMovieDto">Les données du film à créer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        Task<bool> CreateMovieAsync(CreateMovieDto createMovieDto);

        /// <summary>
        /// Ajoute une affiche à un film existant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <param name="posterUrl">L'URL de l'affiche à ajouter.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        Task<bool> AddPosterToMovieAsync(int movieId, string posterUrl);

        /// <summary>
        /// Supprime une affiche d'un film existant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <param name="posterUrl">L'URL de l'affiche à supprimer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        Task<bool> RemovePosterFromMovieAsync(int movieId, string posterUrl);

        /// <summary>
        /// Met à jour les informations d'un film existant.
        /// </summary>
        /// <param name="updateMovieDto">Les nouvelles données du film.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        Task<bool> UpdateMovieAsync(UpdateMovieDto updateMovieDto);

        /// <summary>
        /// Supprime un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film à supprimer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        Task<bool> DeleteMovieAsync(int movieId);
    }

}
