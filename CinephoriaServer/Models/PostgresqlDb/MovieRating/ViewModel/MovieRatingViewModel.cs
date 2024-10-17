namespace CinephoriaServer.Models.PostgresqlDb
{
    public class MovieRatingViewModel
    {
        public string MovieId { get; set; }
        public string AppUserId { get; set; }
        public float Rating { get; set; }
        public string Comment { get; set; }
    }
}
