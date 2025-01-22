using System.ComponentModel.DataAnnotations;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class FilterMoviesRequestDto
    {
        /// <summary>
        /// Identifiant du cinéma (optionnel).
        /// </summary>
        public int? CinemaId { get; set; }

        /// <summary>
        /// Genre du film (optionnel).
        /// </summary>
        public MovieGenre? Genre { get; set; }

        /// <summary>
        /// Date de projection (optionnelle).
        /// </summary>
        public DateTime? Date { get; set; }
    }
}