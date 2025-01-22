
using CinephoriaServer.Models.PostgresqlDb;

namespace CinephoriaServer.Services
{
    public interface IShowtimeService
    {
        /// <summary>
        /// Crée une nouvelle séance (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="createShowtimeDto">Les données de la séance à créer.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        Task<string> CreateShowtimeAsync(CreateShowtimeDto createShowtimeDto);

        /// <summary>
        /// Met à jour les informations d'une séance existante (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="updateShowtimeDto">Les données mises à jour de la séance.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        Task<string> UpdateShowtimeAsync(UpdateShowtimeDto updateShowtimeDto);

    }
}
