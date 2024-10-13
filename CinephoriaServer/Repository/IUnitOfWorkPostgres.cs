using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository.EntityFramwork;

namespace CinephoriaServer.Repository
{
    public interface IUnitOfWorkPostgres : IDisposable
    {
        IRepository<Cinema> Cinemas { get; }
        IRepository<Contact> Contacts { get; }
        IRepository<Reservation> Reservations { get; }
        IRepository<MovieRating> MovieRatings { get; }
        IRepository<AppUser> Users { get; }

        Task<int> CompleteAsync();
        int Complete();
    }
}
