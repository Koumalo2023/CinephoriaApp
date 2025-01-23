
namespace CinephoriaServer.Services
{
    public interface IReservationService
    {

        /// <summary>
        /// Valide un QRCode scanné pour une réservation.
        /// </summary>
        /// <param name="qrCodeData">Les données du QRCode scanné.</param>
        /// <returns>True si la validation est réussie, sinon False.</returns>
        Task<bool> ValidatedSession(string qrCodeData);
    }

}
