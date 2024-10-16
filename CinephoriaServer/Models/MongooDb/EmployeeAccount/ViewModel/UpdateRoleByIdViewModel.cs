using static CinephoriaServer.Configurations.EnumConfig;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.Models.MongooDb
{
    public class UpdateRoleByIdViewModel
    {
        [Required(ErrorMessage = "EmployeeId is required")]
        public string EmployeeId { get; set; }

        [Required(ErrorMessage = "NewRole is required")]
        public UserRole NewRole { get; set; }
    }
}
