using CinephoriaServer.Models.MongooDb;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class MovieRating
    {
        /// <summary>
        /// Identifiant unique de l'avis.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MovieRatingId { get; set; }

        /// <summary>
        /// Référence au film (movies.id).
        /// </summary>
        [ForeignKey("Movie")]
        public int MovieId { get; set; }
        /// <summary>
        /// Référence à l'utilisateur qui a laissé l'avis.
        /// </summary>
        [ForeignKey("AppUser")]
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
        public AppUser AppUser { get; set; }

        /// <summary>
        /// Film pour lequel l'avis a été laissé (navigation).
        /// </summary>
        public Movie Movie { get; set; }


    }
}
