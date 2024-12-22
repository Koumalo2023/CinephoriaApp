
namespace CinephoriaServer.Models.MongooDb
{
    public class MovieDto
    {
        /// <summary>
        /// Identifiant unique du film.
        /// </summary>
        public string Id { get; set; }

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
        public string Genre { get; set; }

        /// <summary>
        /// Identifiant du cinema dans lequel est projecté le film.
        /// </summary>
        public int CinemaId { get; set; }

        /// <summary>
        /// Durée du film.
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
        /// Indique si le film est un coup de cœur de l'équipe.
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Moyenne des notes des utilisateurs pour ce film (sur 5).
        /// </summary>
        public double AverageRating { get; set; }


        /// <summary>
        /// Liste des séances associées à ce film (relation un-à-plusieurs).
        /// </summary>
        public ICollection<ShowtimeDto> Showtimes { get; set; }


        /// <summary>
        /// Liste des images déposés sur ce film.
        /// </summary>
        public ICollection<string>? PosterUrls { get; set; }

        /// <summary>
        /// Liste des avis déposés sur ce film (relation un-à-plusieurs).
        /// </summary>
        public ICollection<ReviewDto> Reviews { get; set; } // Assurez-vous que ReviewDTO est défini
    }
}
