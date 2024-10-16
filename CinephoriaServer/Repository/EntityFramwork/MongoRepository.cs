using CinephoriaServer.Data;
using CinephoriaServer.Models.MongooDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CinephoriaServer.Repository.EntityFramwork
{
    public class MongoRepository<T> : IMongoRepository<T>
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(MongoDbContext context, string collectionName)
        {
            _collection = context.GetCollection<T>(collectionName);
        }

        // Méthode pour obtenir tous les éléments
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
        }

        // Méthode pour obtenir un élément par son Id
        public async Task<T> GetByIdAsync(string id)
        {
            // Conversion de l'ID en ObjectId
            var objectId = ObjectId.Parse(id);

            // Recherche dans la collection par ObjectId
            return await _collection.Find(Builders<T>.Filter.Eq("_id", objectId)).FirstOrDefaultAsync();
        }


        // Méthode pour ajouter un élément
        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        // Méthode pour mettre à jour un élément
        public async Task UpdateAsync(T entity)
        {
            var objectId = typeof(T).GetProperty("Id")?.GetValue(entity)?.ToString();
            if (objectId != null)
            {
                await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", objectId), entity);
            }
            else
            {
                throw new Exception("L'objet ne possède pas d'Id valide.");
            }
        }


        // Méthode pour supprimer un élément par son Id
        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
            return result.DeletedCount > 0;
        }

        // Méthode pour filtrer les éléments
        public async Task<List<T>> FilterAsync(FilterDefinition<T> filter)
        {
            return await _collection.Find(filter).ToListAsync();
        }

    }

}
