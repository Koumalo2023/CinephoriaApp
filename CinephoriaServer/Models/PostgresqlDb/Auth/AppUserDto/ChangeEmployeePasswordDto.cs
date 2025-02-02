using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class ChangeEmployeePasswordDto
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string AppUserId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir entre 8 et 100 caractères.")]
        public string NewPassword { get; set; }
    }
}
