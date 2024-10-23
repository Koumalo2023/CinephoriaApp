using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.MongooDb
{
    public class ShowtimeViewModel
    {
        public string? Id { get; set; }
        public string MovieId { get; set; }
        public string TheaterId { get; set; }
        public int CinemaId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ProjectionQuality ProjectionQuality { get; set; }
        public int AvailableSeats { get; set; }
        public decimal Price { get; set; }
    }
}
