using CinephoriaServer.Models.PostgresqlDb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CinephoriaServer.Models.MongooDb
{
    public class Movie
    {
        /// <summary>
        /// Identifiant unique du film.
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

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
        /// Description du film.
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Réalisateur du film.
        /// </summary>
        public string Director { get; set; }

        // <summary>
        /// Réalisateur du film.
        /// </summary>
        public int CinemaId { get; set; }

        /// <summary>
        /// Date de sortie du film.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Âge minimum recommandé pour visionner ce film.
        /// </summary>
        public int MinimumAge { get; set; }

        /// <summary>
        /// Label "Coup de cœur" si le film a plu à l'équipe du cinéma.
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Moyenne des notes des utilisateurs pour ce film (sur 5).
        /// </summary>
        public double AverageRating { get; set; }

        /// <summary>
        /// Liste des séances associées à ce film (relation un-à-plusieurs).
        /// </summary>
        public ICollection<Showtime> Showtimes { get; set; }

        /// <summary>
        /// Liste des avis déposés sur ce film (relation un-à-plusieurs).
        /// </summary>
        public ICollection<Review> Reviews { get; set; }

        /// <summary>
        /// Liste des images déposés sur ce film.
        /// </summary>
        public ICollection<string>? PosterUrls { get; set; } = new List<string>();

        /// <summary>
        /// Liste des notations associées à ce film.
        /// </summary>
        public virtual ICollection<MovieRating> MovieRatings { get; set; } = new List<MovieRating>();
    }
}
