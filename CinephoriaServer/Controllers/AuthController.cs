using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Models.PostgresqlDb.Auth.ViewModels;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly IImageService _imageService;

        public AuthController(IAuthService authService, IImageService imageService)
        {
            _authService = authService;
            _imageService = imageService;
        }

        /// <summary>
        /// Crée les rôles d'utilisateur par défaut si ils n'existent pas déjà.
        /// </summary>
        /// <returns>Un objet GeneralServiceResponse indiquant le résultat de l'opération.</returns>
        [HttpPost("create-default-roles")]
        [Authorize(Roles = RoleConfigurations.Admin)]
        public async Task<ActionResult<GeneralServiceResponse>> CreateDefaultUsersRoleAsync()
        {
            try
            {
                var result = await _authService.CreateDefaultUsersRoleAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (not shown for brevity)
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "An error occurred while creating default roles: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Inscrit un nouvel utilisateur avec les informations fournies.
        /// </summary>
        /// <param name="registerViewModel">Les informations de l'utilisateur à enregistrer.</param>
        /// <returns>Un objet GeneralServiceResponse contenant le statut de l'opération.</returns>
        [HttpPost("users/register")]
        public async Task<ActionResult<GeneralServiceResponse>> RegisterAsync([FromBody] RegisterViewModel registerViewModel)
        {
            try
            {
                // Récupère le rôle de l'utilisateur actuel à partir du contexte d'authentification
                var currentUserRole = User.IsInRole("Admin") ? "Admin" : "User";

                // Appelle la méthode RegisterAsync en passant le rôle de l'utilisateur actuel
                var result = await _authService.RegisterAsync(registerViewModel, currentUserRole);

                // Vérifie si l'inscription a échoué et renvoie une réponse avec le code d'erreur approprié
                if (!result.IsSucceed)
                {
                    return StatusCode(result.StatusCode, result);
                }

                // Renvoie une réponse 201 Created si l'inscription a réussi
                return StatusCode(201, result);
            }
            catch (Exception ex)
            {
                // Log l'exception (ici on utilise une simple sortie console pour l'exemple)
                Console.WriteLine($"Exception lors de l'inscription de l'utilisateur : {ex.Message}");

                // Retourne une réponse 500 en cas d'erreur serveur
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "An error occurred while registering the user: " + ex.Message
                });
            }
        }


        /// <summary>
        /// Valide le token JWT d'un utilisateur et renvoie ses informations ainsi qu'un nouveau token.
        /// </summary>
        /// <param name="meViewModel">Les informations du token JWT actuel de l'utilisateur.</param>
        /// <returns>Un LoginResponseViewModel contenant un nouveau token JWT et les informations utilisateur, ou null en cas d'échec.</returns>
        [HttpPost("users/me")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<ActionResult<LoginResponseViewModel?>> MeAsync([FromBody] MeViewModel meViewModel)
        {
            try
            {
                var result = await _authService.MeAsync(meViewModel);
                if (result == null)
                {
                    return Unauthorized("Invalid token.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "An error occurred while retrieving user info: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Récupère la liste des utilisateurs enregistrés et leurs informations.
        /// </summary>
        /// <returns>Une liste d'objets UserInfoViewModel contenant les informations de tous les utilisateurs.</returns>
        [HttpGet("users")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<ActionResult<IEnumerable<UserInfos>>> GetUsersListAsync()
        {
            try
            {
                var users = await _authService.GetUsersListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "Une erreur est survenue lors de la récupération de la liste des utilisateurs : " + ex.Message
                });
            }
        }


        /// <summary>
        /// Récupère les détails d'un utilisateur spécifique via son nom d'utilisateur.
        /// </summary>
        /// <param name="userName">Le nom d'utilisateur de l'utilisateur à rechercher.</param>
        /// <returns>Un UserInfoViewModel contenant les informations de l'utilisateur, ou null si l'utilisateur n'existe pas.</returns>
        [HttpGet("user/{userName}")]
        [Authorize(Roles = RoleConfigurations.AdminEmployeeUser)]
        public async Task<ActionResult<UserInfos?>> GetUserDetailsByUserNameAsync(string userName)
        {
            try
            {
                var userInfo = await _authService.GetUserDetailsByUserNameAsync(userName);
                if (userInfo == null)
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        IsSucceed = false,
                        StatusCode = 404,
                        Message = "Utilisateur introuvable."
                    });
                }
                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "Une erreur est survenue lors de la récupération des détails de l'utilisateur : " + ex.Message
                });
            }
        }


        /// <summary>
        /// Met à jour un utilisateur (Admin, Employé ou Utilisateur).
        /// </summary>
        /// <param name="model">Modèle contenant les détails à mettre à jour.</param>
        /// <returns>Un message de confirmation ou une réponse d'erreur.</returns>
        [HttpPut("users/update-user")]
        [Authorize(Roles = RoleConfigurations.AdminEmployeeUser)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserViewModel updateUserViewModel, [FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Les informations fournies sont incorrectes"
                });
            }

            try
            {
                // Téléchargez la nouvelle image de profil, puis mettez à jour `ProfileImageUrl`
                if (file != null)
                {
                    string folder = "users";
                    var imageUrl = await _imageService.UploadImageAsync(file, folder);
                    updateUserViewModel.ProfileImageUrl = imageUrl; 
                }

                var result = await _authService.UpdateUserAsync(updateUserViewModel);
                if (!result.IsSucceed)
                {
                    return StatusCode(result.StatusCode, result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "Une erreur est survenue lors de la mise à jour de l'utilisateur : " + ex.Message
                });
            }
        }


        /// <summary>
        /// Modifie le mot de passe d'un utilisateur après vérification de l'ancien mot de passe.
        /// </summary>
        /// <param name="changePasswordViewModel">Objet contenant les informations nécessaires au changement de mot de passe, y compris l'ancien et le nouveau mot de passe.</param>
        /// <returns>Un GeneralServiceResponse indiquant le succès ou l'échec de l'opération de changement de mot de passe.</returns>
        [HttpPost("users/change-password")]
        [Authorize(Roles = RoleConfigurations.AdminEmployeeUser)]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordViewModel changePasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Les informations de mot de passe sont incorrectes"
                });
            }

            try
            {
                var result = await _authService.ChangePasswordAsync(changePasswordViewModel);

                if (!result.IsSucceed)
                {
                    return StatusCode(result.StatusCode, result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "Une erreur est survenue lors du changement de mot de passe : " + ex.Message
                });
            }
        }


        /// <summary>
        /// Authentifie un utilisateur avec son nom d'utilisateur et son mot de passe.
        /// </summary>
        /// <param name="loginViewModel">Les informations de connexion de l'utilisateur.</param>
        /// <returns>Un LoginResponseViewModel contenant le token JWT et les informations utilisateur, ou null en cas d'échec.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            try
            {
               
                var loginResponse = await _authService.LoginAsync(loginViewModel.UserName, loginViewModel.Password);

                if (loginResponse == null)
                {
                    return Unauthorized(new GeneralServiceResponse()
                    {
                        IsSucceed = false,
                        StatusCode = 401,
                        Message = "Les informations fournies sont incorrects"
                    });
                }

                return Ok(loginResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred during login: {ex.Message}" });
            }
        }
    
        
        /// <summary>
        /// Réinitialise le mot de passe d'un utilisateur (admin ou employé).
        /// </summary>
        /// <param name="model">Modèle contenant l'ID de l'utilisateur et le nouveau mot de passe.</param>
        /// <returns>Un message de confirmation ou une réponse d'erreur.</returns>
        [HttpPost("reset-password")]
        [Authorize(Roles = RoleConfigurations.Admin)]
        public async Task<IActionResult> ResetEmployeePasswordAsync([FromBody] ResetPasswordViewModel model)
        {
            try
            {
                var response = await _authService.ResetEmployeePasswordAsync(model.UserId, model.NewPassword);
                if (!response.IsSucceed)
                {
                    return Unauthorized(new GeneralServiceResponse()
                    {
                        IsSucceed = false,
                        StatusCode = 401,
                        Message = "Les informations fournies sont incorrects"
                    });
                }
                return Ok("Mot de passe réinitialisé avec succès.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "Une erreur est survenue lors de la réinitialisation du mot de passe : " + ex.Message
                });
            }
        }



        /// <summary>
        /// Change le rôle d'un utilisateur (employé vers admin et réciproquement).
        /// </summary>
        /// <param name="model">Modèle contenant l'ID de l'utilisateur et le nouveau rôle.</param>
        /// <returns>Un message de confirmation ou une réponse d'erreur.</returns>
        [HttpPost("change-role")]
        [Authorize(Roles = RoleConfigurations.Admin)]
        public async Task<IActionResult> ChangeEmployeeRoleAsync([FromBody] UpdateRoleByIdViewModel model)
        {
            try
            {
                var response = await _authService.ChangeEmployeeRoleAsync(model.UserId, model.NewRole);
                if (!response.IsSucceed)
                {
                    return Unauthorized(new GeneralServiceResponse()
                    {
                        IsSucceed = false,
                        StatusCode = 401,
                        Message = "Les informations fournies sont incorrects"
                    });
                }
                return Ok($"Rôle changé avec succès en {model.NewRole}.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "Une erreur est survenue lors du changement de rôle : " + ex.Message
                });
            }
        }


        /// <summary>
        /// Récupère les informations d'un utilisateur spécifique via son identifiant.
        /// </summary>
        /// <param name="userId">L'ID de l'utilisateur à rechercher.</param>
        /// <returns>Un objet contenant les informations de l'utilisateur ou une réponse d'erreur.</returns>
        [HttpGet("get-user/{userId}")]
        [Authorize(Roles = RoleConfigurations.AdminEmployeeUser)]
        public async Task<IActionResult> GetEmployeeByIdAsync(string userId)
        {
            try
            {
                var userInfo = await _authService.GetEmployeeByIdAsync(userId);
                if (userInfo == null)
                {
                    return NotFound(new GeneralServiceResponse()
                    {
                        IsSucceed = false,
                        StatusCode = 401,
                        Message = "Utilisateur non trouvé."
                    });
                }
                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "Une erreur est survenue lors de la récupération des informations de l'utilisateur : " + ex.Message
                });
            }
        }


        /// <summary>
        /// Supprime un utilisateur (Admin, Employé ou Utilisateur).
        /// </summary>
        /// <param name="userId">L'ID de l'utilisateur à supprimer.</param>
        /// <returns>Un message de confirmation ou une réponse d'erreur.</returns>
        [HttpDelete("delete-user/{userId}")]
        [Authorize(Roles = RoleConfigurations.Admin)]
        public async Task<IActionResult> DeleteUserAsync(string userId)
        {
            try
            {
                var response = await _authService.DeleteUserAsync(userId);
                if (!response.IsSucceed)
                {
                    return StatusCode(response.StatusCode, response);
                }
                return Ok("Utilisateur supprimé avec succès.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "Une erreur est survenue lors de la suppression de l'utilisateur : " + ex.Message
                });
            }
        }

    }
}
