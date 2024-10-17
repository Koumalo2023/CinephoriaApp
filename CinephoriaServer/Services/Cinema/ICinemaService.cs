using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;

namespace CinephoriaServer.Services
{
    public interface ICinemaService
    {
        /// <summary>
        /// Crée un nouveau cinéma dans la base de données.
        /// </summary>
        Task<GeneralServiceResponseData<object>> CreateCinemaAsync(CinemaViewModel cinemaViewModel);

        /// <summary>
        /// Modifie un cinéma existant dans la base de données.
        /// </summary>
        Task<GeneralServiceResponse> UpdateCinemaAsync(int cinemaId, CinemaViewModel cinemaViewModel);

        /// <summary>
        /// Supprime un cinéma de la base de données.
        /// </summary>
        Task<GeneralServiceResponse> DeleteCinemaAsync(int cinemaId);

        /// <summary>
        /// Récupère les informations de tous les cinémas.
        /// </summary>
        Task<IEnumerable<CinemaDto>> GetAllCinemasAsync();

        /// <summary>
        /// Crée une nouvelle salle de projection pour un cinéma spécifique.
        /// </summary>
        Task<GeneralServiceResponseData<object>> CreateTheaterForCinemaAsync(TheaterViewModel theaterViewModel);

        /// <summary>
        /// Modifie une salle de projection existante.
        /// </summary>
        Task<GeneralServiceResponse> UpdateTheaterAsync(string theaterId, TheaterViewModel theaterViewModel);

        /// <summary>
        /// Supprime une salle de projection existante.
        /// </summary>
        Task<GeneralServiceResponse> DeleteTheaterAsync(string theaterId);

        /// <summary>
        /// Récupère toutes les salles d'un cinéma spécifique.
        /// </summary>
        Task<IEnumerable<TheaterDto>> GetTheatersByCinemaAsync(int cinemaId);

        Task<GeneralServiceResponseData<object>> GetTheaterByIdAsync(string theaterId);
    }
}
