using AutoMapper;
using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Services
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieService> _logger;
        private readonly IImageService _imageService;

        public MovieService(IUnitOfWorkPostgres unitOfWork, IMapper mapper, ILogger<MovieService> logger, IImageService imageService)
        {
             _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _imageService = imageService;
        }

        /// <summary>
        /// Récupère la liste des derniers films ajoutés.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        public async Task<List<MovieDto>> GetRecentMoviesAsync()
        {
            var movies = await _unitOfWork.Movies.GetRecentMoviesAsync();
            var movieDtos = _mapper.Map<List<MovieDto>>(movies);

            _logger.LogInformation("Récupération des derniers films réussie.");
            return movieDtos;
        }

        /// <summary>
        /// Récupère la liste de tous les films.
        /// </summary>
        /// <returns>Une liste de films.</returns>
        public async Task<List<MovieDto>> GetAllMoviesAsync()
        {
            var movies = await _unitOfWork.Movies.GetAllMoviesAsync();
            var movieDtos = _mapper.Map<List<MovieDto>>(movies);

            _logger.LogInformation("Récupération de tous les films réussie.");
            return movieDtos;
        }


        /// <summary>
        /// Récupère les détails d'un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Un objet Movie contenant les détails du film.</returns>
        public async Task<MovieDetailsDto> GetMovieDetailsAsync(int movieId)
        {
            var movie = await _unitOfWork.Movies.GetMovieDetailsAsync(movieId);
            if (movie == null)
            {
                _logger.LogWarning("Film avec l'ID {MovieId} non trouvé.", movieId);
                throw new ApiException("Film non trouvé.", StatusCodes.Status404NotFound);
            }

            var movieDetailsDto = _mapper.Map<MovieDetailsDto>(movie);

            _logger.LogInformation("Détails du film avec l'ID {MovieId} récupérés avec succès.", movieId);
            return movieDetailsDto;
        }

        /// <summary>
        /// Récupère la liste des séances disponibles pour un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>Une liste de séances.</returns>
        public async Task<List<ShowtimeDto>> GetMovieSessionsAsync(int movieId)
        {
            var showtimes = await _unitOfWork.Movies.GetMovieSessionsAsync(movieId);
            var showtimeDtos = _mapper.Map<List<ShowtimeDto>>(showtimes);

            _logger.LogInformation("Séances du film avec l'ID {MovieId} récupérées avec succès.", movieId);
            return showtimeDtos;
        }

        /// <summary>
        /// Soumet un avis sur un film de la part d'un utilisateur.
        /// </summary>
        /// <param name="reviewDto">Les données de l'avis.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        public async Task<string> SubmitMovieReviewAsync(MovieReviewDto reviewDto)
        {
            await _unitOfWork.Movies.SubmitMovieReviewAsync(reviewDto);

            _logger.LogInformation("Avis soumis avec succès pour le film avec l'ID {MovieId}.", reviewDto.MovieId);
            return "Avis soumis avec succès";
        }

        /// <summary>
        /// Filtre les films en fonction du cinéma, du genre et de la date.
        /// </summary>
        /// <param name="filterDto">Les critères de filtrage.</param>
        /// <returns>Une liste de films correspondant aux critères.</returns>
        public async Task<List<MovieDto>> FilterMoviesAsync(int? cinemaId, MovieGenre? genre, DateTime? date)
        {
            var movies = await _unitOfWork.Movies.FilterMoviesAsync(cinemaId, genre, date);
            var movieDtos = _mapper.Map<List<MovieDto>>(movies);

            _logger.LogInformation("Filtrage des films réussi.");
            return movieDtos;
        }

        /// <summary>
        /// Crée un nouveau film.
        /// </summary>
        /// <param name="createMovieDto">Les données du film à créer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        public async Task<string> CreateMovieAsync(CreateMovieDto createMovieDto)
        {
            var movie = _mapper.Map<Movie>(createMovieDto);
            await _unitOfWork.Movies.CreateMovieAsync(movie);

            _logger.LogInformation("Film créé avec succès.");
            return "Film créé avec succès.";
        }

        /// <summary>
        /// Ajoute une affiche à un film existant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <param name="posterUrl">L'URL de l'affiche à ajouter.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        public async Task<string> AddPosterToMovieAsync(int movieId, string posterUrl)
        {
            // Récupérer le film existant
            var movie = await _unitOfWork.Movies.GetByIdAsync(movieId);
            if (movie == null)
            {
                _logger.LogWarning("Film avec l'ID {MovieId} non trouvé.", movieId);
                throw new ApiException("Film non trouvé.", StatusCodes.Status404NotFound);
            }

            // Ajouter l'URL de l'affiche à la liste des affiches du film
            movie.PosterUrls = posterUrl;

            // Mettre à jour le film dans la base de données
            await _unitOfWork.Movies.UpdateAsync(movie);

            _logger.LogInformation("Affiche ajoutée avec succès au film avec l'ID {MovieId}.", movieId);
            return "Affiche ajoutée avec succès au film";
        }

        /// <summary>
        /// Supprime une affiche d'un film existant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <param name="posterUrl">L'URL de l'affiche à supprimer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        public async Task<string> RemovePosterFromMovieAsync(int movieId, string posterUrl)
        {
            // Récupérer le film existant
            var movie = await _unitOfWork.Movies.GetByIdAsync(movieId);
            if (movie == null)
            {
                _logger.LogWarning("Film avec l'ID {MovieId} non trouvé.", movieId);
                throw new ApiException("Film non trouvé.", StatusCodes.Status404NotFound);
            }

            // Supprimer l'affiche du stockage
            var imageDeleted = await _imageService.DeleteImageAsync(posterUrl);
            if (!imageDeleted)
            {
                throw new ApiException("L'affiche n'a pas pu être supprimée du stockage.", StatusCodes.Status500InternalServerError);
            }

            // Supprimer l'URL de l'affiche de la liste des affiches du film
            movie.PosterUrls = posterUrl;

            // Mettre à jour le film dans la base de données
            await _unitOfWork.Movies.UpdateAsync(movie);

            _logger.LogInformation("Affiche supprimée avec succès du film avec l'ID {MovieId}.", movieId);
            return "Affiche supprimée avec succès.";
        }

        /// <summary>
        /// Met à jour les informations d'un film existant.
        /// </summary>
        /// <param name="updateMovieDto">Les nouvelles données du film.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        public async Task<string> UpdateMovieAsync(UpdateMovieDto updateMovieDto)
        {
            var movie = _mapper.Map<Movie>(updateMovieDto);
            await _unitOfWork.Movies.UpdateMovieAsync(movie);

            _logger.LogInformation("Film avec l'ID {MovieId} mis à jour avec succès.", updateMovieDto.MovieId);
            return "Film  mis à jour avec succès.";
        }

        /// <summary>
        /// Supprime un film en fonction de son identifiant.
        /// </summary>
        /// <param name="movieId">L'identifiant du film à supprimer.</param>
        /// <returns>Une réponse indiquant si l'opération a réussi.</returns>
        public async Task<string> DeleteMovieAsync(int movieId)
        {
            await _unitOfWork.Movies.DeleteMovieAsync(movieId);

            _logger.LogInformation("Film avec l'ID {MovieId} supprimé avec succès.", movieId);
            return "Film supprimé avec succès.";
        }

        /// <summary>
        /// Récupère la liste des films qui ont des séances dans un cinéma spécifique.
        /// </summary>
        /// <param name="cinemaId">L'identifiant du cinéma.</param>
        /// <returns>Une liste de films.</returns>
        public async Task<List<MovieDto>> GetMoviesByCinemaIdAsync(int cinemaId)
        {
            var movies = await _unitOfWork.Movies.GetMoviesByCinemaIdAsync(cinemaId);
            if (movies == null || !movies.Any())
            {
                _logger.LogWarning("Aucun film trouvé pour le cinéma avec l'ID {CinemaId}.", cinemaId);
                throw new ApiException("Aucun film trouvé pour ce cinéma.", StatusCodes.Status404NotFound);
            }

            var movieDtos = _mapper.Map<List<MovieDto>>(movies);

            _logger.LogInformation("Films récupérés avec succès pour le cinéma avec l'ID {CinemaId}.", cinemaId);
            return movieDtos;
        }
    }

}
