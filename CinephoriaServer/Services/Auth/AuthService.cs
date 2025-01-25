using AutoMapper;
using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Models.PostgresqlDb.Auth.AppUserDto;
using CinephoriaServer.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly IImageService _imageService;
        private readonly ILogger<AuthService> _logger;
        private readonly IMapper _mapper;
        public AuthService(UserManager<AppUser> userManager, EmailService emailService, IMapper mapper, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IImageService imageService, ILogger<AuthService> logger, IUnitOfWorkPostgres unitOfWork, IRoleService roleService)   
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _imageService = imageService;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _roleService = roleService;
        }

        /// <summary>
        /// Met à jour l'image de profil d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="imageUrl">L'URL de la nouvelle image de profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> UpdateProfileImageAsync(string userId, string imageUrl)
        {
            // Récupérer l'utilisateur existant
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Mettre à jour l'URL de l'image de profil
            user.ProfilePictureUrl = imageUrl;

            // Mettre à jour l'utilisateur dans la base de données
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return "Erreur lors de la mise à jour de l'image de profil.";
            }

            _logger.LogInformation("Image de profil mise à jour avec succès pour l'utilisateur avec l'ID {UserId}.", userId);
            return "Image de profil mise à jour avec succès.";
        }

        /// <summary>
        /// Supprime l'image de profil d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="imageUrl">L'URL de l'image de profil à supprimer.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> RemoveProfileImageAsync(string userId, string imageUrl)
        {
            // Récupérer l'utilisateur existant
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Supprimer l'image du stockage
            var imageDeleted = await _imageService.DeleteImageAsync(imageUrl);
            if (!imageDeleted)
            {
                return "L'image de profil n'a pas pu être supprimée du stockage.";
            }

            // Supprimer l'URL de l'image de profil de l'utilisateur
            user.ProfilePictureUrl = null;

            // Mettre à jour l'utilisateur dans la base de données
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return "Erreur lors de la suppression de l'image de profil.";
            }

            _logger.LogInformation("Image de profil supprimée avec succès pour l'utilisateur avec l'ID {UserId}.", userId);
            return "Image de profil supprimée avec succès.";
        }

        /// <summary>
        /// Enregistre un nouvel utilisateur.
        /// </summary>
        /// <param name="registerUserDto">Les données de l'utilisateur à enregistrer.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            // Validation de base
            if (registerUserDto.Password != registerUserDto.ConfirmPassword)
            {
                return "Les mots de passe sont différents.";
            }

            // Vérifie si l'utilisateur existe déjà
            var existingUser = await _userManager.FindByEmailAsync(registerUserDto.Email);
            if (existingUser != null)
            {
                return "L'utilisateur existe déjà.";
            }

            // Création de l'utilisateur
            var newUser = new AppUser
            {
                UserName = registerUserDto.Email,
                Email = registerUserDto.Email,
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                HasApprovedTermsOfUse = false
            };

            // Créer l'utilisateur
            var createUserResult = await _userManager.CreateAsync(newUser, registerUserDto.Password);
            if (!createUserResult.Succeeded)
            {
                return "Erreur lors de la création de l'utilisateur: " + string.Join(" ", createUserResult.Errors.Select(e => e.Description));
            }

            // Assigner le rôle à l'utilisateur en utilisant RoleService
            var roleResult = await _roleService.AssignRoleToUserAsync(newUser, EnumConfig.UserRole.User);
            if (!roleResult.Contains("succès"))
            {
                // Si l'assignation du rôle échoue, supprimer l'utilisateur
                await _userManager.DeleteAsync(newUser);
                return roleResult;
            }

            // Envoyer l'email de confirmation
            await SendConfirmationEmailAsync(newUser);

            return "Utilisateur créé avec succès. Un email de confirmation a été envoyé.";
        }

        /// <summary>
        /// Enregistre un nouvel employé ou administrateur.
        /// </summary>
        /// <param name="createEmployeeDto">Les données de l'employé ou de l'administrateur à enregistrer.</param>
        /// <param name="currentUserRole">Le rôle de l'utilisateur actuel.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> RegisterEmployeeOrAdminAsync(CreateEmployeeDto createEmployeeDto, string currentUserRole)
        {

            // Vérifie si l'utilisateur existe déjà
            var existingUser = await _userManager.FindByEmailAsync(createEmployeeDto.Email);
            if (existingUser != null)
            {
                return "L'utilisateur existe déjà.";
            }

            // Vérifie que l'utilisateur actuel est un administrateur
            if (currentUserRole != UserRole.Admin.ToString())
            {
                return "Seul un administrateur peut créer un autre administrateur ou un employé.";
            }

            // Vérifie que l'adresse email se termine par "@cinephoria.com"
            if (!createEmployeeDto.Email.EndsWith("@cinephoria.com"))
            {
                return "Les comptes administrateurs et employés doivent utiliser une adresse email se terminant par '@cinephoria.com'.";
            }

            // Validation supplémentaire pour les employés
            if (createEmployeeDto.Role == UserRole.Employee)
            {
                if (string.IsNullOrEmpty(createEmployeeDto.PhoneNumber) ||
                    !createEmployeeDto.HiredDate.HasValue ||
                    string.IsNullOrEmpty(createEmployeeDto.Position))
                {
                    return "Le numéro de téléphone, la date d'embauche et le poste de l'employé sont obligatoires.";
                }
            }

            // Générer un mot de passe temporaire
            var temporaryPassword = GenerateTemporaryPassword();

            // Création de l'utilisateur
            var newUser = new AppUser
            {
                UserName = createEmployeeDto.Email,
                Email = createEmployeeDto.Email,
                FirstName = createEmployeeDto.FirstName,
                LastName = createEmployeeDto.LastName,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                PhoneNumber = createEmployeeDto.PhoneNumber,
                HiredDate = createEmployeeDto.HiredDate,
                Position = createEmployeeDto.Position,
                ProfilePictureUrl = createEmployeeDto.ProfilePictureUrl,
                Role = createEmployeeDto.Role
            };

            // Créer l'utilisateur avec le mot de passe temporaire
            var createUserResult = await _userManager.CreateAsync(newUser, temporaryPassword);
            if (!createUserResult.Succeeded)
            {
                return "Erreur lors de la création de l'utilisateur "; 
                
            }

            // Assigner le rôle "Employee" à l'utilisateur
            await _userManager.AddToRoleAsync(newUser, UserRole.Employee.ToString());

            // Envoyer l'e-mail avec le mot de passe temporaire et le lien pour changer le mot de passe
            await SendEmployeePasswordResetEmailAsync(newUser, temporaryPassword);

            return "Compte employé créé avec succès. Un e-mail a été envoyé avec un mot de passe temporaire.";
        }

        /// <summary>
        /// Connecte un utilisateur en vérifiant ses informations d'identification.
        /// </summary>
        /// <param name="loginUserDto">Les informations d'identification de l'utilisateur.</param>
        /// <returns>Un jeton JWT en cas de succès, ou un message d'erreur.</returns>
        public async Task<string> LoginAsync(LoginUserDto loginUserDto)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByEmailAsync(loginUserDto.Email);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Vérifier si le mot de passe est correct
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);
            if (!isPasswordValid)
            {
                return "Mot de passe incorrect.";
            }

            // Vérifier si l'email est confirmé
            if (!user.EmailConfirmed)
            {
                return "Veuillez confirmer votre adresse email avant de vous connecter.";
            }

            // Générer le token JWT
            var token = GenerateJwtToken(user);

            return token;
        }

        // Gestion des mot de passe(Demande de changement & Réinitialisation)

        /// <summary>
        /// Demande de réinitialisation de mot de passe pour un utilisateur normal.
        /// </summary>
        /// <param name="request">Les informations de demande de réinitialisation (e-mail).</param>
        /// <returns>Un message indiquant si la demande a été traitée avec succès.</returns>
        public async Task<string> ForgotPasswordAsync(RequestPasswordResetDto request)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Ne pas révéler que l'utilisateur n'existe pas pour des raisons de sécurité
                return "Si l'e-mail existe, un lien de réinitialisation sera envoyé.";
            }

            // Générer un jeton de réinitialisation de mot de passe
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Envoyer l'e-mail de réinitialisation
            await SendResetPasswordEmailAsync(user, resetToken);

            return "Un lien de réinitialisation a été envoyé à votre adresse e-mail.";
        }

        /// <summary>
        /// Réinitialise le mot de passe d'un utilisateur normal.
        /// </summary>
        /// <param name="resetPasswordDto">Les informations de réinitialisation (token et nouveau mot de passe).</param>
        /// <returns>Un message indiquant si la réinitialisation a réussi.</returns>
        public async Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Réinitialiser le mot de passe
            var resetResult = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!resetResult.Succeeded)
            {
                return "Erreur lors de la réinitialisation du mot de passe ";
            }

            return "Votre mot de passe a été réinitialisé avec succès.";
        }

        /// <summary>
        /// Valide un jeton de réinitialisation de mot de passe.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="token">Le jeton de réinitialisation.</param>
        /// <returns>Un message indiquant si le jeton est valide.</returns>
        public async Task<string> ValidateResetTokenAsync(string userId, string token)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Vérifier si le jeton est valide
            var isValidToken = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);
            if (!isValidToken)
            {
                return "Le jeton de réinitialisation est invalide ou a expiré.";
            }

            return "Le jeton de réinitialisation est valide.";
        }

        /// <summary>
        /// Force la réinitialisation du mot de passe d'un utilisateur normal.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Un message indiquant si la réinitialisation forcée a réussi.</returns>
        public async Task<string> ForcePasswordResetAsync(string userId)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Générer un nouveau jeton de réinitialisation
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Envoyer l'e-mail de réinitialisation
            await SendResetPasswordEmailAsync(user, resetToken);

            return "Un nouveau lien de réinitialisation a été envoyé à votre adresse e-mail.";
        }

        /// <summary>
        /// Permet à un employé de changer son mot de passe après avoir utilisé un mot de passe temporaire.
        /// </summary>
        /// <param name="changePasswordDto">Les informations de changement de mot de passe.</param>
        /// <returns>Un message indiquant si le changement de mot de passe a réussi.</returns>
        public async Task<string> ChangeEmployeePasswordAsync(ChangeEmployeePasswordDto changePasswordDto)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByIdAsync(changePasswordDto.UserId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Vérifier si l'ancien mot de passe (temporaire) est correct
            var isOldPasswordValid = await _userManager.CheckPasswordAsync(user, changePasswordDto.OldPassword);
            if (!isOldPasswordValid)
            {
                return "L'ancien mot de passe est incorrect.";
            }

            user.EmailConfirmed = true;

            // Changer le mot de passe
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                return "Erreur lors du changement de mot de passe : ";
            }
           

            return "Votre mot de passe a été changé avec succès.";
        }

        /// <summary>
        /// Force un employé à changer son mot de passe (par exemple, si le mot de passe temporaire a expiré).
        /// </summary>
        /// <param name="userId">L'identifiant de l'employé.</param>
        /// <returns>Un message indiquant si la réinitialisation forcée a réussi.</returns>
        public async Task<string> ForceEmployeePasswordChangeAsync(string userId)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Générer un nouveau mot de passe temporaire
            var temporaryPassword = GenerateTemporaryPassword();

            // Réinitialiser le mot de passe de l'utilisateur avec le mot de passe temporaire
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, temporaryPassword);
            if (!resetResult.Succeeded)
            {
                return "Erreur lors de la réinitialisation du mot de passe ";
            }

            // Envoyer l'e-mail avec le nouveau mot de passe temporaire
            await SendEmployeePasswordResetEmailAsync(user, temporaryPassword);

            return "Un nouveau mot de passe temporaire a été envoyé à l'employé.";
        }


        /// <summary>
        /// Récupère la liste de tous les utilisateurs.
        /// </summary>
        /// <returns>Une liste d'utilisateurs ou un message d'erreur.</returns>
        public async Task<List<AppUserDto>> GetAllUsersAsync()
        {
            // Récupérer tous les utilisateurs
            var users = _userManager.Users.ToList();

            // Mapper les utilisateurs vers AppUserDto
            var userDtos = _mapper.Map<List<AppUserDto>>(users);

            return userDtos;
        }


        /// <summary>
        /// Récupère les détails d'un utilisateur spécifique par son identifiant.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Les détails de l'utilisateur ou un message d'erreur.</returns>
        public async Task<AppUserDto> GetUserByIdAsync(string userId)
        {
            // Récupérer l'utilisateur par son ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null; // Ou retourner un message d'erreur si nécessaire
            }

            // Mapper l'utilisateur vers AppUserDto
            var userDto = _mapper.Map<AppUserDto>(user);

            return userDto;
        }


        /// <summary>
        /// Met à jour le profil d'un utilisateur spécifique.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="updateAppUserDto">Les nouvelles données du profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> UpdateUserProfileAsync(string userId, UpdateAppUserDto updateAppUserDto)
        {
            // Récupérer l'utilisateur par son ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Mettre à jour les informations de l'utilisateur
            user.FirstName = updateAppUserDto.FirstName;
            user.LastName = updateAppUserDto.LastName;
            user.Email = updateAppUserDto.Email;
            user.UserName = updateAppUserDto.UserName;
            user.PhoneNumber = updateAppUserDto.PhoneNumber;
            user.ProfilePictureUrl = updateAppUserDto.ProfilePictureUrl;

            // Mettre à jour l'utilisateur dans la base de données
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return "Erreur lors de la mise à jour du profil : " + string.Join(", ", updateResult.Errors.Select(e => e.Description));
            }

            return "Profil utilisateur mis à jour avec succès.";
        }

        /// <summary>
        /// Récupère le profil d'un utilisateur spécifique.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Le profil de l'utilisateur ou un message d'erreur.</returns>
        public async Task<UserProfileDto> GetUserProfileAsync(string userId)
        {
            // Récupérer le profil utilisateur via le repository
            var user = await _unitOfWork.Users.GetUserProfileAsync(userId);
            if (user == null)
            {
                return null;
            }

            // Mapper l'utilisateur vers UserProfileDto
            var userDto = _mapper.Map<UserProfileDto>(user);

            return userDto;
        }


        /// <summary>
        /// Récupère les réservations d'un utilisateur spécifique.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Les réservations de l'utilisateur ou un message d'erreur.</returns>
        public async Task<List<ReservationDto>> GetUserOrdersAsync(string userId)
        {
            // Récupérer les réservations de l'utilisateur via le repository
            var reservations = await _unitOfWork.Users.GetUserOrdersAsync(userId);
            if (reservations == null || !reservations.Any())
            {
                return null; // Ou retourner un message d'erreur si nécessaire
            }

            // Mapper les réservations vers ReservationDto
            var reservationDtos = _mapper.Map<List<ReservationDto>>(reservations);

            return reservationDtos;
        }


        /// <summary>
        /// Récupère le profil d'un employé spécifique.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé.</param>
        /// <returns>Le profil de l'employé ou un message d'erreur.</returns>
        public async Task<EmployeeProfileDto> GetEmployeeProfileAsync(string employeeId)
        {
            // Récupérer le profil de l'employé via le repository
            var employee = await _unitOfWork.Users.GetEmployeeProfileAsync(employeeId);
            if (employee == null)
            {
                return null; // Ou retourner un message d'erreur si nécessaire
            }

            // Mapper l'employé vers EmployeeProfileDto
            var employeeProfileDto = _mapper.Map<EmployeeProfileDto>(employee);

            return employeeProfileDto;
        }


        /// <summary>
        /// Met à jour le profil d'un employé spécifique.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé.</param>
        /// <param name="updateEmployeeDto">Les nouvelles données du profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> UpdateEmployeeProfileAsync(string employeeId, UpdateEmployeeDto updateEmployeeDto)
        {
            // Récupérer l'employé par son ID
            var employee = await _userManager.FindByIdAsync(employeeId);
            if (employee == null)
            {
                return "Employé non trouvé.";
            }

            // Mettre à jour les informations de l'employé
            employee.FirstName = updateEmployeeDto.FirstName;
            employee.LastName = updateEmployeeDto.LastName;
            employee.Email = updateEmployeeDto.Email;
            employee.PhoneNumber = updateEmployeeDto.PhoneNumber;
            employee.Position = updateEmployeeDto.Position;
            employee.ProfilePictureUrl = updateEmployeeDto.ProfilePictureUrl;

            // Mettre à jour l'employé dans la base de données
            var updateResult = await _userManager.UpdateAsync(employee);
            if (!updateResult.Succeeded)
            {
                return "Erreur lors de la mise à jour du profil : " + string.Join(", ", updateResult.Errors.Select(e => e.Description));
            }

            return "Profil employé mis à jour avec succès.";
        }

        /// <summary>
        /// Confirme l'adresse email d'un utilisateur.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <param name="token">Le jeton de confirmation.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return "L'ID de l'utilisateur ne peut pas être vide.";
            }

            if (string.IsNullOrEmpty(token))
            {
                return "Le token de confirmation ne peut pas être vide.";
            }

            // Récupérer l'utilisateur par son ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Confirmer l'email avec le token
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return "Erreur lors de la confirmation de l'email : " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return "Email confirmé avec succès.";
        }


        private string GenerateJwtToken(AppUser user)
        {
            // Récupérer les rôles de l'utilisateur
            var roles = _userManager.GetRolesAsync(user).Result;

            // Créer les claims (informations sur l'utilisateur)
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
        };

            // Ajouter les rôles de l'utilisateur aux claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Récupérer la clé secrète depuis la configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            // Créer les informations d'identification pour le token
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Configurer le token JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JWT:LifeSpanInDays"])),
                signingCredentials: creds
            );

            // Générer le token sous forme de chaîne
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateTemporaryPassword()
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            var random = new Random();
            var password = new StringBuilder();

            for (int i = 0; i < 12; i++) // Mot de passe de 12 caractères
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }

            return password.ToString();
        }

        private async Task SendConfirmationEmailAsync(AppUser user)
        {
            // Générer le token de confirmation d'email
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Créer le lien de confirmation
            var confirmationLink = $"{_configuration["AppBaseUrl"]}/confirm-email?userId={user.Id}&token={WebUtility.UrlEncode(emailConfirmationToken)}";

            // Charger le template HTML
            var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "Configurations/Templates", "ConfirmationEmail.html");
            var emailTemplate = await File.ReadAllTextAsync(emailTemplatePath);

            // Remplacer les placeholders dans le template
            var emailBody = emailTemplate.Replace("{{ConfirmationLink}}", confirmationLink);

            // Envoyer l'email de confirmation
            var emailSubject = "Confirmez votre adresse email";
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
        }

        private async Task SendResetPasswordEmailAsync(AppUser user, string resetToken)
        {
            // Créer le lien de réinitialisation
            var resetPasswordLink = $"{_configuration["AppBaseUrl"]}/reset-password?userId={user.Id}&token={WebUtility.UrlEncode(resetToken)}";

            // Charger le template HTML
            var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "Configurations/Templates", "ResetPasswordEmail.html");
            var emailTemplate = await File.ReadAllTextAsync(emailTemplatePath);

            // Remplacer les placeholders dans le template
            var emailBody = emailTemplate.Replace("{{ResetPasswordLink}}", resetPasswordLink);

            // Envoyer l'email de réinitialisation
            var emailSubject = "Réinitialisation de votre mot de passe";
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
        }

        private async Task SendEmployeePasswordResetEmailAsync(AppUser user, string temporaryPassword)
        {
            // Créer le lien de changement de mot de passe
            var changePasswordLink = $"{_configuration["AppBaseUrl"]}/change-password?userId={user.Id}";

            // Charger le template HTML
            var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "Configurations/Templates", "EmployeePasswordResetEmail.html");
            var emailTemplate = await File.ReadAllTextAsync(emailTemplatePath);

            // Remplacer les placeholders dans le template
            var emailBody = emailTemplate
                .Replace("{{ChangePasswordLink}}", changePasswordLink)
                .Replace("{{TemporaryPassword}}", temporaryPassword);

            // Envoyer l'email de changement de mot de passe
            var emailSubject = "Changement de mot de passe requis";
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
        }

        private void ValidateUserRegistration(dynamic registerDto)
        {
            // Vérifie si les mots de passe correspondent
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                throw new BadRequestException("Les mots de passe sont différents.");
            }

            // Vérifie la validité du rôle
            if (!Enum.IsDefined(typeof(UserRole), registerDto.Role))
            {
                throw new BadRequestException($"Le rôle spécifié '{registerDto.Role}' n'est pas valide.");
            }

            // Si aucun rôle n'est spécifié, définir le rôle par défaut (User)
            if (registerDto.Role == null)
            {
                registerDto.Role = UserRole.User;
            }
        }
    }
}
