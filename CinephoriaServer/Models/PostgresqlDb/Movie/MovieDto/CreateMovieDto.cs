using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class CreateMovieDto
    {
        [Required(ErrorMessage = "Le titre du film est requis.")]
        [StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères.")]
        /// <summary>
        /// Titre du film.
        /// </summary>
        public string Title { get; set; }

        [Required(ErrorMessage = "La description du film est requise.")]
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères.")]
        /// <summary>
        /// Description du film.
        /// </summary>
        public string Description { get; set; }

        [Required(ErrorMessage = "Le genre du film est requis.")]
        /// <summary>
        /// Genre du film.
        /// </summary>
        public MovieGenre Genre { get; set; }

        [Required(ErrorMessage = "La durée du film est requise.")]
        /// <summary>
        /// Durée du film en minutes.
        /// </summary>
        public string Duration { get; set; }

        [Required(ErrorMessage = "Le réalisateur du film est requis.")]
        [StringLength(100, ErrorMessage = "Le nom du réalisateur ne peut pas dépasser 100 caractères.")]
        /// <summary>
        /// Réalisateur du film.
        /// </summary>
        public string Director { get; set; }

        [Required(ErrorMessage = "La date de sortie du film est requise.")]
        /// <summary>
        /// Date de sortie du film.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "L'âge minimum recommandé est requis.")]
        [Range(0, 18, ErrorMessage = "L'âge minimum doit être compris entre 0 et 18 ans.")]
        /// <summary>
        /// Âge minimum recommandé pour visionner ce film.
        /// </summary>
        public int MinimumAge { get; set; }

        /// <summary>
        /// Liste des images associées au film.
        /// </summary>
        public string PosterUrls { get; set; }
    }
}
