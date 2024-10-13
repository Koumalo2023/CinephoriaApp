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


        /// <summary>
        /// Authentifie un utilisateur avec son nom d'utilisateur et son mot de passe.
        /// </summary>
        /// <param name="loginViewModel">Les informations de connexion de l'utilisateur.</param>
        /// <returns>Un LoginResponseViewModel contenant le token JWT et les informations utilisateur, ou null en cas d'échec.</returns>
        Task<LoginResponseViewModel?> LoginAsync(LoginViewModel loginViewModel);
    }
}
