using MongoDB.Bson;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.MongooDb
{
    public class EmployeeAccountDto
    {

        // <summary>
        /// Identifiant unique du compte employé.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Email utilisé comme login.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Prénom de l'employé.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Nom de famille de l'employé.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Numéro de téléphone de l'employé.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Date de création du compte.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Rôle de l'employé (EMPLOYEE, ADMIN).
        /// </summary>
        public List<string> Role { get; set; }


        /// <summary>
        /// Date d'embauche de l'employé.
        /// </summary>
        public DateTime HiredDate { get; set; }

        /// <summary>
        /// Poste ou fonction de l'employé.
        /// </summary>
        public string Position { get; set; }
    }
}
