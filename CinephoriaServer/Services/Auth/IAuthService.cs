using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using MongoDB.Bson;
using System.Security.Claims;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Services
{
    public interface IAuthService
    {
        // Gestion des utilisateurs


        Task<GeneralServiceResponse> CreateDefaultUsersRoleAsync();
        /// <summary>
        /// Inscrit un nouvel utilisateur avec les informations fournies.
        /// </summary>
        /// <param name="registerViewModel">Les informations de l'utilisateur à enregistrer.</param>
        /// <returns>Un objet GeneralServiceResponse contenant le statut de l'opération.</returns>
        Task<GeneralServiceResponse> RegisterAsync(RegisterViewModel registerViewModel);

        /// <summary>
        /// Valide le token JWT d'un utilisateur et renvoie ses informations ainsi qu'un nouveau token.
        /// </summary>
        /// <param name="meViewModel">Les informations du token JWT actuel de l'utilisateur.</param>
        /// <returns>Un LoginResponseViewModel contenant un nouveau token JWT et les informations utilisateur, ou null en cas d'échec.</returns>
        Task<LoginResponseViewModel?> MeAsync(MeViewModel meViewModel);

        /// <summary>
        /// Récupère la liste des utilisateurs enregistrés et leurs informations.
        /// </summary>
        /// <returns>Une liste d'objets UserInfoViewModel contenant les informations de tous les utilisateurs.</returns>
        Task<IEnumerable<UserInfos>> GetUsersListAsync();

        /// <summary>
        /// Récupère les détails d'un utilisateur spécifique via son nom d'utilisateur.
        /// </summary>
        /// <param name="userName">Le nom d'utilisateur de l'utilisateur à rechercher.</param>
        /// <returns>Un UserInfoViewModel contenant les informations de l'utilisateur, ou null si l'utilisateur n'existe pas.</returns>
        Task<UserInfos?> GetUserDetailsByUserNameAsync(string userName);

        Task<GeneralServiceResponse> ChangePasswordAsync(UserChangePasswordViewModel changePasswordViewModel);
        Task<GeneralServiceResponse> UpdateUserAsync(UpdateUserViewModel updateUserViewModel);


        // Methode cocmmune aux utilisateurs et aux emplyés
        /// <summary>
        /// Authentifie un utilisateur avec son nom d'utilisateur et son mot de passe.
        /// </summary>
        /// <param name="loginViewModel">Les informations de connexion de l'utilisateur.</param>
        /// <returns>Un LoginResponseViewModel contenant le token JWT et les informations utilisateur, ou null en cas d'échec.</returns>
        Task<LoginResponseViewModel?> LoginAsync(string login, string password);

        // Gestion des emplyés
        Task<GeneralServiceResponse> RegisterEmployeeAsync(EmployeeRegisterViewModel employeeRegisterViewModel);
        Task<GeneralServiceResponse> ChangeEmployeeRoleAsync(UpdateRoleByIdViewModel updateRoleViewModel);
        Task<GeneralServiceResponse> ResetEmployeePasswordAsync(ResetPasswordByIdViewModel resetPasswordViewModel);
        Task<GeneralServiceResponse> UpdateEmployeeAsync(string employeeId, EmployeeUpdateViewModel employeeUpdateViewModel);
        Task<GeneralServiceResponseData<List<EmployeeAccount>>> GetAllEmployeesAsync();
        Task<EmployeeAccount?> GetEmployeeByIdAsync(string employeeId);
        Task<GeneralServiceResponse> DeleteEmployeeAsync(string employeeId);
        Task<List<EmployeeAccount>> FilterEmployeesAsync(string? firstName = null, string? lastName = null, string? email = null);
    }
}
