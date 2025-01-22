using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class CreateAppUserDto
    {
        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le nom de famille est obligatoire.")]
        [StringLength(50, ErrorMessage = "Le nom de famille ne peut pas dépasser 50 caractères.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "L'adresse e-mail est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'adresse e-mail n'est pas valide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire.")]
        [StringLength(50, ErrorMessage = "Le nom d'utilisateur ne peut pas dépasser 50 caractères.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir entre 8 et 100 caractères.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir entre 8 et 100 caractères.")]
        public string ConfirmPassword { get; set; }

        public bool HasApprovedTermsOfUse { get; set; }

        public DateTime? HiredDate { get; set; }

        [StringLength(100, ErrorMessage = "Le poste ne peut pas dépasser 100 caractères.")]
        public string Position { get; set; }

        [Url(ErrorMessage = "L'URL de l'image de profil n'est pas valide.")]
        public string ProfilePictureUrl { get; set; }

        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Le rôle est obligatoire.")]
        public UserRole Role { get; set; }
    }
}
