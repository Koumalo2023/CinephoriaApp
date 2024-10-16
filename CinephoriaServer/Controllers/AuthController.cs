using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Crée les rôles d'utilisateur par défaut si ils n'existent pas déjà.
        /// </summary>
        /// <returns>Un objet GeneralServiceResponse indiquant le résultat de l'opération.</returns>
        [HttpPost("create-default-roles")]
        //[Authorize(Roles = "Admin")]
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
                var result = await _authService.RegisterAsync(registerViewModel);
                if (!result.IsSucceed)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (not shown for brevity)
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
                // Log the exception (not shown for brevity)
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
                // Log the exception (not shown for brevity)
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "An error occurred while retrieving users list: " + ex.Message
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
                    return NotFound("User not found.");
                }
                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                // Log the exception (not shown for brevity)
                return StatusCode(500, new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "An error occurred while retrieving user details: " + ex.Message
                });
            }
        }

        
        [HttpPut("users/update-user")]
        [Authorize(Roles = RoleConfigurations.AdminEmployeeUser)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserViewModel updateUserViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Invalid user data"
                });
            }

            var result = await _authService.UpdateUserAsync(updateUserViewModel);

            if (!result.IsSucceed)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

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
                    Message = "Invalid password data"
                });
            }

            var result = await _authService.ChangePasswordAsync(changePasswordViewModel);

            if (!result.IsSucceed)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }




        // Endpoint pour la connexion des utilisateurs et employés/admins

        /// <summary>
        /// Authentifie un utilisateur avec son nom d'utilisateur et son mot de passe.
        /// </summary>
        /// <param name="loginViewModel">Les informations de connexion de l'utilisateur.</param>
        /// <returns>Un LoginResponseViewModel contenant le token JWT et les informations utilisateur, ou null en cas d'échec.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new GeneralServiceResponse()
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Invalid login data"
                });
            }

            // Appel du service pour tenter la connexion de l'utilisateur ou de l'employé
            var loginResponse = await _authService.LoginAsync(loginViewModel.UserName, loginViewModel.Password);

            if (loginResponse == null)
            {
                // Si les informations de connexion sont incorrectes
                return Unauthorized(new GeneralServiceResponse()
                {
                    IsSucceed = false,
                    StatusCode = 401,
                    Message = "Invalid credentials"
                });
            }

            return Ok(new GeneralServiceResponseData<LoginResponseViewModel>()
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "Login successful",
                Data = loginResponse
            });
        }




        // Endpoint pour l'enregistrement des employés/admins

        [HttpPost("employees/register-employee")]
        [Authorize(Roles = RoleConfigurations.Admin)]
        public async Task<IActionResult> RegisterEmployee([FromBody] EmployeeRegisterViewModel employeeRegisterViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterEmployeeAsync(employeeRegisterViewModel);
            if (result.IsSucceed)
            {
                return StatusCode(result.StatusCode, new { message = result.Message });
            }
            return StatusCode(result.StatusCode, new { message = result.Message });
        }
       
        //// <summary>
        /// Modifie le rôle d'un employé.
        /// </summary>
        /// <param name="updateRoleViewModel">Le modèle contenant l'identifiant de l'employé et le nouveau rôle.</param>
        /// <returns>Un objet GeneralServiceResponse indiquant le succès ou l'échec de l'opération.</returns>
        [HttpPut("employees/change-role")]
        [Authorize(Roles = RoleConfigurations.Admin)]
        public async Task<IActionResult> ChangeEmployeeRoleAsync([FromBody] UpdateRoleByIdViewModel updateRoleViewModel)
        {
            var response = await _authService.ChangeEmployeeRoleAsync(updateRoleViewModel);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Réinitialise le mot de passe d'un employé.
        /// </summary>
        /// <param name="resetPasswordViewModel">Le modèle contenant l'identifiant de l'employé et le nouveau mot de passe.</param>
        /// <returns>Un objet GeneralServiceResponse indiquant le succès ou l'échec de l'opération.</returns>
        [HttpPost("employees/reset-password")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> ResetEmployeePasswordAsync([FromBody] ResetPasswordByIdViewModel resetPasswordViewModel)
        {
            var response = await _authService.ResetEmployeePasswordAsync(resetPasswordViewModel);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("UpdateEmployee/{id}")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> UpdateEmployee(string id, [FromBody] EmployeeUpdateViewModel employeeUpdateViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.UpdateEmployeeAsync(id, employeeUpdateViewModel);

            if (!response.IsSucceed)
            {
                return StatusCode(response.StatusCode, response.Message);
            }

            return Ok(response.Message);
        }


        /// <summary>
        /// Récupère la liste de tous les employés.
        /// </summary>
        /// <returns>Un objet GeneralServiceResponseData contenant la liste des employés.</returns>
        [HttpGet("employees")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> GetAllEmployeesAsync()
        {
            var response = await _authService.GetAllEmployeesAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Récupère les détails d'un employé par son identifiant.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé.</param>
        /// <returns>Un objet GeneralServiceResponse contenant les détails de l'employé.</returns>
        [HttpGet("employees/{employeeId}")]
        [Authorize(Roles = RoleConfigurations.AdminEmployee)]
        public async Task<IActionResult> GetEmployeeByIdAsync(string employeeId)
        {
            var response = await _authService.GetEmployeeByIdAsync(employeeId);

            if (response == null)
            {
                // Si l'employé n'est pas trouvé, renvoie une réponse 404
                return NotFound(new { Message = "Employee not found" });
            }

            // Si l'employé est trouvé, renvoie une réponse 200 avec les détails de l'employé
            return Ok(new { Employee = response });
        }

        /// <summary>
        /// Supprime un employé par son identifiant.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé à supprimer.</param>
        /// <returns>Un objet GeneralServiceResponse indiquant le succès ou l'échec de l'opération.</returns>
        [HttpDelete("employees/{employeeId}")]
        [Authorize(Roles = RoleConfigurations.Admin)]
        public async Task<IActionResult> DeleteEmployeeAsync(string employeeId)
        {
            var response = await _authService.DeleteEmployeeAsync(employeeId);
            return StatusCode(response.StatusCode, response);
        }





    }
}
