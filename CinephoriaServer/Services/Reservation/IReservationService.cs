using CinephoriaServer.Configurations;
using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;

namespace CinephoriaServer.Services
{
    public interface IReservationService
    {
        Task<List<Cinema>> GetAllCinemasAsync();
        Task<List<Movie>> GetMoviesByCinemaAsync(int cinemaId);
        Task<GeneralServiceResponseData<byte[]>> GetReservationQrCodeAsync(int reservationId);
        Task<GeneralServiceResponseData<object>> ValidateReservationAsync(string qrCode);
        Task<List<ReservationDto>> GetReservationsByUserAsync(string userId);
        Task<GeneralServiceResponseData<ReservationDto>> CreateReservationAsync(string userId, ReservationViewModel model);
        Task<ReservationDto> GetReservationByIdAsync(int reservationId);
        Task<List<ReservationDto>> GetReservationsByShowtimeAsync(string showtimeId);
        Task<GeneralServiceResponseData<object>> DeleteReservationAsync(int reservationId);
    }

}
