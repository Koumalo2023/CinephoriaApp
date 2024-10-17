using CinephoriaServer.Models.MongooDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CinephoriaServer.Repository
{
    public interface IUserRepository
    {
        /// <summary>
        /// Crée un compte employé.
        /// Cette opération est réservée à l'administrateur.
        /// </summary>
        /// <param name="employeeAccount">Les informations du compte de l'employé à créer.</param>
        /// <returns>Le compte employé créé.</returns>
        Task<EmployeeAccount> CreateEmployeeAsync(EmployeeAccount employeeAccount);

        /// <summary>
        /// Réinitialise le mot de passe d'un employé.
        /// Cette opération est réservée à l'administrateur.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé dont le mot de passe doit être réinitialisé.</param>
        /// <param name="newPasswordHash">Le nouveau mot de passe hashé.</param>
        /// <returns>True si la réinitialisation du mot de passe a réussi, sinon false.</returns>
        Task<bool> ResetEmployeePasswordAsync(ObjectId employeeId, string newPasswordHash);

        /// <summary>
        /// Récupère tous les comptes employés dans la base de données.
        /// </summary>
        /// <returns>Une liste de tous les comptes employés.</returns>
        Task<List<EmployeeAccount>> GetAllEmployeesAsync();

        /// <summary>
        /// Récupère les informations d'un employé spécifique par son identifiant.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé à récupérer.</param>
        /// <returns>L'employé correspondant, ou null si aucun employé avec cet identifiant n'est trouvé.</returns>
        Task<EmployeeAccount?> GetEmployeeByIdAsync(ObjectId employeeId);

        /// <summary>
        /// Supprime un compte employé de la base de données.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé à supprimer.</param>
        /// <returns>True si la suppression a réussi, sinon false.</returns>
        Task<bool> DeleteEmployeeAsync(ObjectId employeeId);

        /// <summary>
        /// Filtre les comptes employés en fonction de critères donnés : prénom, nom, ou email.
        /// </summary>
        /// <param name="firstName">Le prénom de l'employé (peut être null ou vide pour ignorer ce critère).</param>
        /// <param name="lastName">Le nom de l'employé (peut être null ou vide pour ignorer ce critère).</param>
        /// <param name="email">L'adresse email de l'employé (peut être null ou vide pour ignorer ce critère).</param>
        /// <returns>Une liste de comptes employés qui correspondent aux critères donnés.</returns>
        Task<List<EmployeeAccount>> FilterEmployeesAsync(string? firstName = null, string? lastName = null, string? email = null);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<EmployeeAccount> _employeeAccounts;

        public UserRepository(IMongoDatabase database)
        {
            _employeeAccounts = database.GetCollection<EmployeeAccount>("EmployeeAccounts");
        }

        /// <summary>
        /// Crée un compte employé (uniquement pour un administrateur).
        /// </summary>
        /// <param name="employeeAccount">Les informations de l'employé.</param>
        /// <returns>Le compte créé.</returns>
        public async Task<EmployeeAccount> CreateEmployeeAsync(EmployeeAccount employeeAccount)
        {
            employeeAccount.CreatedAt = DateTime.UtcNow;
            await _employeeAccounts.InsertOneAsync(employeeAccount);
            return employeeAccount;
        }

        /// <summary>
        /// Réinitialise le mot de passe d'un employé (uniquement pour un administrateur).
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé.</param>
        /// <param name="newPasswordHash">Le nouveau mot de passe hashé.</param>
        /// <returns>Vrai si la mise à jour a réussi, faux sinon.</returns>
        public async Task<bool> ResetEmployeePasswordAsync(ObjectId employeeId, string newPasswordHash)
        {
            var update = Builders<EmployeeAccount>.Update.Set(e => e.PasswordHash, newPasswordHash);
            var result = await _employeeAccounts.UpdateOneAsync(
                e => e.Id == employeeId,
                update
            );
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Récupère tous les employés.
        /// </summary>
        /// <returns>Une liste de tous les comptes employés.</returns>
        public async Task<List<EmployeeAccount>> GetAllEmployeesAsync()
        {
            return await _employeeAccounts.Find(_ => true).ToListAsync();
        }

        /// <summary>
        /// Récupère un employé par son identifiant.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé.</param>
        /// <returns>L'employé correspondant, ou null s'il n'existe pas.</returns>
        public async Task<EmployeeAccount?> GetEmployeeByIdAsync(ObjectId employeeId)
        {
            return await _employeeAccounts.Find(e => e.Id == employeeId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Supprime un compte employé par son identifiant.
        /// </summary>
        /// <param name="employeeId">L'identifiant de l'employé à supprimer.</param>
        /// <returns>Vrai si la suppression a réussi, faux sinon.</returns>
        public async Task<bool> DeleteEmployeeAsync(ObjectId employeeId)
        {
            var result = await _employeeAccounts.DeleteOneAsync(e => e.Id == employeeId);
            return result.DeletedCount > 0;
        }

        /// <summary>
        /// Filtre les employés par nom, prénom ou email.
        /// </summary>
        /// <param name="firstName">Le prénom de l'employé.</param>
        /// <param name="lastName">Le nom de l'employé.</param>
        /// <param name="email">L'email de l'employé.</param>
        /// <returns>Une liste d'employés correspondant aux critères de filtrage.</returns>
        public async Task<List<EmployeeAccount>> FilterEmployeesAsync(string? firstName = null, string? lastName = null, string? email = null)
        {
            var filters = new List<FilterDefinition<EmployeeAccount>>();

            if (!string.IsNullOrEmpty(firstName))
            {
                filters.Add(Builders<EmployeeAccount>.Filter.Regex(e => e.FirstName, new BsonRegularExpression(firstName, "i")));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                filters.Add(Builders<EmployeeAccount>.Filter.Regex(e => e.LastName, new BsonRegularExpression(lastName, "i")));
            }

            if (!string.IsNullOrEmpty(email))
            {
                filters.Add(Builders<EmployeeAccount>.Filter.Regex(e => e.UserName, new BsonRegularExpression(email, "i")));
            }

            var filter = filters.Count > 0 ? Builders<EmployeeAccount>.Filter.And(filters) : Builders<EmployeeAccount>.Filter.Empty;

            return await _employeeAccounts.Find(filter).ToListAsync();
        }
    }
}
