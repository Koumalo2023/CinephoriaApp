using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.Models.PostgresqlDb.Auth.AppUserDto
{
    public class ResetEmployeePasswordDto
    {
        /// <summary>
        /// Identifiant unique de l'employé.
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Nouveau mot de passe de l'employé.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir entre 8 et 100 caractères.")]
        public string NewPassword { get; set; }
    }
}
