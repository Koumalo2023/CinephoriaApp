using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        //[Authorize(Roles = "GlobalAdmin")] // Autoriser seulement l'administrateur global
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
        [HttpPost("register")]
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
    }
}
