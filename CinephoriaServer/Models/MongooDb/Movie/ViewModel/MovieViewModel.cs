using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.MongooDb
{
    public class MovieViewModel
    {
        public string? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }    
        public string Duration { get; set; }
        public string Director { get; set; }
        public int CinemaId { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int MinimumAge { get; set; }
        public bool IsFavorite { get; set; }
        public ICollection<string>? PosterUrls { get; set; } = new List<string>();
    }
}
