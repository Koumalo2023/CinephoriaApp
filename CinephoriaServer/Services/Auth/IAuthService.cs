using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;

namespace CinephoriaServer.Services
{
    public interface IAuthService
    {
        Task<GeneralServiceResponse> CreateDefaultUsersRoleAsync();

        /// <summary>
        /// Inscrit un nouvel utilisateur avec les informations fournies.
        /// </summary>
        /// <param name="registerViewModel">Les informations de l'utilisateur à enregistrer.</param>
        /// <returns>Un objet GeneralServiceResponse contenant le statut de l'opération.</returns>
        Task<GeneralServiceResponse> RegisterAsync(RegisterViewModel registerViewModel);
    }
}
