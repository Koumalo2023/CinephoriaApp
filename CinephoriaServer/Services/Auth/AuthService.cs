using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using Microsoft.AspNetCore.Identity;

namespace CinephoriaServer.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

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
    }
}
