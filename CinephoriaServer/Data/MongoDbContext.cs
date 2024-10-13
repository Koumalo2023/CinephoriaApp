using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CinephoriaServer.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        // Inject MongoDbSettings instead of MongoDbContext
        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        // Méthode générique pour obtenir une collection
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        //Collections pour des types spécifiques si nécessaire
        public IMongoCollection<AdminDashboard> AdminDashboards => _database.GetCollection<AdminDashboard>("admin_dashboard");
        public IMongoCollection<EmployeeAccount> EmployeeAccounts => _database.GetCollection<EmployeeAccount>("employee_accounts");
        public IMongoCollection<Movie> Movie => _database.GetCollection<Movie>("movie");
        public IMongoCollection<Incident> Incidents => _database.GetCollection<Incident>("incidents");
        public IMongoCollection<Review> Reviews => _database.GetCollection<Review>("Reviews");
        public IMongoCollection<Showtime> Showtimes => _database.GetCollection<Showtime>("showtimes");
        public IMongoCollection<Theater> Theaters => _database.GetCollection<Theater>("theaters");
    }
}
