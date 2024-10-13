using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CinephoriaServer.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public AuthService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;

        }

        public async Task<GeneralServiceResponse> CreateDefaultUsersRoleAsync()
        {
            bool isGlobalAdminRoleExists = await _roleManager.RoleExistsAsync(RoleConfigurations.GlobalAdmin);
            bool isCineAdminRoleExists = await _roleManager.RoleExistsAsync(RoleConfigurations.CineAdmin);
            bool isEmployeeRoleExists = await _roleManager.RoleExistsAsync(RoleConfigurations.Employee);
            bool isUserRoleExists = await _roleManager.RoleExistsAsync(RoleConfigurations.User);

            if (isGlobalAdminRoleExists && isCineAdminRoleExists && isEmployeeRoleExists && isUserRoleExists)
                return new GeneralServiceResponse()
                {
                    IsSucceed = true,
                    StatusCode = 200,
                    Message = "Roles Seeding is Already Done"
                };

            await _roleManager.CreateAsync(new IdentityRole(RoleConfigurations.GlobalAdmin));
            await _roleManager.CreateAsync(new IdentityRole(RoleConfigurations.CineAdmin));
            await _roleManager.CreateAsync(new IdentityRole(RoleConfigurations.Employee));
            await _roleManager.CreateAsync(new IdentityRole(RoleConfigurations.User));

            return new GeneralServiceResponse()
            {
                IsSucceed = true,
                StatusCode = 201,
                Message = "Roles Seeding Done Successfully"
            };
        }



        public async Task<GeneralServiceResponse> RegisterAsync(RegisterViewModel registerViewModel)
        {
            // Vérifie si l'utilisateur existe déjà (basé sur l'email)
            var isExistsUser = await _userManager.FindByNameAsync(registerViewModel.UserName);
            if (isExistsUser != null)
            {
                return new GeneralServiceResponse()
                {
                    IsSucceed = false,
                    StatusCode = 409,
                    Message = "UserName Already Exists"
                };
            }

            // Vérifie si les mots de passe correspondent
            if (registerViewModel.Password != registerViewModel.ConfirmPassword)
            {
                return new GeneralServiceResponse()
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Passwords do not match"
                };
            }

            // Création de l'utilisateur avec les informations fournies
            var newUser = new AppUser()
            {
                UserName = registerViewModel.UserName,
                Email = registerViewModel.UserName,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            // Création de l'utilisateur avec le mot de passe hashé
            var createUserResult = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            // Si la création échoue, retourne les erreurs retournées par ASP.NET Identity
            if (!createUserResult.Succeeded)
            {
                var errorString = string.Join(" ", createUserResult.Errors.Select(e => e.Description));
                return new GeneralServiceResponse()
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "User creation failed: " + errorString
                };
            }

            // Assigner le rôle par défaut à l'utilisateur (par exemple "User")
            var addRoleResult = await _userManager.AddToRoleAsync(newUser, "User");

            // Si l'assignation de rôle échoue, retourner un message d'erreur
            if (!addRoleResult.Succeeded)
            {
                var roleErrorString = string.Join(" ", addRoleResult.Errors.Select(e => e.Description));
                return new GeneralServiceResponse()
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "Failed to assign role: " + roleErrorString
                };
            }

            // Retourne un message de succès si tout s'est bien passé
            return new GeneralServiceResponse()
            {
                IsSucceed = true,
                StatusCode = 201,
                Message = "User Created Successfully"
            };
        }

        public async Task<LoginResponseViewModel?> LoginAsync(LoginViewModel loginViewModel)
        {
            // Recherche de l'utilisateur par son nom d'utilisateur
            var user = await _userManager.FindByNameAsync(loginViewModel.UserName);
            if (user is null)
            {
                return null; // L'utilisateur n'existe pas
            }

            // Vérification du mot de passe
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
            if (!isPasswordValid)
            {
                return null; // Mot de passe incorrect
            }

            // Génération du jeton JWT pour l'utilisateur
            var token = await GenerateJWTTokenAsync(user);

            // Récupération des rôles de l'utilisateur
            var roles = await _userManager.GetRolesAsync(user);

            // Création d'un UserInfo à renvoyer au client
            var userInfo = GenerateUserInfoObject(user, roles);

            return new LoginResponseViewModel()
            {
                NewToken = token,
                UserInfo = userInfo
            };
        }
        private UserInfos GenerateUserInfoObject(AppUser user, IEnumerable<string> roles)
        {
            return new UserInfos()
            {
                AppUserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt, 
                Role = roles.ToList() 
            };
        }

        private async Task<string> GenerateJWTTokenAsync(AppUser user)
        {
            // Récupère les rôles de l'utilisateur
            var userRoles = await _userManager.GetRolesAsync(user);

            // Crée les claims pour le token JWT
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName)
        };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var signingCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

            // Construction du token JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),  // Durée de validité du token (3 heures)
                claims: authClaims,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
