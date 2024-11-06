using AutoMapper;
using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWorkMongoDb _unitOfWork;
        private readonly IMapper _mapper;
        public AuthService(UserManager<AppUser> userManager, IUnitOfWorkMongoDb unitOfWork, IMapper mapper, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }

        public async Task<GeneralServiceResponse> CreateDefaultUsersRoleAsync()
        {
            bool isAdminRoleExists = await _roleManager.RoleExistsAsync(RoleConfigurations.Admin);
            bool isEmployeeRoleExists = await _roleManager.RoleExistsAsync(RoleConfigurations.Employee);
            bool isUserRoleExists = await _roleManager.RoleExistsAsync(RoleConfigurations.User);

            if (isAdminRoleExists && isEmployeeRoleExists && isUserRoleExists)
            {
                return new GeneralServiceResponse()
                {
                    IsSucceed = true,
                    StatusCode = 200,
                    Message = "Roles Seeding is Already Done"
                };
            }

            // Crée les rôles s'ils n'existent pas déjà
            await _roleManager.CreateAsync(new IdentityRole(RoleConfigurations.Admin));
            await _roleManager.CreateAsync(new IdentityRole(RoleConfigurations.Employee));
            await _roleManager.CreateAsync(new IdentityRole(RoleConfigurations.User));

            return new GeneralServiceResponse()
            {
                IsSucceed = true,
                StatusCode = 201,
                Message = "Roles Seeding Done Successfully"
            };
        }

        public async Task<GeneralServiceResponse> RegisterAsync(RegisterViewModel registerViewModel, string currentUserRole)
        {
            // Vérifie si l'utilisateur existe déjà (basé sur l'email)
            var isExistsUser = await _userManager.FindByNameAsync(registerViewModel.UserName);
            if (isExistsUser != null)
            {
                return new GeneralServiceResponse()
                {
                    IsSucceed = false,
                    StatusCode = 409,
                    Message = "L'utilisateur existe déjà!"
                };
            }

            // Vérifie si les mots de passe correspondent
            if (registerViewModel.Password != registerViewModel.ConfirmPassword)
            {
                return new GeneralServiceResponse()
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Les mots de passe sont différents"
                };
            }

            // Gestion du rôle par défaut "USER" si aucun rôle ou un rôle incorrect est spécifié
            if (registerViewModel.Roles == null ||
                !registerViewModel.Roles.Any() ||
                registerViewModel.Roles.First().Equals("string", StringComparison.OrdinalIgnoreCase))
            {
                registerViewModel.Roles = new List<string> { RoleConfigurations.User };
            }

            string role = registerViewModel.Roles.First();

            // Vérifie les règles pour les rôles "ADMIN" et "EMPLOYEE"
            if (role == RoleConfigurations.Admin || role == RoleConfigurations.Employee)
            {
                if (currentUserRole != RoleConfigurations.Admin)
                {
                    return new GeneralServiceResponse()
                    {
                        IsSucceed = false,
                        StatusCode = 403,
                        Message = "Seul un administrateur peut créer un autre administrateur ou un employé"
                    };
                }

                // Vérifie que l'adresse email a le suffixe "@cinephoria.com"
                if (!registerViewModel.UserName.EndsWith("@cinephoria.com"))
                {
                    return new GeneralServiceResponse()
                    {
                        IsSucceed = false,
                        StatusCode = 400,
                        Message = "Les comptes administrateurs et employés doivent utiliser une adresse email se terminant par '@cinephoria.com'."
                    };
                }

                // Validation supplémentaire pour les employés
                if (role == RoleConfigurations.Employee)
                {
                    if (string.IsNullOrEmpty(registerViewModel.PhoneNumber) ||
                        !registerViewModel.HiredDate.HasValue ||
                        string.IsNullOrEmpty(registerViewModel.Position))
                    {
                        return new GeneralServiceResponse()
                        {
                            IsSucceed = false,
                            StatusCode = 400,
                            Message = "Le numéro de téléphone, la date d'embauche et le poste de l'employé sont obligatoires"
                        };
                    }
                }
            }
            else if (role != RoleConfigurations.User)
            {
                // Si le rôle n'est ni "USER", ni "EMPLOYEE", ni "ADMIN", renvoie une erreur
                return new GeneralServiceResponse()
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = $"Le rôle spécifié '{role}' n'est pas valide."
                };
            }

            // Création de l'utilisateur avec les informations fournies
            AppUser newUser = new AppUser()
            {
                UserName = registerViewModel.UserName,
                Email = registerViewModel.UserName,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                PhoneNumber = role == RoleConfigurations.Employee ? registerViewModel.PhoneNumber : null,
                HiredDate = role == RoleConfigurations.Employee ? registerViewModel.HiredDate : null,
                Position = role == RoleConfigurations.Employee ? registerViewModel.Position : null
            };

            // Création de l'utilisateur avec le mot de passe hashé
            var createUserResult = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            if (!createUserResult.Succeeded)
            {
                var errorString = string.Join(" ", createUserResult.Errors.Select(e => e.Description));
                return new GeneralServiceResponse()
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Erreur lors de la création de l'utilisateur: " + errorString
                };
            }

            // Assigner le rôle approprié (User, Employee, ou Admin)
            var addRoleResult = await _userManager.AddToRoleAsync(newUser, role);

            if (!addRoleResult.Succeeded)
            {
                var roleErrorString = string.Join(" ", addRoleResult.Errors.Select(e => e.Description));
                return new GeneralServiceResponse()
                {
                    IsSucceed = false,
                    StatusCode = 500,
                    Message = "Le rôle n'a pas été assigné à l'utilisateur en cours de création: " + roleErrorString
                };
            }

            return new GeneralServiceResponse()
            {
                IsSucceed = true,
                StatusCode = 201,
                Message = "Votre inscription a bien été prise en compte"
            };
        }   

        public async Task<LoginResponseViewModel?> MeAsync(MeViewModel meViewModel)
        {
            // Validation du token JWT
            ClaimsPrincipal handler = new JwtSecurityTokenHandler().ValidateToken(meViewModel.Token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidAudience = _configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]))
            }, out SecurityToken securityToken);

            var decodedUserName = handler.Claims.First(q => q.Type == ClaimTypes.Name).Value;
            if (decodedUserName == null)
                return null;

            var user = await _userManager.FindByNameAsync(decodedUserName);
            if (user == null)
                return null;

            var role = await _userManager.GetRolesAsync(user);

            // Génération du nouveau token JWT
            var newToken = await GenerateJWTToken(user);

            var userDto = await GenerateUserInfoObject(user, role);
            // Mapping AppUser vers UserInfos
            var userInfo = new UserInfos()
            {
                AppUserId = userDto.AppUserId,
                UserName = userDto.UserName,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                CreatedAt = userDto.CreatedAt,
                UpdatedAt = userDto.UpdatedAt,
                Roles = userDto.Roles.ToList(),
                HasApprovedTermsOfUse = userDto.HasApprovedTermsOfUse,
                HiredDate = userDto.HiredDate,
                Position = userDto.Position,
                
            };

            return new LoginResponseViewModel
            {
                NewToken = newToken,
                UserInfo = userInfo
            };
        }

        public async Task<AppUserDto?> GetUserDetailsByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            var userInfo = new AppUserDto()
            {
                AppUserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToList(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                HasApprovedTermsOfUse = user.HasApprovedTermsOfUse,
                HiredDate = user.HiredDate,
                Position = user.Position,
                ReportedIncidents = user.ReportedIncidents,
                Reservations = user.Reservations,
                MovieRatings = user.MovieRatings,
                Contact = user.Contact
            };
            return userInfo;
        }

        public async Task<IEnumerable<AppUserDto>> GetUsersListAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            List<AppUserDto> userInfosList = new List<AppUserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Attendre que la tâche se termine pour obtenir le DTO
                var userInfo = await GenerateUserInfoObject(user, roles);
                userInfosList.Add(userInfo);
            }

            return userInfosList;
        }

        public async Task<GeneralServiceResponse> ChangePasswordAsync(UserChangePasswordViewModel changePasswordViewModel)
        {
            // Récupération de l'utilisateur à partir de son nom d'utilisateur (email)
            var user = await _userManager.FindByNameAsync(changePasswordViewModel.UserName);
            if (user == null)
            {
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "User not found"
                };
            }

            // Vérifier si l'ancien mot de passe est correct
            var isOldPasswordValid = await _userManager.CheckPasswordAsync(user, changePasswordViewModel.OldPassword);
            if (!isOldPasswordValid)
            {
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Old password is incorrect"
                };
            }

            // Changer le mot de passe de l'utilisateur
            var result = await _userManager.ChangePasswordAsync(user, changePasswordViewModel.OldPassword, changePasswordViewModel.NewPassword);
            if (!result.Succeeded)
            {
                var errorString = string.Join(" ", result.Errors.Select(e => e.Description));
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Password change failed: " + errorString
                };
            }

            return new GeneralServiceResponse
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "Password changed successfully"
            };
        }

        public async Task<GeneralServiceResponse> UpdateUserAsync(UpdateUserViewModel updateUserViewModel)
        {
            // Récupérer l'utilisateur par son nom d'utilisateur
            var user = await _userManager.FindByNameAsync(updateUserViewModel.UserName);
            if (user == null)
            {
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "User not found"
                };
            }

            // Mise à jour des informations de l'utilisateur
            user.FirstName = updateUserViewModel.FirstName;
            user.LastName = updateUserViewModel.LastName;
            user.Email = updateUserViewModel.Email;
            user.Position = updateUserViewModel.Position;
            user.UserName = updateUserViewModel.UserName;
            user.PhoneNumber = updateUserViewModel.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errorString = string.Join(" ", result.Errors.Select(e => e.Description));
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "User update failed: " + errorString
                };
            }

            return new GeneralServiceResponse
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "User updated successfully"
            };
        }


        // Méthode pour réinitialiser le mot de passe d'un utilisateur
        public async Task<GeneralServiceResponse> ResetEmployeePasswordAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Utilisateur non trouvé."
                };
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (!resetResult.Succeeded)
            {
                var errorString = string.Join(" ", resetResult.Errors.Select(e => e.Description));
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Erreur de réinitialisation du mot de passe: " + errorString
                };
            }

            return new GeneralServiceResponse
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "Mot de passe réinitialisé avec succès."
            };
        }

        // Méthode pour changer le rôle d'un utilisateur
        public async Task<GeneralServiceResponse> ChangeEmployeeRoleAsync(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Utilisateur non trouvé."
                };
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
            {
                var errorString = string.Join(" ", removeResult.Errors.Select(e => e.Description));
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Erreur lors de la suppression des anciens rôles: " + errorString
                };
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, newRole);

            if (!addRoleResult.Succeeded)
            {
                var errorString = string.Join(" ", addRoleResult.Errors.Select(e => e.Description));
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Erreur lors de l'attribution du nouveau rôle: " + errorString
                };
            }

            return new GeneralServiceResponse
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = $"Rôle changé avec succès à {newRole}."
            };
        }

        // Méthode pour supprimer un utilisateur
        public async Task<GeneralServiceResponse> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Utilisateur non trouvé."
                };
            }

            var deleteResult = await _userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
            {
                var errorString = string.Join(" ", deleteResult.Errors.Select(e => e.Description));
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Erreur lors de la suppression de l'utilisateur: " + errorString
                };
            }

            return new GeneralServiceResponse
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "Utilisateur supprimé avec succès."
            };
        }

        // Méthode pour obtenir un utilisateur par son identifiant
        public async Task<AppUserDto?> GetEmployeeByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            return new AppUserDto
            {
                AppUserId = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToList(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                HasApprovedTermsOfUse = user.HasApprovedTermsOfUse,
                HiredDate = user.HiredDate,
                Position = user.Position,
                ReportedIncidents = user.ReportedIncidents,
                Reservations = user.Reservations,
                MovieRatings = user.MovieRatings,
            };
        }

        public async Task<LoginResponseViewModel?> LoginAsync(string login, string password)
        {
            // Cherche l'utilisateur dans ASP.NET Identity par son login
            var identityUser = await _userManager.FindByNameAsync(login);
            if (identityUser == null || !await _userManager.CheckPasswordAsync(identityUser, password))
            {
                return null;
            }

            // Récupère les rôles de l'utilisateur
            var roles = await _userManager.GetRolesAsync(identityUser);

            // Génère le token JWT
            var token = await GenerateJWTToken(identityUser);

            var userInfo = new UserInfos
            {
                AppUserId = identityUser.Id,
                FirstName = identityUser.FirstName,
                LastName = identityUser.LastName,
                UserName = identityUser.UserName,
                Email = identityUser.Email,
                Roles = roles,
                CreatedAt = identityUser.CreatedAt
            };

            // Retourne la réponse avec le token et les informations de l'utilisateur
            return new LoginResponseViewModel
            {
                NewToken = token,
                UserInfo = userInfo
            };
        }

        private async Task<string> GenerateJWTToken(AppUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var signingCredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256);

            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: signingCredentials
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;
        }

        private async Task<AppUserDto> GenerateUserInfoObject(AppUser user, IEnumerable<string> Roles)
        {
            var userDto = new AppUserDto()
            {
                AppUserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Roles = Roles.ToList(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                HasApprovedTermsOfUse = user.HasApprovedTermsOfUse,
                HiredDate = user.HiredDate,
                Position = user.Position
            };

            return userDto;
        }


    }
}
