using AutoMapper;
using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository;
using Microsoft.Extensions.Logging;

namespace CinephoriaServer.Services
{
    public class SeatService : ISeatService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SeatService> _logger;

        public SeatService(IUnitOfWorkPostgres unitOfWork, IMapper mapper, ILogger<SeatService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Récupère la liste des sièges disponibles pour une séance spécifique.
        /// </summary>
        /// <param name="sessionId">L'identifiant de la séance.</param>
        /// <returns>Une liste de sièges disponibles sous forme de DTO.</returns>
        public async Task<List<SeatDto>> GetAvailableSeatsAsync(int sessionId)
        {
            var seats = await _unitOfWork.Seats.GetAvailableSeatsAsync(sessionId);
            var seatDtos = _mapper.Map<List<SeatDto>>(seats);

            _logger.LogInformation("{Count} sièges disponibles récupérés pour la séance avec l'ID {SessionId}.", seatDtos.Count, sessionId);
            return seatDtos;
        }

        /// <summary>
        /// Ajoute un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="seatNumber">Le numéro du siège à ajouter.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        public async Task<bool> AddHandicapSeatAsync(int theaterId, string seatNumber)
        {
            await _unitOfWork.Seats.AddHandicapSeatAsync(theaterId, seatNumber);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Siège réservé pour personnes à mobilité réduite ajouté avec succès dans la salle avec l'ID {TheaterId}.", theaterId);
            return true;
        }

        /// <summary>
        /// Supprime un siège réservé pour les personnes à mobilité réduite dans une salle de cinéma.
        /// </summary>
        /// <param name="theaterId">L'identifiant de la salle de cinéma.</param>
        /// <param name="seatNumber">Le numéro du siège à supprimer.</param>
        /// <returns>Une réponse indiquant le succès ou l'échec de l'opération.</returns>
        public async Task<bool> RemoveHandicapSeatAsync(int theaterId, string seatNumber)
        {
            await _unitOfWork.Seats.RemoveHandicapSeatAsync(theaterId, seatNumber);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Siège réservé pour personnes à mobilité réduite supprimé avec succès dans la salle avec l'ID {TheaterId}.", theaterId);
            return true;
        }
    }
}
