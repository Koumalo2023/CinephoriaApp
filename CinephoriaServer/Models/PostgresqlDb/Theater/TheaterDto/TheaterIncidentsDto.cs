namespace CinephoriaServer.Models.PostgresqlDb
{
    public class TheaterIncidentsDto
    {
        /// <summary>
        /// Identifiant de la salle de cinéma.
        /// </summary>
        public int TheaterId { get; set; }

        /// <summary>
        /// Nom ou numéro de la salle.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Liste des incidents signalés dans cette salle.
        /// </summary>
        public List<IncidentDto> Incidents { get; set; } = new List<IncidentDto>();
    }
}
