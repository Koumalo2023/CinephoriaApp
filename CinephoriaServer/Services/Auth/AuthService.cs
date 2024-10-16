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
        private readonly IMongoCollection<EmployeeAccount> _employeeCollection;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWorkMongoDb _unitOfWork;
        private readonly IMapper _mapper;
        public AuthService(UserManager<AppUser> userManager, IUnitOfWorkMongoDb unitOfWork, IMapper mapper, IMongoDatabase database, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _employeeCollection = database.GetCollection<EmployeeAccount>("EmployeeAccount");
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
            var newToken =  GenerateJWTToken(user.Id, user.UserName, user.FirstName, user.LastName, role.ToList());

            // Mapping AppUser vers UserInfos
            var userInfo = _mapper.Map<UserInfos>(user);

            return new LoginResponseViewModel
            {
                NewToken = newToken,
                UserInfo = userInfo
            };
        }

        public async Task<UserInfos?> GetUserDetailsByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return null;

            // Mapping AppUser vers UserInfos
            var userInfo = _mapper.Map<UserInfos>(user);

            return userInfo;
        }

        public async Task<IEnumerable<UserInfos>> GetUsersListAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            // Mapping de la liste AppUser vers une liste UserInfos
            var userInfosList = _mapper.Map<IEnumerable<UserInfos>>(users);

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




        // Gestion des employée & &Admin

        // Implémentation pour l'enregistrement des employés/admins
        public async Task<GeneralServiceResponse> RegisterEmployeeAsync(EmployeeRegisterViewModel employeeRegisterViewModel)
        {
            // Vérifie si un employé existe déjà (basé sur l'email)
            var existingEmployee = await _employeeCollection.Find(e => e.UserName == employeeRegisterViewModel.UserName).FirstOrDefaultAsync();
            if (existingEmployee != null)
            {
                return new GeneralServiceResponse()
                {
                    IsSucceed = false,
                    StatusCode = 409,
                    Message = "Employee already exists"
                };
            }

            // Hashage du mot de passe de l'employé
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(employeeRegisterViewModel.Password);

            // Création de l'objet EmployeeAccount
            var newEmployee = new EmployeeAccount()
            {
                UserName = employeeRegisterViewModel.UserName,
                PasswordHash = passwordHash,
                FirstName = employeeRegisterViewModel.FirstName,
                LastName = employeeRegisterViewModel.LastName,
                PhoneNumber = employeeRegisterViewModel.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                Roles = new List<string> { employeeRegisterViewModel.IsAdmin ? "Admin" : "Employee" },
                HiredDate = employeeRegisterViewModel.HiredDate,
                Position = employeeRegisterViewModel.Position
            };

            // Ajout dans la base de données MongoDB
            await _employeeCollection.InsertOneAsync(newEmployee);

            return new GeneralServiceResponse()
            {
                IsSucceed = true,
                StatusCode = 201,
                Message = "Employee and Identity user created successfully"
            };
        }

        public async Task<GeneralServiceResponse> UpdateEmployeeAsync(string employeeId, EmployeeUpdateViewModel employeeUpdateViewModel)
        {
            // Convertir l'ID de l'employé en ObjectId
            if (!ObjectId.TryParse(employeeId, out ObjectId objectId))
            {
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Invalid employee ID format"
                };
            }

            // Recherche de l'employé par ObjectId
            var existingEmployee = await _employeeCollection.Find(e => e.Id == objectId).FirstOrDefaultAsync();

            if (existingEmployee == null)
            {
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Employee not found"
                };
            }

            // Mise à jour des champs modifiables
            existingEmployee.FirstName = employeeUpdateViewModel.FirstName ?? existingEmployee.FirstName;
            existingEmployee.LastName = employeeUpdateViewModel.LastName ?? existingEmployee.LastName;
            existingEmployee.PhoneNumber = employeeUpdateViewModel.PhoneNumber ?? existingEmployee.PhoneNumber;
            existingEmployee.Position = employeeUpdateViewModel.Position ?? existingEmployee.Position;
            existingEmployee.HiredDate = employeeUpdateViewModel.HiredDate ?? existingEmployee.HiredDate;

            // Si le rôle est mis à jour
            if (employeeUpdateViewModel.Roles != null && employeeUpdateViewModel.Roles.Any())
            {
                existingEmployee.Roles = employeeUpdateViewModel.Roles;
            }

            // Mise à jour dans la base de données MongoDB
            await _employeeCollection.ReplaceOneAsync(e => e.Id == objectId, existingEmployee);

            return new GeneralServiceResponse
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "Employee updated successfully"
            };
        }


        public async Task<GeneralServiceResponse> ChangeEmployeeRoleAsync(UpdateRoleByIdViewModel updateRoleViewModel)
        {
            // Recherche de l'employé par EmployeeId
            var employeeAccount = await _unitOfWork.EmployeeAccounts.GetByIdAsync(updateRoleViewModel.EmployeeId);

            if (employeeAccount == null)
            {
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Employee not found"
                };
            }

            // Conversion du nouveau rôle en chaîne de caractères et assignation
            employeeAccount.Roles = new List<string> { updateRoleViewModel.NewRole.ToString() };

            // Mise à jour de l'employé dans la base de données
            await _unitOfWork.EmployeeAccounts.UpdateAsync(employeeAccount);

            return new GeneralServiceResponse
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "Employee role changed successfully"
            };
        }

        public async Task<GeneralServiceResponse> ResetEmployeePasswordAsync(ResetPasswordByIdViewModel resetPasswordViewModel)
        {
            // Recherche de l'employé par EmployeeId
            var employee = await _unitOfWork.EmployeeAccounts.GetByIdAsync(resetPasswordViewModel.EmployeeId);

            if (employee == null)
            {
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Employee not found"
                };
            }

            // Hachage du nouveau mot de passe
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordViewModel.NewPassword);

            // Mise à jour du mot de passe
            employee.PasswordHash = passwordHash;
            await _unitOfWork.EmployeeAccounts.UpdateAsync(employee);

            return new GeneralServiceResponse
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "Password reset successfully"
            };
        }

        public async Task<GeneralServiceResponse> DeleteEmployeeAsync(string employeeId)
        {
            var employee = await _unitOfWork.EmployeeAccounts.GetByIdAsync(employeeId);

            if (employee == null)
            {
                return new GeneralServiceResponse
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Employee not found"
                };
            }

            await _unitOfWork.EmployeeAccounts.DeleteAsync(employeeId);

            return new GeneralServiceResponse
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "Employee deleted successfully"
            };
        }

        public async Task<EmployeeAccount?> GetEmployeeByIdAsync(string employeeId)
        {
            var employee = await _unitOfWork.EmployeeAccounts.GetByIdAsync(employeeId);

            if (employee == null)
            {
                return null;
            }

            return employee;
        }

        public async Task<GeneralServiceResponseData<List<EmployeeAccount>>> GetAllEmployeesAsync()
        {
            var employees = (await _unitOfWork.EmployeeAccounts.GetAllAsync()).ToList();

            return new GeneralServiceResponseData<List<EmployeeAccount>>
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "All employees retrieved successfully",
                Data = employees
            };
        }

        public async Task<List<EmployeeAccount>> FilterEmployeesAsync(string? firstName = null, string? lastName = null, string? email = null)
        {
            var filterBuilder = Builders<EmployeeAccount>.Filter;
            var filters = new List<FilterDefinition<EmployeeAccount>>();

            if (!string.IsNullOrEmpty(firstName))
            {
                filters.Add(filterBuilder.Regex(e => e.FirstName, new BsonRegularExpression(firstName, "i")));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                filters.Add(filterBuilder.Regex(e => e.LastName, new BsonRegularExpression(lastName, "i")));
            }

            if (!string.IsNullOrEmpty(email))
            {
                filters.Add(filterBuilder.Regex(e => e.UserName, new BsonRegularExpression(email, "i")));
            }

            var finalFilter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;

            return await _unitOfWork.EmployeeAccounts.FilterAsync(finalFilter);
        }



        //Methodes communes aux utilisateurs et aux employés & Admin
        public async Task<LoginResponseViewModel?> LoginAsync(string login, string password)
        {
            bool isEmployee = login.EndsWith("@cinephoria.com");

            if (isEmployee)
            {
                var employee = await _employeeCollection.Find(e => e.UserName == login).FirstOrDefaultAsync();
                if (employee == null || !BCrypt.Net.BCrypt.Verify(password, employee.PasswordHash))
                {
                    return null;
                }

                // Synchroniser l'utilisateur avec ASP.NET Identity
                var identityUser = await _userManager.FindByNameAsync(login);
                if (identityUser == null)
                {
                    // Si l'utilisateur n'existe pas dans Identity, on le crée
                    identityUser = new AppUser
                    {
                        UserName = employee.UserName,
                        Email = employee.UserName,
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        CreatedAt = DateTime.UtcNow
                    };

                    await _userManager.CreateAsync(identityUser, password);

                    // Assigner les rôles à l'utilisateur dans ASP.NET Identity
                    foreach (var role in employee.Roles)
                    {
                        await _userManager.AddToRoleAsync(identityUser, role);
                    }
                }

                // Générer le token JWT avec les rôles
                var roles = await _userManager.GetRolesAsync(identityUser);
                var token = GenerateJWTToken(identityUser.Id, identityUser.UserName, identityUser.FirstName, identityUser.LastName, roles.ToList());

                var userInfo = new UserInfos
                {
                    AppUserId = employee.Id.ToString(),
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    UserName = employee.UserName,
                    Email = employee.UserName,
                    Roles = roles, // Utiliser la liste des rôles
                    CreatedAt = employee.CreatedAt
                };

                return new LoginResponseViewModel
                {
                    NewToken = token,
                    UserInfo = userInfo
                };
            }
            else
            {
                // Gestion de l'authentification des utilisateurs classiques via ASP.NET Identity
                var user = await _userManager.FindByNameAsync(login);
                if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                {
                    return null;
                }

                var roles = await _userManager.GetRolesAsync(user);
                var token = GenerateJWTToken(user.Id, user.UserName, user.FirstName, user.LastName, roles.ToList());

                var userInfo = new UserInfos
                {
                    AppUserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles,
                    CreatedAt = user.CreatedAt
                };

                return new LoginResponseViewModel
                {
                    NewToken = token,
                    UserInfo = userInfo
                };
            }
        }



        private string GenerateJWTToken(string userId, string userName, string firstName, string lastName, List<string> roles)
        {
            // Création des claims
                    var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("FirstName", firstName),
                new Claim("LastName", lastName)
            };

            // Ajout des rôles dans les claims (si plusieurs rôles sont présents)
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var signingCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

            // Génération du token JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private UserInfos GenerateUserInfoObject(AppUser user, string role)
        {
            return new UserInfos()
            {
                AppUserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Roles = user.Roles
            };
        }

    }
}
