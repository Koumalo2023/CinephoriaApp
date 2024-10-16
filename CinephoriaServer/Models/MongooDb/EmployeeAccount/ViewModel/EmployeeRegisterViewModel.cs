using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.Models.MongooDb
{
    public class EmployeeRegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public DateTime HiredDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string Position { get; set; }

        public bool IsAdmin { get; set; }
    }

}
