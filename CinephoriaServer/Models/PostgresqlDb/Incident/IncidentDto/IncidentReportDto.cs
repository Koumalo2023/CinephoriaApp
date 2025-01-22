using System.ComponentModel.DataAnnotations;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class IncidentReportDto
    {
        /// <summary>
        /// Identifiant de la salle où l'incident a été signalé.
        /// </summary>
        [Required(ErrorMessage = "L'identifiant de la salle est requis.")]
        [Range(1, int.MaxValue, ErrorMessage = "L'identifiant de la salle doit être un nombre positif.")]
        public int TheaterId { get; set; }

        /// <summary>
        /// Description de l'incident.
        /// </summary>
        [Required(ErrorMessage = "La description de l'incident est requise.")]
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères.")]
        public string Description { get; set; }

        /// <summary>
        /// Identifiant de l'employé qui a signalé l'incident.
        /// </summary>
        [Required(ErrorMessage = "L'identifiant de l'employé est requis.")]
        public string ReportedBy { get; set; }

        /// <summary>
        /// Liste des images de l'incident.
        /// </summary>
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
