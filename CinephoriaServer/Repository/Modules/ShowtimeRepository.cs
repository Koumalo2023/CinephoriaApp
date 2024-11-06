using CinephoriaServer.Models.MongooDb;
using MongoDB.Driver;

namespace CinephoriaServer.Repository
{
    public interface IShowtimeRepository
    {
        /// <summary>
        /// Récupère toutes les séances disponibles pour un film dans un cinéma spécifique.
        /// </summary>
        /// <param name="movieId">ID du film</param>
        /// <param name="cinemaId">ID du cinéma</param>
        /// <returns>Liste des séances correspondantes</returns>
        Task<List<Showtime>> GetShowtimesByMovieAndCinemaAsync(string movieId, string cinemaId);

        /// <summary>
        /// Crée une nouvelle séance avec validation des horaires (sans chevauchement).
        /// </summary>
        /// <param name="showtime">Objet Showtime à créer</param>
        /// <returns>La séance créée</returns>
        Task<Showtime> CreateShowtimeAsync(Showtime showtime);

        /// <summary>
        /// Récupère toutes les séances auxquelles un utilisateur authentifié a pris part.
        /// </summary>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Liste des séances pour lesquelles l'utilisateur a une réservation.</returns>
        Task<List<Showtime>> GetShowtimesByUserIdAsync(string userId);

        /// <summary>
        /// Modifie une séance existante avec validation des horaires (sans chevauchement).
        /// </summary>
        /// <param name="showtimeId">ID de la séance à modifier</param>
        /// <param name="updatedShowtime">Objet Showtime mis à jour</param>
        /// <returns>La séance mise à jour</returns>
        Task<Showtime> UpdateShowtimeAsync(string showtimeId, Showtime updatedShowtime);

        /// <summary>
        /// Supprime une séance existante.
        /// </summary>
        /// <param name="showtimeId">ID de la séance à supprimer</param>
        Task DeleteShowtimeAsync(string showtimeId);

        /// <summary>
        /// Récupère les séances auxquelles l'utilisateur a un billet pour le jour courant et les jours suivants.
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <returns>Liste des séances</returns>
        Task<List<Showtime>> GetShowtimesForTodayAsync(string userId);

        /// <summary>
        /// Vérifie si une séance chevauche une autre dans la même salle.
        /// </summary>
        /// <param name="showtime">Objet Showtime à vérifier</param>
        /// <param name="existingShowtimeId">ID de la séance existante (pour la modification)</param>
        /// <returns>True si chevauchement détecté, sinon False</returns>
        Task<bool> IsShowtimeOverlappingAsync(Showtime showtime, string? existingShowtimeId = null);
    }


    public class ShowtimeRepository : IShowtimeRepository
    {
        private readonly IMongoCollection<Showtime> _showtimesCollection;

        public ShowtimeRepository(IMongoDatabase database)
        {
            _showtimesCollection = database.GetCollection<Showtime>("Showtimes");
        }

        public async Task<List<Showtime>> GetShowtimesByMovieAndCinemaAsync(string movieId, string cinemaId)
        {
            return await _showtimesCollection
                .Find(s => s.MovieId == movieId && s.CinemaId.ToString() == cinemaId)
                .ToListAsync();
        }

        public async Task<Showtime> CreateShowtimeAsync(Showtime showtime)
        {
            var overlapping = await IsShowtimeOverlappingAsync(showtime);
            if (overlapping)
            {
                throw new InvalidOperationException("La séance chevauche une autre dans la même salle.");
            }

            await _showtimesCollection.InsertOneAsync(showtime);
            return showtime;
        }

        public async Task<List<Showtime>> GetShowtimesByUserIdAsync(string userId)
        {
            var filter = Builders<Showtime>.Filter.ElemMatch(s => s.Reservations, r => r.AppUserId == userId);
            return await _showtimesCollection.Find(filter).ToListAsync();
        }

        public async Task<Showtime> UpdateShowtimeAsync(string showtimeId, Showtime updatedShowtime)
        {
            var overlapping = await IsShowtimeOverlappingAsync(updatedShowtime, showtimeId);
            if (overlapping)
            {
                throw new InvalidOperationException("La séance chevauche une autre dans la même salle.");
            }

            var result = await _showtimesCollection.FindOneAndReplaceAsync<Showtime>(
                s => s.Id == showtimeId,
                updatedShowtime,
                new FindOneAndReplaceOptions<Showtime> { ReturnDocument = ReturnDocument.After }
            );

            return result;
        }

        public async Task DeleteShowtimeAsync(string showtimeId)
        {
            await _showtimesCollection.DeleteOneAsync(s => s.Id == showtimeId);
        }

        public async Task<List<Showtime>> GetShowtimesForTodayAsync(string userId)
        {
            var today = DateTime.UtcNow.Date;
            var filter = Builders<Showtime>.Filter.And(
                Builders<Showtime>.Filter.Gte(s => s.StartTime, today),
                Builders<Showtime>.Filter.ElemMatch(s => s.Reservations, r => r.AppUserId == userId)
            );

            return await _showtimesCollection.Find(filter).ToListAsync();
        }

        public async Task<bool> IsShowtimeOverlappingAsync(Showtime showtime, string? existingShowtimeId = null)
        {
            var filter = Builders<Showtime>.Filter.And(
                Builders<Showtime>.Filter.Eq(s => s.TheaterId, showtime.TheaterId),
                Builders<Showtime>.Filter.Gte(s => s.EndTime, showtime.StartTime),
                Builders<Showtime>.Filter.Lte(s => s.StartTime, showtime.EndTime)
            );

            if (!string.IsNullOrEmpty(existingShowtimeId))
            {
                filter = Builders<Showtime>.Filter.And(filter, Builders<Showtime>.Filter.Ne(s => s.Id, existingShowtimeId));
            }

            var overlappingShowtime = await _showtimesCollection.Find(filter).FirstOrDefaultAsync();
            return overlappingShowtime != null;
        }
    }
}
