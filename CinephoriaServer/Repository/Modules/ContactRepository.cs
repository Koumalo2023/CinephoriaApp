using CinephoriaBackEnd.Data;
using Microsoft.EntityFrameworkCore;
using CinephoriaServer.Models.PostgresqlDb;

namespace CinephoriaServer.Repository
{
    public interface IContactRepository
    {
        /// <summary>
        /// Permet à un utilisateur ou un visiteur de contacter le cinéma via un formulaire de contact.
        /// </summary>
        /// <param name="contactMessage">Le message de contact contenant les informations de l'utilisateur et son message.</param>
        /// <returns>True si le message a été enregistré avec succès, sinon False.</returns>
        Task<Cinema?> GetCinemaInfoAsync(int cinemaId);

        /// <summary>
        /// Récupère les informations du cinéma, y compris l'adresse, le numéro de téléphone et les horaires d'ouverture.
        /// </summary>
        /// <param name="cinemaId">Identifiant du cinéma.</param>
        /// <returns>Les informations du cinéma.</returns>
        Task<Contact> CreateContactAsync(Contact contact);
    }

    public class ContactRepository : IContactRepository
    {
        private readonly CinephoriaDbContext _context;

        public ContactRepository(CinephoriaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Récupère les informations du cinéma (adresse, numéro de téléphone, horaires d'ouverture).
        /// </summary>
        /// <param name="cinemaId">Identifiant du cinéma.</param>
        /// <returns>Les informations du cinéma.</returns>
        public async Task<Cinema?> GetCinemaInfoAsync(int cinemaId)
        {
            return await _context.Cinemas
                .Where(c => c.CinemaId == cinemaId)
                .Select(c => new Cinema
                {
                    CinemaId = c.CinemaId,
                    Name = c.Name,
                    Address = c.Address,
                    PhoneNumber = c.PhoneNumber,
                    OpeningHours = c.OpeningHours
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Crée une nouvelle demande de contact dans la base de données.
        /// </summary>
        /// <param name="contact">Les détails de la demande de contact.</param>
        /// <returns>La demande de contact créée.</returns>
        public async Task<Contact> CreateContactAsync(Contact contact)
        {
            try
            {
                contact.CreatedAt = DateTime.UtcNow;
                await _context.Contacts.AddAsync(contact);
                await _context.SaveChangesAsync();
                return contact;
            }
            catch (Exception ex)
            {
                // Gestion des erreurs (log ou re-throw exception)
                throw new Exception("Erreur lors de la création de la demande de contact", ex);
            }
        }

        /// <summary>
        /// Sauvegarde des modifications de manière asynchrone dans la base de données.
        /// </summary>
        /// <returns>Le nombre de lignes affectées.</returns>
        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log ou gestion des erreurs
                throw new Exception("Erreur lors de la sauvegarde des changements", ex);
            }
        }

    }
}
