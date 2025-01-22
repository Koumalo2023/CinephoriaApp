using CinephoriaServer.Repository;

namespace CinephoriaServer.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWorkPostgres _unitOfWorkPostgres;
        private readonly IUnitOfWorkMongoDb _unitOfWorkMongooBb;

        public ReservationService(IUnitOfWorkPostgres unitOfWorkPostgres, IUnitOfWorkMongoDb unitOfWorkMongooDb)
        {
            _unitOfWorkPostgres = unitOfWorkPostgres;
            _unitOfWorkMongooBb = unitOfWorkMongooDb;
        }

        
    }

}
