using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class UpdateRoleViewModel
    {
        [Required(ErrorMessage = " UserName is required")]
        public string UserName { get; set; }
        public UserRole NewRole { get; set; }
    }

}
