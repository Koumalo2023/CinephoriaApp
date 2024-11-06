using CinephoriaServer.Models.MongooDb;
using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class AppUserDto
    {
        /// <summary>
        /// Identifiant de l'utilisateur.
        /// </summary>
        public string AppUserId { get; set; } = string.Empty;

        /// <summary>
        /// Pseudonyme de l'utilisateur.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// validation des termes d'utilisation par l'utilisateur.
        /// </summary>
        public bool HasApprovedTermsOfUse { get; set; }

        [Required]
        /// <summary>
        /// Prénom de l'utilisateur.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        [Required]
        /// <summary>
        /// Nom de famille de l'utilisateur.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Date de création du compte.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de mise à jour du compte.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Date d'embauche de l'employé.
        /// Applicable aux utilisateurs de type "Employee".
        /// </summary>
        public DateTime? HiredDate { get; set; }

        /// <summary>
        /// Poste ou fonction de l'employé.
        /// Applicable aux utilisateurs de type "Employee".
        /// </summary>
        public string? Position { get; set; }

        /// <summary>
        /// Rôle(s) de l'utilisateur : ADMIN, EMPLOYEE, USER.
        /// </summary>
        public IList<string>? Roles { get; set; } = new List<string>();

        /// <summary>
        /// Liste des incidents signalés par l'employé.
        /// Applicable uniquement pour les employés.
        /// </summary>
        public ICollection<Incident> ReportedIncidents { get; set; } = new List<Incident>();

        /// <summary>
        /// Liste des réservations effectuées par l'utilisateur.
        /// Applicable aux utilisateurs normaux.
        /// </summary>
        public List<Reservation> Reservations { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Liste des avis laissés par l'utilisateur.
        /// Applicable aux utilisateurs normaux.
        /// </summary>
        public List<MovieRating> MovieRatings { get; set; }

        /// <summary>
        /// Photo de profile des utilisateurs
        /// Applicable aux employés.
        /// </summary>
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// Liste des messages de contact envoyés par l'utilisateur.
        /// </summary>
        public List<Contact> Contact { get; set; }
    }
}
