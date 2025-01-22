
using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Models.PostgresqlDb.Auth.AppUserDto;
using Microsoft.AspNetCore.Identity;

namespace CinephoriaServer.Services
{
    public interface IAuthService
    {
        // Gestion des utilisateurs

        /// <summary>
        /// Met à jour l'image de profil d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="imageUrl">L'URL de la nouvelle image de profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> UpdateProfileImageAsync(string userId, string imageUrl);

        /// <summary>
        /// Supprime l'image de profil d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="imageUrl">L'URL de l'image de profil à supprimer.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> RemoveProfileImageAsync(string userId, string imageUrl);

        /// <summary>
        /// Enregistre un nouvel utilisateur.
        /// </summary>
        /// <param name="registerUserDto">Les données de l'utilisateur à enregistrer.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> RegisterUserAsync(RegisterUserDto registerUserDto);

        /// <summary>
        /// Enregistre un nouvel employé ou administrateur.
        /// </summary>
        /// <param name="createEmployeeDto">Les données de l'employé ou de l'administrateur à enregistrer.</param>
        /// <param name="currentUserRole">Le rôle de l'utilisateur actuel.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> RegisterEmployeeOrAdminAsync(CreateEmployeeDto createEmployeeDto, string currentUserRole);

        /// <summary>
        /// Confirme l'adresse email d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="token">Le jeton de confirmation.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> ConfirmEmailAsync(string userId, string token);


        /// <summary>
        /// Permet à un utilisateur de se connecter en vérifiant ses informations d'identification.
        /// </summary>
        /// <param name="loginUserDto">Les informations de connexion de l'utilisateur.</param>
        /// <returns>Un jeton JWT si la connexion est réussie, ou un message d'erreur.</returns>
        Task<string> LoginAsync(LoginUserDto loginUserDto);

        /// <summary>
        /// Récupère la liste de tous les utilisateurs enregistrés.
        /// </summary>
        /// <returns>Une liste d'utilisateurs avec leurs informations.</returns>
        Task<List<AppUserDto>> GetAllUsersAsync();

        /// <summary>
        /// Récupère un utilisateur spécifique par son identifiant.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Les informations de l'utilisateur.</returns>
        Task<AppUserDto> GetUserByIdAsync(string userId);

        /// <summary>
        /// Met à jour le profil d'un utilisateur spécifique.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="updateAppUserDto">Les nouvelles données du profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> UpdateUserProfileAsync(string userId, UpdateAppUserDto updateAppUserDto);

        /// <summary>
        /// Met à jour le profil d'un employé spécifique.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé.</param>
        /// <param name="updateEmployeeDto">Les nouvelles données du profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> UpdateEmployeeProfileAsync(string employeeId, UpdateEmployeeDto updateEmployeeDto);

        /// <summary>
        /// Récupère le profil d'un utilisateur spécifique.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Le profil de l'utilisateur.</returns>
        Task<UserProfileDto> GetUserProfileAsync(string userId);

        /// <summary>
        /// Récupère les réservations d'un utilisateur spécifique.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Les réservations de l'utilisateur.</returns>
        Task<List<ReservationDto>> GetUserOrdersAsync(string userId);

        /// <summary>
        /// Récupère le profil d'un employé spécifique.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé.</param>
        /// <returns>Le profil de l'employé.</returns>
        Task<EmployeeProfileDto> GetEmployeeProfileAsync(string employeeId);


        // Gestion des mot de passe(Demande de changement & Réinitialisation)
        Task<string> ForgotPasswordAsync(RequestPasswordResetDto request);

        Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

        Task<string> ValidateResetTokenAsync(string userId, string token);

        Task<string> ForcePasswordResetAsync(string userId);

        Task<string> ForceEmployeePasswordChangeAsync(string userId);

        Task<string> ChangeEmployeePasswordAsync(ChangeEmployeePasswordDto changePasswordDto);
    }
}
