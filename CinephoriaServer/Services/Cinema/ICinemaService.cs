using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;

namespace CinephoriaServer.Services
{
    public interface ICinemaService
    {
        /// <summary>
        /// Crée un nouveau cinéma avec les informations fournies.
        /// </summary>
        /// <param name="cinemaViewModel">Les détails du cinéma à créer.</param>
        /// <returns>Un GeneralServiceResponseData contenant le résultat de la création du cinéma.</returns>
        Task<GeneralServiceResponseData<object>> CreateCinemaAsync(CinemaViewModel cinemaViewModel);

        /// <summary>
        /// Met à jour un cinéma existant avec de nouvelles informations.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma à mettre à jour.</param>
        /// <param name="cinemaViewModel">Les nouvelles informations du cinéma.</param>
        /// <returns>Un GeneralServiceResponse indiquant le succès ou l'échec de la mise à jour.</returns>
        Task<GeneralServiceResponse> UpdateCinemaAsync(int cinemaId, CinemaViewModel cinemaViewModel);

        /// <summary>
        /// Supprime un cinéma spécifique par son identifiant.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma à supprimer.</param>
        /// <returns>Un GeneralServiceResponse indiquant le succès ou l'échec de la suppression.</returns>
        Task<GeneralServiceResponse> DeleteCinemaAsync(int cinemaId);

        /// <summary>
        /// Récupère la liste de tous les cinémas.
        /// </summary>
        /// <returns>Une liste de CinemaDto contenant les informations de chaque cinéma.</returns>
        Task<IEnumerable<CinemaDto>> GetAllCinemasAsync();

        /// <summary>
        /// Crée une nouvelle salle pour un cinéma spécifique.
        /// </summary>
        /// <param name="theaterViewModel">Les informations de la salle à créer.</param>
        /// <returns>Un GeneralServiceResponseData contenant le résultat de la création de la salle.</returns>
        Task<GeneralServiceResponseData<object>> CreateTheaterForCinemaAsync(TheaterViewModel theaterViewModel);

        /// <summary>
        /// Met à jour une salle existante avec de nouvelles informations.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle à mettre à jour.</param>
        /// <param name="theaterViewModel">Les nouvelles informations de la salle.</param>
        /// <returns>Un GeneralServiceResponse indiquant le succès ou l'échec de la mise à jour.</returns>
        Task<GeneralServiceResponse> UpdateTheaterAsync(string theaterId, TheaterViewModel theaterViewModel);

        /// <summary>
        /// Supprime une salle spécifique par son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle à supprimer.</param>
        /// <returns>Un GeneralServiceResponse indiquant le succès ou l'échec de la suppression.</returns>
        Task<GeneralServiceResponse> DeleteTheaterAsync(string theaterId);

        /// <summary>
        /// Récupère la liste des salles d'un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma pour lequel les salles doivent être récupérées.</param>
        /// <returns>Une liste de TheaterDto contenant les informations de chaque salle.</returns>
        Task<IEnumerable<TheaterDto>> GetTheatersByCinemaAsync(int cinemaId);

        /// <summary>
        /// Récupère les détails d'une salle spécifique par son identifiant.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle à récupérer.</param>
        /// <returns>Un GeneralServiceResponseData contenant les informations de la salle.</returns>
        Task<GeneralServiceResponseData<object>> GetTheaterByIdAsync(string theaterId);

    }
}
