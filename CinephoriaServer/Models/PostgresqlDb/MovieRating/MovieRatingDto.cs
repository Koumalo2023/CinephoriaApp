using CinephoriaServer.Models.MongooDb;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class MovieRatingDto
    {
        /// <summary>
        /// Identifiant unique de l'avis.
        /// </summary>
        public int MovieRatingId { get; set; }

        /// <summary>
        /// Référence au film (movies.id).
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Référence à l'utilisateur qui a laissé l'avis.
        /// </summary>
        public string AppUserId { get; set; }

        /// <summary>
        /// Note donnée par l'utilisateur (sur 5).
        /// </summary>
        public float Rating { get; set; }

        /// <summary>
        /// Description laissée par l'utilisateur.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Indique si l'avis a été approuvé par un employé.
        /// </summary>
        public bool IsValidated { get; set; }

        /// <summary>
        /// Date de l'avis.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de mise à jour de l'avis.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Utilisateur ayant laissé l'avis (navigation).
        /// </summary>
        public AppUserDto AppUser { get; set; }

        /// <summary>
        /// Film pour lequel l'avis a été laissé (navigation).
        /// </summary>
        public Movie Movies { get; set; }
    }
}
