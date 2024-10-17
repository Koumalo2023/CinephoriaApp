using CinephoriaServer.Data;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Repository.EntityFramwork;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CinephoriaServer.Repository
{
    public class UnitOfWorkMongoDb : IUnitOfWorkMongoDb
    {
        private readonly MongoDbContext _context;

        public IMongoRepository<Movie> Movies { get; private set; }
        public IMongoRepository<Review> Reviews { get; private set; }
        public IMongoRepository<Incident> Incidents { get; private set; }
        public IMongoRepository<Theater> Theaters { get; private set; }
        public IMongoRepository<Showtime> Showtimes { get; private set; }
        public IMongoRepository<EmployeeAccount> EmployeeAccounts { get; private set; }
        public IMongoRepository<AdminDashboard> AdminDashboards { get; private set; }

        public UnitOfWorkMongoDb(MongoDbContext context)
        {
            _context = context;

            // Utilisation de la méthode GetCollection dans le MongoRepository
            Movies = new MongoRepository<Movie>(_context, "Movie");
            Reviews = new MongoRepository<Review>(_context, "Review");
            Theaters = new MongoRepository<Theater>(_context, "theater");
            Showtimes = new MongoRepository<Showtime>(_context, "Showtime");
            Incidents = new MongoRepository<Incident>(_context, "incident");
            EmployeeAccounts = new MongoRepository<EmployeeAccount>(_context, "EmployeeAccount");
            AdminDashboards = new MongoRepository<AdminDashboard>(_context, "AdminDashboard");
        }

        public async Task<bool> ExistsAsync<T>(string id) where T : class
        {
            var collection = _context.GetCollection<T>(typeof(T).Name);
            var objectId = ObjectId.Parse(id);
            var filter = Builders<T>.Filter.Eq("_id", objectId);
            return await collection.Find(filter).AnyAsync();
        }

        public async Task SaveChangesAsync()
        {
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            // Libérer les ressources si nécessaire
        }
    }
}
