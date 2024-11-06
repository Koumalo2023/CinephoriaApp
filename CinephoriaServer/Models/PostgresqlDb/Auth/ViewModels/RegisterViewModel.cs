using CinephoriaServer.Configurations;
using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Numéro de téléphone de l'employé.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Date d'embauche de l'employé.
        /// </summary>
        public DateTime? HiredDate { get; set; }

        /// <summary>
        /// Poste ou fonction de l'employé.
        /// </summary>
        public string? Position { get; set; }

        public IEnumerable<string>? Roles { get; set; } = new List<string> { RoleConfigurations.User };

    }
}
