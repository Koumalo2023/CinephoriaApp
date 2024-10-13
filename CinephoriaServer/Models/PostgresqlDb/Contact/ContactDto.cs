namespace CinephoriaServer.Models.PostgresqlDb
{
    public class ContactDto
    {
        /// <summary>
        /// Identifiant unique de la demande de contact.
        /// </summary>
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
        public string? UserId { get; set; }

        /// <summary>
        /// Utilisateur ayant fait la demande (navigation).
        /// </summary>
        public AppUserDto? AppUser { get; set; }
    }
}
