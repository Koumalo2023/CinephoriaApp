namespace CinephoriaServer.Models.MongooDb
{
    public class EmployeeUpdateViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? HiredDate { get; set; }
        public string? Position { get; set; }
        public List<string>? Roles { get; set; }
    }
}
