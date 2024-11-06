using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class UpdateUserViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Position { get; set; }

        /// <summary>
        /// Photo de profile des utilisateurs
        /// Applicable aux employés.
        /// </summary>
        public string ProfileImageUrl { get; set; }
    }
}
