using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class CinemaViewModel
    {
        /// <summary>
        /// Identifiant unique du cinéma.
        /// </summary>
        public int CinemaId { get; set; }
        /// <summary>
        /// Nom du cinéma.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Adresse du cinéma.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Numéro de téléphone du cinéma.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Ville où se trouve le cinéma.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Pays où se trouve le cinéma.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Horaires d'ouverture du cinéma.
        /// </summary>
        public string OpeningHours { get; set; }
    }
}
