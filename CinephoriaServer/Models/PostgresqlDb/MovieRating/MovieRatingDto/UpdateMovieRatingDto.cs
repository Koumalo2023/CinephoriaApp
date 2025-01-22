namespace CinephoriaServer.Models.PostgresqlDb
{
    public class UpdateMovieRatingDto
    {
        /// <summary>
        /// Identifiant unique de l'avis à mettre à jour.
        /// </summary>
        public int MovieRatingId { get; set; }

        /// <summary>
        /// Nouvelle note donnée par l'utilisateur (sur 5).
        /// </summary>
        public float Rating { get; set; }

        /// <summary>
        /// Nouvelle description laissée par l'utilisateur.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Indique si l'avis a été approuvé par un employé.
        /// </summary>
        public bool IsValidated { get; set; }
    }
}
