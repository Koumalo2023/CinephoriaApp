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
        Task<GeneralServiceResponse> RegisterAsync(RegisterViewModel registerViewModel, string currentUserRole);

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
        Task<IEnumerable<AppUserDto>> GetUsersListAsync();

        /// <summary>
        /// Récupère les détails d'un utilisateur spécifique via son nom d'utilisateur.
        /// </summary>
        /// <param name="userName">Le nom d'utilisateur de l'utilisateur à rechercher.</param>
        /// <returns>Un UserInfoViewModel contenant les informations de l'utilisateur, ou null si l'utilisateur n'existe pas.</returns>
        Task<AppUserDto?> GetUserDetailsByUserNameAsync(string userName);

        /// <summary>
        /// Authentifie un utilisateur en utilisant son nom d'utilisateur et son mot de passe.
        /// </summary>
        /// <param name="loginViewModel">Les informations de connexion de l'utilisateur.</param>
        /// <returns>Un objet LoginResponseViewModel contenant le token JWT et les informations utilisateur, ou null en cas d'échec de l'authentification.</returns>
        Task<LoginResponseViewModel?> LoginAsync(string login, string password);

        /// <summary>
        /// Modifie le mot de passe d'un utilisateur après vérification de l'ancien mot de passe.
        /// </summary>
        /// <param name="changePasswordViewModel">Objet contenant les informations nécessaires au changement de mot de passe, y compris l'ancien et le nouveau mot de passe.</param>
        /// <returns>Un GeneralServiceResponse indiquant le succès ou l'échec de l'opération de changement de mot de passe.</returns>
        
        Task<GeneralServiceResponse> ChangePasswordAsync(UserChangePasswordViewModel changePasswordViewModel);

        /// <summary>
        /// Met à jour les informations d'un utilisateur sans permettre de modifier son rôle.
        /// </summary>
        /// <param name="updateUserViewModel">Objet contenant les nouvelles informations de l'utilisateur à mettre à jour.</param>
        /// <returns>Un GeneralServiceResponse indiquant le succès ou l'échec de l'opération de mise à jour.</returns>
        Task<GeneralServiceResponse> UpdateUserAsync(UpdateUserViewModel updateUserViewModel);

        /// <summary>
        /// Modifie le rôle d'un employé. Un administrateur peut changer le rôle d'un employé en rôle administrateur ou inversement.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur dont le rôle doit être modifié.</param>
        /// <param name="newRole">Le nouveau rôle à attribuer à l'utilisateur.</param>
        /// <returns>Un GeneralServiceResponse indiquant le succès ou l'échec de l'opération de changement de rôle.</returns>
        Task<GeneralServiceResponse> ChangeEmployeeRoleAsync(string userId, string newRole);

        /// <summary>
        /// Réinitialise le mot de passe d'un utilisateur (employé ou administrateur) avec un nouveau mot de passe défini par un administrateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur dont le mot de passe doit être réinitialisé.</param>
        /// <param name="newPassword">Le nouveau mot de passe à attribuer à l'utilisateur.</param>
        /// <returns>Un GeneralServiceResponse indiquant le succès ou l'échec de l'opération de réinitialisation du mot de passe.</returns>
        Task<GeneralServiceResponse> ResetEmployeePasswordAsync(string userId, string newPassword);

        /// <summary>
        /// Récupère les informations d'un employé à partir de son identifiant utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Un objet AppUserDto contenant les informations de l'utilisateur, ou null si l'utilisateur n'existe pas.</returns>
        Task<AppUserDto?> GetEmployeeByIdAsync(string userId);

        /// <summary>
        /// Supprime un utilisateur de la base de données, quel que soit son rôle (administrateur, employé ou utilisateur).
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur à supprimer.</param>
        /// <returns>Un GeneralServiceResponse indiquant le succès ou l'échec de l'opération de suppression.</returns>
        Task<GeneralServiceResponse> DeleteUserAsync(string userId);

    }
}
