namespace CinephoriaServer.Models.PostgresqlDb
{
    public class UserInfos
    {
        public string AppUserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        /// <summary>
        /// validation des termes d'utilisation par l'utilisateur.
        /// </summary>
        public bool HasApprovedTermsOfUse { get; set; }

        /// <summary>
        /// Confirmation de l'Email par l'utilisateur.
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Date d'embauche de l'employé.
        /// Applicable aux utilisateurs de type "Employee".
        /// </summary>
        public DateTime? HiredDate { get; set; }

        /// <summary>
        /// Poste ou fonction de l'employé.
        /// Applicable aux utilisateurs de type "Employee".
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Photo de profile des utilisateurs
        /// Applicable aux employés.
        /// </summary>
        public string ProfileImageUrl { get; set; }
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();        

    }
}
