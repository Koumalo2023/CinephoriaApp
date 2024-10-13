using CinephoriaBackEnd.Data;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository.EntityFramwork;

namespace CinephoriaServer.Repository
{
    public class UnitOfWorkPostgres : IUnitOfWorkPostgres
    {
        private readonly CinephoriaDbContext _context;

        // Déclaration des repositories spécifiques
        public IRepository<Cinema> Cinemas { get; private set; }
        public IRepository<Contact> Contacts { get; private set; }
        public IRepository<Reservation> Reservations { get; private set; }
        public IRepository<MovieRating> MovieRatings { get; private set; }
        public IRepository<AppUser> Users { get; private set; }

        // Constructeur qui injecte le contexte et initialise les repositories
        public UnitOfWorkPostgres(CinephoriaDbContext context)
        {
            _context = context;
            Cinemas = new EFRepository<Cinema>(context);
            Contacts = new EFRepository<Contact>(context);
            Reservations = new EFRepository<Reservation>(context);
            MovieRatings = new EFRepository<MovieRating>(context);
            Users = new EFRepository<AppUser>(context);
        }

        // Méthode pour sauvegarder les modifications dans la base de données
        public int Complete()
        {
            return _context.SaveChanges();
        }

        // Méthode asynchrone pour sauvegarder les modifications
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Dispose du contexte
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
