using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.Models.MongooDb
{
    public class ResetPasswordByIdViewModel
    {
        [Required(ErrorMessage = "EmployeeId is required")]
        public string EmployeeId { get; set; }

        [Required(ErrorMessage = "NewPassword is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 6 characters long")]
        public string NewPassword { get; set; }
    }
}
