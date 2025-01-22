using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class MovieDetailsDto
    {
        /// <summary>
        /// Identifiant unique du film.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Titre du film.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description du film.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Genre du film.
        /// </summary>
        public MovieGenre Genre { get; set; }

        /// <summary>
        /// Durée du film en minutes.
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Réalisateur du film.
        /// </summary>
        public string Director { get; set; }

        /// <summary>
        /// Date de sortie du film.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Âge minimum recommandé pour visionner ce film.
        /// </summary>
        public int MinimumAge { get; set; }

        /// <summary>
        /// Note moyenne du film.
        /// </summary>
        public double AverageRating { get; set; }

        /// <summary>
        /// Liste des URLs des affiches du film.
        /// </summary>
        public List<string> PosterUrls { get; set; } = new List<string>();

        /// <summary>
        /// Liste des séances disponibles pour ce film.
        /// </summary>
        public List<ShowtimeDto> Showtimes { get; set; } = new List<ShowtimeDto>();

        /// <summary>
        /// Liste des avis sur ce film.
        /// </summary>
        public List<MovieRatingDto> Ratings { get; set; } = new List<MovieRatingDto>();
    }

}
