﻿using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Models.PostgresqlDb.Auth.AppUserDto;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly IImageService _imageService;
        private readonly IRoleService _roleService;

        public AuthController(IAuthService authService, IImageService imageService, IRoleService roleService)
        {
            _authService = authService;
            _imageService = imageService;
            _roleService = roleService;
        }

        /// <summary>
        /// Télécharge une image de profil pour un utilisateur spécifique.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="file">Le fichier image à télécharger.</param>
        /// <returns>L'URL de l'image téléchargée ou un message d'erreur.</returns>
        [HttpPost("upload-user-profile/{userId}")]
        public async Task<IActionResult> UploadUserProfile(string userId, [FromForm, Required] IFormFile file)
        {
            try
            {
                string folder = "users";
                var imageUrl = await _imageService.UploadImageAsync(file, folder);
                if (imageUrl == null)
                {
                    return BadRequest(new { Message = "Erreur lors du téléchargement de l'image." });
                }

                var result = await _authService.UpdateProfileImageAsync(userId, imageUrl);
                return Ok(new { Message = result, Url = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors du téléchargement de l'image de profil." });
            }
        }

        /// <summary>
        /// Supprime l'image de profil d'un utilisateur spécifique.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="imageUrl">L'URL de l'image à supprimer.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        [HttpDelete("delete-user-profile-image/{userId}")]
        public async Task<IActionResult> DeleteUserProfileImage(string userId, [FromQuery] string imageUrl)
        {
            try
            {
                var result = await _authService.RemoveProfileImageAsync(userId, imageUrl);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la suppression de l'image de profil." });
            }
        }

        /// <summary>
        /// Crée les rôles par défaut pour les utilisateurs (Admin, Employee, User).
        /// </summary>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        [HttpPost("create-default-roles")]
        public async Task<IActionResult> CreateDefaultRoles()
        {
            try
            {
                var result = await _roleService.CreateDefaultRolesAsync();
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la création des rôles par défaut." });
            }
        }

        /// <summary>
        /// Enregistre un nouvel utilisateur.
        /// </summary>
        /// <param name="registerUserDto">Les données de l'utilisateur à enregistrer.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
        {
            try
            {
                var result = await _authService.RegisterUserAsync(registerUserDto);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de l'enregistrement de l'utilisateur." });
            }
        }


        /// <summary>
        /// Crée un compte employé ou administrateur avec un mot de passe temporaire.
        /// </summary>
        /// <param name="createEmployeeDto">Les informations de création ou de l'administrateur du compte employé.</param>
        /// <returns>Un message indiquant si la création a réussi.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("register-employee")]
        public async Task<IActionResult> RegisterEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            try
            {
                var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var result = await _authService.RegisterEmployeeOrAdminAsync(createEmployeeDto, currentUserRole);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de l'enregistrement de l'employé ou de l'administrateur." });
            }
        }

        /// <summary>
        /// Connecte un utilisateur en vérifiant ses informations d'identification.
        /// </summary>
        /// <param name="loginUserDto">Les informations de connexion de l'utilisateur.</param>
        /// <returns>
        /// Un jeton JWT et le profil de l'utilisateur si la connexion est réussie,
        /// ou un message d'erreur en cas d'échec.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            try
            {
                // Appeler le service d'authentification pour se connecter
                var loginResponse = await _authService.LoginAsync(loginUserDto);

                // Si le token est null, cela signifie qu'une erreur s'est produite (utilisateur non trouvé, mot de passe incorrect, etc.)
                if (loginResponse.Token == null)
                {
                    return BadRequest(new { Message = loginResponse.Profile });
                }

                // Retourner le token et le profil encapsulés dans LoginResponseDto
                return Ok(loginResponse);
            }
            catch (ApiException ex)
            {
                // Gérer les erreurs spécifiques (par exemple, utilisateur non trouvé, mot de passe incorrect, etc.)
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la connexion." });
            }
        }

        /// <summary>
        /// Récupère la liste de tous les utilisateurs enregistrés.
        /// </summary>
        /// <returns>Une liste d'utilisateurs avec leurs informations.</returns>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la récupération des utilisateurs." });
            }
        }

        /// <summary>
        /// Récupère un utilisateur spécifique par son identifiant.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Les informations de l'utilisateur.</returns>
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { Message = "Utilisateur non trouvé." });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la récupération de l'utilisateur." });
            }
        }

        /// <summary>
        /// Récupère le profil d'un employé spécifique.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé.</param>
        /// <returns>Le profil de l'employé.</returns>
        [Authorize(Roles = "Employee,Admin")]
        [HttpGet("employee-profile/{employeeId}")]
        public async Task<IActionResult> GetEmployeeProfile(string employeeId)
        {
            try
            {
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (currentUserRole != UserRole.Employee.ToString() && currentUserRole != UserRole.Admin.ToString())
                {
                    return StatusCode(403, new { Message = "Vous n'êtes pas autorisé à accéder à ce profil." });
                }

                var employeeProfile = await _authService.GetEmployeeProfileAsync(employeeId);
                if (employeeProfile == null)
                {
                    return NotFound(new { Message = "Employé non trouvé." });
                }
                return Ok(employeeProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la récupération du profil de l'employé." });
            }
        }

        /// <summary>
        /// Récupère le profil d'un utilisateur spécifique.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Le profil de l'utilisateur.</returns>
        [Authorize]
        [HttpGet("user-profile/{userId}")]
        public async Task<IActionResult> GetUserProfile(string userId)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId != userId)
                {
                    return StatusCode(403, new { Message = "Vous n'êtes pas autorisé à accéder à ce profil." });
                }

                var userProfile = await _authService.GetUserProfileAsync(userId);
                if (userProfile == null)
                {
                    return NotFound(new { Message = "Utilisateur non trouvé." });
                }
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la récupération du profil utilisateur." });
            }
        }

        /// <summary>
        /// Met à jour le profil d'un utilisateur spécifique.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="updateAppUserDto">Les nouvelles données du profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        [Authorize]
        [HttpPut("update-profile/{userId}")]
        public async Task<IActionResult> UpdateUserProfile(string userId, [FromBody] UpdateAppUserDto updateAppUserDto)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId != userId)
                {
                    return StatusCode(403, new { Message = "Vous n'êtes pas autorisé à mettre à jour ce profil." });
                }

                var result = await _authService.UpdateUserProfileAsync(userId, updateAppUserDto);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la mise à jour du profil utilisateur." });
            }
        }

        /// <summary>
        /// Met à jour le profil d'un employé spécifique.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé.</param>
        /// <param name="updateEmployeeDto">Les nouvelles données du profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        [Authorize(Roles = "Employee,Admin")]
        [HttpPut("update-employee-profile/{employeeId}")]
        public async Task<IActionResult> UpdateEmployeeProfile(string employeeId, [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId != employeeId && !User.IsInRole("Admin"))
                {
                    return StatusCode(403, new { Message = "Vous n'êtes pas autorisé à mettre à jour ce profil." });
                }

                var result = await _authService.UpdateEmployeeProfileAsync(employeeId, updateEmployeeDto);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la mise à jour du profil employé." });
            }
        }

        /// <summary>
        /// Confirme l'adresse email d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="token">Le jeton de confirmation.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                var result = await _authService.ConfirmEmailAsync(userId, token);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la confirmation de l'email." });
            }
        }


        // Gestion de changement de mot-de-passe

        /// <summary>
        /// Demande de réinitialisation de mot de passe pour un utilisateur normal.
        /// </summary>
        /// <param name="request">Les informations de demande de réinitialisation (e-mail).</param>
        /// <returns>Un message indiquant si la demande a été traitée avec succès.</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] RequestPasswordResetDto request)
        {
            try
            {
                var result = await _authService.ForgotPasswordAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {Message = "Une erreur s'est produite lors de la demande de réinitialisation de mot de passe " });
            }
        }

        /// <summary>
        /// Réinitialise le mot de passe d'un utilisateur normal.
        /// </summary>
        /// <param name="resetPasswordDto">Les informations de réinitialisation (token et nouveau mot de passe).</param>
        /// <returns>Un message indiquant si la réinitialisation a réussi.</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var result = await _authService.ResetPasswordAsync(resetPasswordDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur s'est produite lors de la réinitialisation du mot de passe" });
            }
        }

        /// <summary>
        /// Valide un jeton de réinitialisation de mot de passe.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="token">Le jeton de réinitialisation.</param>
        /// <returns>Un message indiquant si le jeton est valide.</returns>
        [HttpGet("validate-reset-token")]
        public async Task<IActionResult> ValidateResetToken([FromQuery] string userId, [FromQuery] string token)
        {
            try
            {
                var result = await _authService.ValidateResetTokenAsync(userId, token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur s'est produite lors de la validation du jeton" });
                
            }
        }

        /// <summary>
        /// Force la réinitialisation du mot de passe d'un utilisateur normal.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Un message indiquant si la réinitialisation forcée a réussi.</returns>
        [HttpPost("force-password-reset")]
        public async Task<IActionResult> ForcePasswordReset([FromQuery] string userId)
        {
            try
            {
                var result = await _authService.ForcePasswordResetAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur s'est produite lors de la réinitialisation forcée du mot de passe" });
            }
        }


        /// <summary>
        /// Permet à un employé de changer son mot de passe après avoir utilisé un mot de passe temporaire.
        /// </summary>
        /// <param name="changePasswordDto">Les informations de changement de mot de passe.</param>
        /// <returns>Un message indiquant si le changement de mot de passe a réussi.</returns>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangeEmployeePassword([FromBody] ChangeEmployeePasswordDto changePasswordDto)
        {
            try
            {
                var result = await _authService.ChangeEmployeePasswordAsync(changePasswordDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur s'est produite lors du changement de mot de passe." });
                
            }
        }

        /// <summary>
        /// Force un employé à changer son mot de passe (par exemple, si le mot de passe temporaire a expiré).
        /// </summary>
        /// <param name="userId">L'identifiant de l'employé.</param>
        /// <returns>Un message indiquant si la réinitialisation forcée a réussi.</returns>
        [HttpPost("force-password-change")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ForceEmployeePasswordChange([FromQuery] string userId)
        {
            try
            {
                var result = await _authService.ForceEmployeePasswordChangeAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur s'est produite lors de la réinitialisation forcée du mot de passe : " });
            }
        }

    }
}
