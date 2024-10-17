using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class AppUserDto
    {
        /// <summary>
        /// Identifiant unique de l'utilisateur.
        /// </summary>
        public string AppUserId { get; set; }

        /// <summary>
        /// Rôle de l'utilisateur dans l'application.
        /// </summary>
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Date de création du compte utilisateur.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de la dernière mise à jour des informations utilisateur.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Liste des réservations effectuées par l'utilisateur.
        /// </summary>
        public List<Reservation> Reservations { get; set; }

        /// <summary>
        /// Liste des avis laissés par l'utilisateur.
        /// </summary>
        public List<MovieRating> MovieRatings { get; set; }

        /// <summary>
        /// Liste des message de contact effectuées par l'utilisateur.
        /// </summary>
        public List<Contact> Contact { get; set; }
    }
}
