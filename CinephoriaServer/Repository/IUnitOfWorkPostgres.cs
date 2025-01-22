using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository.EntityFramwork;

namespace CinephoriaServer.Repository
{
    public interface IUnitOfWorkPostgres : IDisposable
    {
        // Propriétés pour les repositories spécifiques
        ICinemaRepository Cinemas { get; }
        IMovieRepository Movies { get; }
        IMovieRatingRepository MovieRatings { get; }
        ITheaterRepository Theaters { get; }
        ISeatRepository Seats { get; }
        //IShowtimeRepository Showtimes { get; }
        //IReservationRepository Reservations { get; }
        IIncidentRepository Incidents { get; }
        IUserRepository Users { get; }

        // Méthodes génériques pour les repositories
        IReadRepository<TEntity> ReadRepository<TEntity>() where TEntity : class;
        IWriteRepository<TEntity> WriteRepository<TEntity>() where TEntity : class;

        // Méthodes pour sauvegarder les changements
        Task<int> CompleteAsync();
        int Complete();

        
    }
}
