namespace CinephoriaServer.Models.PostgresqlDb
{
    public class UserInfos
    {
        public string AppUserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();


    }
}
