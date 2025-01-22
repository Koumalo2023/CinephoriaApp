using AutoMapper;
using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly IShowtimeRepository _showtimeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ShowtimeService> _logger;

        public ShowtimeService(IUnitOfWorkPostgres unitOfWork, IMapper mapper, ILogger<ShowtimeService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Crée une nouvelle séance (réservé aux administrateurs et employés).
        /// </summary>
        /// <param name="createShowtimeDto">Les données de la séance à créer.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        public async Task<string> CreateShowtimeAsync(CreateShowtimeDto createShowtimeDto)
        {
            if (createShowtimeDto == null)
            {
                throw new ApiException("Les données de la séance sont invalides.", StatusCodes.Status400BadRequest);
            }

            // Calculer le prix de base en fonction de la qualité de projection
            var basePrice = CalculateBasePrice(createShowtimeDto.Quality);

            // Appliquer les ajustements de prix
            var finalPrice = basePrice + createShowtimeDto.PriceAdjustment;

            // Appliquer une promotion si nécessaire
            if (createShowtimeDto.IsPromotion)
            {
                finalPrice *= 0.9m; // 10% de réduction
            }

            var showtime = _mapper.Map<Showtime>(createShowtimeDto);
            await _unitOfWork.Showtimes.CreateSessionAsync(showtime);

            _logger.LogInformation("Séance créée avec succès pour le film avec l'ID {MovieId}.", createShowtimeDto.MovieId);
            return "Séance créée avec succès.";
        }




        private decimal CalculateBasePrice(ProjectionQuality quality)
        {
            return quality switch
            {
                ProjectionQuality.FourDX => 20.00m,
                ProjectionQuality.ThreeD => 15.00m,
                ProjectionQuality.IMAX => 18.00m,
                ProjectionQuality.FourK => 12.00m,
                ProjectionQuality.Standard2D => 10.00m,
                ProjectionQuality.DolbyCinema => 22.00m,
                _ => throw new ArgumentException("Qualité de projection non reconnue.")
            };


        }
    }
}
