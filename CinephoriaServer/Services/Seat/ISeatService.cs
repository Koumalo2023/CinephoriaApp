using CinephoriaServer.Models.PostgresqlDb;

namespace CinephoriaServer.Services
{
    public interface ISeatService
    {
        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles sous forme de DTO.</returns>
        Task<List<SeatDto>> GetAvailableSeatsAsync(int sessionId);

        /// <summary>
        /// Ajoute un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="seatNumber">Le numéro du siège à ajouter.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        Task<bool> AddHandicapSeatAsync(int theaterId, string seatNumber);

        /// <summary>
        /// Supprime un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="seatNumber">Le numéro du siège à supprimer.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        Task<bool> RemoveHandicapSeatAsync(int theaterId, string seatNumber);
    }
}
