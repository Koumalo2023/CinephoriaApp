using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class UpdateMovieDto
    {
        [Required(ErrorMessage = "L'identifiant du film est requis.")]
        [Range(1, int.MaxValue, ErrorMessage = "L'identifiant du film doit être un nombre positif.")]
        /// <summary>
        /// Identifiant unique du film à mettre à jour.
        /// </summary>
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Le titre du film est requis.")]
        [StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères.")]
        /// <summary>
        /// Nouveau titre du film.
        /// </summary>
        public string Title { get; set; }

        [Required(ErrorMessage = "La description du film est requise.")]
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères.")]
        /// <summary>
        /// Nouvelle description du film.
        /// </summary>
        public string Description { get; set; }

        [Required(ErrorMessage = "Le genre du film est requis.")]
        [StringLength(50, ErrorMessage = "Le genre ne peut pas dépasser 50 caractères.")]
        /// <summary>
        /// Nouveau genre du film.
        /// </summary>
        public MovieGenre Genre { get; set; }

        [Required(ErrorMessage = "La durée du film est requise.")]
        [Range(1, int.MaxValue, ErrorMessage = "La durée doit être un nombre positif.")]
        /// <summary>
        /// Nouvelle durée du film en minutes.
        /// </summary>
        public string Duration { get; set; }

        [Required(ErrorMessage = "Le réalisateur du film est requis.")]
        [StringLength(100, ErrorMessage = "Le nom du réalisateur ne peut pas dépasser 100 caractères.")]
        /// <summary>
        /// Nouveau réalisateur du film.
        /// </summary>
        public string Director { get; set; }

        [Required(ErrorMessage = "La date de sortie du film est requise.")]
        /// <summary>
        /// Nouvelle date de sortie du film.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "L'âge minimum recommandé est requis.")]
        [Range(0, 18, ErrorMessage = "L'âge minimum doit être compris entre 0 et 18 ans.")]
        /// <summary>
        /// Nouvel âge minimum recommandé pour visionner ce film.
        /// </summary>
        public int MinimumAge { get; set; }

        /// <summary>
        /// Indique si le film est un "coup de cœur" de l'équipe du cinéma.
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Liste des images associées au film.
        /// </summary>
        public string PosterUrls { get; set; }
    }
}
