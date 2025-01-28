namespace CinephoriaServer.Models.PostgresqlDb
{
    public class UserProfileDto
    {
        /// <summary>
        /// Identifiant unique de l'utilisateur.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Prénom de l'utilisateur.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Nom de famille de l'utilisateur.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Adresse e-mail de l'utilisateur.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Date de création du compte.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Date de mise-à-jour du compte.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Liste des réservations de l'utilisateur.
        /// </summary>
        public ICollection<ReservationDto> Reservations { get; set; } = new List<ReservationDto>();

        public string PhoneNumber { get; set; }

        /// <summary>
        /// Liste des avis laissés par l'utilisateur.
        /// </summary>
        public ICollection<MovieRatingDto> MovieRatings { get; set; } = new List<MovieRatingDto>();

        /// <summary>
        /// Rôle de l'utilisateur.
        /// </summary>
        public string Role { get; set; }
    }
}
