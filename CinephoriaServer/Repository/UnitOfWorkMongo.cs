﻿using CinephoriaServer.Data;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Repository.EntityFramwork;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CinephoriaServer.Repository
{
    public class UnitOfWorkMongoDb : IUnitOfWorkMongoDb
    {
        private readonly MongoDbContext _context;

        // Repositories pour les différentes collections
        public IMongoRepository<Movie> Movies { get; private set; }
        public IMongoRepository<Review> Reviews { get; private set; }
        public IMongoRepository<Incident> Incidents { get; private set; }
        public IMongoRepository<Theater> Theaters { get; private set; }
        public IMongoRepository<Showtime> Showtimes { get; private set; }
        public IMongoRepository<AdminDashboard> AdminDashboards { get; private set; }

        // Constructeur
        public UnitOfWorkMongoDb(MongoDbContext context)
        {
            _context = context;

            // Instanciation des repositories avec les noms de collections en minuscules
            Movies = new MongoRepository<Movie>(_context, "movie");
            Reviews = new MongoRepository<Review>(_context, "review");
            Theaters = new MongoRepository<Theater>(_context, "theater");
            Showtimes = new MongoRepository<Showtime>(_context, "showtime");
            Incidents = new MongoRepository<Incident>(_context, "incident");
            AdminDashboards = new MongoRepository<AdminDashboard>(_context, "admindashboard");
        }

        // Méthode pour vérifier l'existence d'un document par son ID
        public async Task<bool> ExistsAsync<T>(string id) where T : class
        {
            var collection = _context.GetCollection<T>(typeof(T).Name.ToLower()); // Utilisation du nom en minuscule
            var objectId = ObjectId.Parse(id);
            var filter = Builders<T>.Filter.Eq("_id", objectId);
            return await collection.Find(filter).AnyAsync();
        }

        // Sauvegarder les changements
        public async Task SaveChangesAsync()
        {
            await Task.CompletedTask;
        }

        // Libérer les ressources si nécessaire
        public void Dispose()
        {
            // Libération des ressources
        }
    }

}
