using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CinephoriaServer.Configurations.Extensions;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class Movie : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identifiant unique du film.
        /// </summary>
        public int MovieId { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères.")]
        /// <summary>
        /// Titre du film.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères.")]
        /// <summary>
        /// Description du film.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        [Required]
        /// <summary>
        /// Genre du film.
        /// </summary>
        public MovieGenre Genre { get; set; }

        [Required]
        /// <summary>
        /// Durée du film en minutes.
        /// </summary>
        public string Duration { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Le nom du réalisateur ne peut pas dépasser 100 caractères.")]
        /// <summary>
        /// Réalisateur du film.
        /// </summary>
        public string Director { get; set; } = string.Empty;

        [Required]
        /// <summary>
        /// Date de sortie du film.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        [Required]
        [Range(0, 18, ErrorMessage = "L'âge minimum doit être compris entre 0 et 18 ans.")]
        /// <summary>
        /// Âge minimum recommandé pour visionner ce film.
        /// </summary>
        public int MinimumAge { get; set; }

        /// <summary>
        /// Label "Coup de cœur" si le film a plu à l'équipe du cinéma.
        /// </summary>
        public bool IsFavorite { get; set; } = false;

        /// <summary>
        /// Moyenne des notes des utilisateurs pour ce film (sur 5).
        /// </summary>
        public double AverageRating { get; set; } = 0.0;

        /// <summary>
        /// Liste des séances associées à ce film (relation un-à-plusieurs).
        /// </summary>
        public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();

        /// <summary>
        /// Liste des images déposées sur ce film.
        /// </summary>
        public string PosterUrls { get; set; }

        /// <summary>
        /// Liste des notations associées à ce film.
        /// </summary>
        public ICollection<MovieRating> MovieRatings { get; set; } = new List<MovieRating>();
    }
}
