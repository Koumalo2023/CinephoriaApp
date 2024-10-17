namespace CinephoriaServer.Models.MongooDb
{
    public class ReviewViewModel
    {
        public string? Id { get; set; }
        public string MovieId { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
