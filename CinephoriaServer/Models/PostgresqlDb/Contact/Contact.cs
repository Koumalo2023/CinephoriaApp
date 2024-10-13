using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class Contact
    {
        /// <summary>
        /// Identifiant unique de la demande de contact.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactId { get; set; }

        /// <summary>
        /// Nom de l'utilisateur qui a fait la demande (optionnel).
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Titre de la demande de contact.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description détaillée de la demande de contact.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Date et heure de création de la demande de contact.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur lié à cette demande (facultatif).
        /// </summary>
        [ForeignKey("AppUser")]
        public string? UserId { get; set; }
        /// <summary>
        /// Navigation vers l'utilisateur ayant fait la demande (relation facultative un-à-un).
        /// </summary>
        public AppUser? AppUser { get; set; } // Relation facultative
    }
}
