using CinephoriaBackEnd.Data;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace CinephoriaServer.Repository
{
    public interface ICinemaRepository
    {
        #region Gestion des cinémas

        /// <summary>
        /// Crée un nouveau cinéma dans la base de données.
        /// </summary>
        /// <param name="cinema">Le cinéma à créer.</param>
        /// <returns>Le cinéma créé.</returns>
        Task<Cinema> CreateCinemaAsync(Cinema cinema);

        /// <summary>
        /// Modifie un cinéma existant dans la base de données.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma à modifier.</param>
        /// <param name="updatedCinema">Le cinéma contenant les nouvelles informations.</param>
        /// <returns>True si la modification a réussi, sinon False.</returns>
        Task<bool> UpdateCinemaAsync(int cinemaId, Cinema updatedCinema);

        /// <summary>
        /// Supprime un cinéma de la base de données.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma à supprimer.</param>
        /// <returns>True si la suppression a réussi, sinon False.</returns>
        Task<bool> DeleteCinemaAsync(int cinemaId);

        /// <summary>
        /// Récupère les informations de tous les cinémas (nom, adresse, téléphone, etc.).
        /// </summary>
        /// <returns>Une liste de cinémas avec leurs informations complètes.</returns>
        Task<List<Cinema>> GetCinemaInformationsAsync();

        #endregion

        #region Gestion des salles

        /// <summary>
        /// Crée une nouvelle salle de projection pour un cinéma spécifique.
        /// </summary>
        /// <param name="theater">La salle à créer.</param>
        /// <returns>La salle créée.</returns>
        Task<Theater> CreateTheaterAsync(Theater theater);

        /// <summary>
        /// Modifie une salle de projection existante.
        /// </summary>
        /// <param name="theaterId">Identifiant de la salle à modifier.</param>
        /// <param name="updatedTheater">La salle contenant les nouvelles informations.</param>
        /// <returns>True si la modification a réussi, sinon False.</returns>
        Task<bool> UpdateTheaterAsync(string theaterId, Theater updatedTheater);

        /// <summary>
        /// Supprime une salle de projection existante.
        /// </summary>
        /// <param name="theaterId">Identifiant de la salle à supprimer.</param>
        /// <returns>True si la suppression a réussi, sinon False.</returns>
        Task<bool> DeleteTheaterAsync(string theaterId);

        /// <summary>
        /// Récupère toutes les salles d'un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">Identifiant du cinéma.</param>
        /// <returns>Liste des salles du cinéma spécifié.</returns>
        Task<List<Theater>> GetTheatersByCinemaIdAsync(string cinemaId);

        #endregion
    }

    public class CinemaRepository : ICinemaRepository
    {
        private readonly CinephoriaDbContext _context; // Pour les cinémas
        private readonly IMongoCollection<Theater> _theaterCollection; // Pour MongoDB

        public CinemaRepository(CinephoriaDbContext context, IMongoDatabase mongoDatabase)
        {
            _context = context;
            _theaterCollection = mongoDatabase.GetCollection<Theater>("Theaters");
        }

        #region Gestion des cinémas

        /// <summary>
        /// Crée un nouveau cinéma dans la base de données.
        /// </summary>
        /// <param name="cinema">Le cinéma à créer.</param>
        /// <returns>Le cinéma créé.</returns>
        public async Task<Cinema> CreateCinemaAsync(Cinema cinema)
        {
            _context.Cinemas.Add(cinema);
            await _context.SaveChangesAsync();
            return cinema;
        }

        /// <summary>
        /// Modifie un cinéma existant dans la base de données.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma à modifier.</param>
        /// <param name="updatedCinema">Le cinéma contenant les nouvelles informations.</param>
        /// <returns>True si la modification a réussi, sinon False.</returns>
        public async Task<bool> UpdateCinemaAsync(int cinemaId, Cinema updatedCinema)
        {
            var cinema = await _context.Cinemas.FindAsync(cinemaId);
            if (cinema == null)
                return false;

            cinema.Name = updatedCinema.Name;
            cinema.Address = updatedCinema.Address;
            cinema.PhoneNumber = updatedCinema.PhoneNumber;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Supprime un cinéma de la base de données.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma à supprimer.</param>
        /// <returns>True si la suppression a réussi, sinon False.</returns>
        public async Task<bool> DeleteCinemaAsync(int cinemaId)
        {
            var cinema = await _context.Cinemas.FindAsync(cinemaId);
            if (cinema == null)
                return false;

            _context.Cinemas.Remove(cinema);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Récupère les informations de tous les cinémas (nom, adresse, téléphone, etc.).
        /// </summary>
        /// <returns>Une liste de cinémas avec leurs informations complètes.</returns>
        public async Task<List<Cinema>> GetCinemaInformationsAsync()
        {
            return await _context.Cinemas.ToListAsync();
        }

        #endregion

        #region Gestion des salles

        /// <summary>
        /// Crée une nouvelle salle de projection pour un cinéma spécifique.
        /// </summary>
        /// <param name="theater">La salle à créer.</param>
        /// <returns>La salle créée.</returns>
        public async Task<Theater> CreateTheaterAsync(Theater theater)
        {
            await _theaterCollection.InsertOneAsync(theater);
            return theater;
        }

        /// <summary>
        /// Modifie une salle de projection existante.
        /// </summary>
        /// <param name="theaterId">Identifiant de la salle à modifier.</param>
        /// <param name="updatedTheater">La salle contenant les nouvelles informations.</param>
        /// <returns>True si la modification a réussi, sinon False.</returns>
        public async Task<bool> UpdateTheaterAsync(string theaterId, Theater updatedTheater)
        {
            var result = await _theaterCollection.ReplaceOneAsync(t => t.Id == theaterId, updatedTheater);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        /// <summary>
        /// Supprime une salle de projection existante.
        /// </summary>
        /// <param name="theaterId">Identifiant de la salle à supprimer.</param>
        /// <returns>True si la suppression a réussi, sinon False.</returns>
        public async Task<bool> DeleteTheaterAsync(string theaterId)
        {
            var result = await _theaterCollection.DeleteOneAsync(t => t.Id == theaterId);
            return result.DeletedCount > 0;
        }

        /// <summary>
        /// Récupère toutes les salles d'un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">Identifiant du cinéma.</param>
        /// <returns>Liste des salles du cinéma spécifié.</returns>
        public async Task<List<Theater>> GetTheatersByCinemaIdAsync(string cinemaId)
        {
            return await _theaterCollection.Find(t => t.CinemaId == cinemaId).ToListAsync();
        }

        #endregion
    }
}
