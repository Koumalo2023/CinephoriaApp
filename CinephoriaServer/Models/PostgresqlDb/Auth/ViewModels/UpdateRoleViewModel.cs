using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class UpdateRoleViewModel
    {
        [Required(ErrorMessage = " UserName is required")]
        public string UserName { get; set; }
        public RoleType NewRole { get; set; }
    }

    public enum RoleType
    {
        Admin,
        Employee,
        User
    }
}
