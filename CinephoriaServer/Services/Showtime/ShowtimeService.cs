using CinephoriaServer.Repository;

namespace CinephoriaServer.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IUnitOfWorkMongoDb _unitOfWork;
        private readonly IUnitOfWorkPostgres _unitOfWorkPostgres;

        public ShowtimeService(IUnitOfWorkPostgres unitOfWorkPostgres, IUnitOfWorkMongoDb unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkPostgres = unitOfWorkPostgres;
        }


    }
}
