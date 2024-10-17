namespace CinephoriaServer.Models.MongooDb
{
    public class IncidentFilterDto
    {
        public string EmployeeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string TheaterId { get; set; }
        public string CinemaId { get; set; }
    }
}
