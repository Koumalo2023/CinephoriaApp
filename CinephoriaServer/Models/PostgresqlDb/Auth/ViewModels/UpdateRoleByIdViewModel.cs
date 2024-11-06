using static CinephoriaServer.Configurations.EnumConfig;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.Models.PostgresqlDb.Auth.ViewModels
{
    public class UpdateRoleByIdViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string NewRole { get; set; } = string.Empty;
    }
}
