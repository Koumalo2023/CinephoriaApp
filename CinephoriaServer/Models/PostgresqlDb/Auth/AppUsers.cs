using CinephoriaServer.Models.MongooDb;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class AppUser : IdentityUser
    {
        [Required]

        /// <summary>
        /// Prénom de l'utilisateur.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Nom de famille de l'utilisateur.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Rôle de l'utilisateur : ADMIN, EMPLOYEE, USER.
        /// </summary>
        public UserRole Role { get; set; } = UserRole.USER;

        /// <summary>
        /// Date de création du compte.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de mise à jour du compte.
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
