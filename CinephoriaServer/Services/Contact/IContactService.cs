using CinephoriaServer.Models.PostgresqlDb;

namespace CinephoriaServer.Services
{
    public interface IContactService
    {
        // <summary>
        /// Récupère les informations du cinéma.
        /// </summary>
        /// <param name="cinemaId">Identifiant du cinéma.</param>
        /// <returns>Les informations du cinéma.</returns>
        Task<CinemaDto?> GetCinemaInfoAsync(int cinemaId);

        /// <summary>
        /// Crée une demande de contact à partir d'un utilisateur ou visiteur.
        /// </summary>
        /// <param name="contactDto">Données de la demande de contact.</param>
        /// <returns>La demande de contact créée.</returns>
        Task<ContactDto> CreateContactAsync(ContactViewModel contactViewModel);
    }
}
