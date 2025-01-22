using AutoMapper;
using CinephoriaServer.Configurations;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository;
using Microsoft.AspNetCore.Identity;

namespace CinephoriaServer.Services
{
    public class MovieRatingService : IMovieRatingService
    {
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieRatingService> _logger;

        public MovieRatingService(IUnitOfWorkPostgres unitOfWork, IMapper mapper, ILogger<MovieRatingService> logger, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        /// Soumet un avis sur un film de la part d'un utilisateur.
        /// </summary>
        /// <param name="createMovieRatingDto">Les données de l'avis à soumettre.</param>
        /// <param name="userId">L'identifiant de l'utilisateur.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        public async Task<string> SubmitMovieReviewAsync(CreateMovieRatingDto createMovieRatingDto, string userId)
        {
            if (createMovieRatingDto.MovieId <= 0 || string.IsNullOrWhiteSpace(userId))
            {
                throw new ApiException("Entrées invalides.", StatusCodes.Status400BadRequest);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new ApiException("Utilisateur introuvable.", StatusCodes.Status400BadRequest);

            var movieExists = await _unitOfWork.Movies.GetByIdAsync(createMovieRatingDto.MovieId) != null;
            if (!movieExists) throw new ApiException("Film introuvable.", StatusCodes.Status400BadRequest);

            var movieRating = _mapper.Map<MovieRating>(createMovieRatingDto);
            movieRating.AppUserId = userId;
            movieRating.IsValidated = false;

            await _unitOfWork.MovieRatings.CreateAsync(movieRating);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Avis soumis avec succès pour le film avec l'ID {MovieId}.", createMovieRatingDto.MovieId);
            return "Avis soumis avec succès.";

        }

        /// <summary>
        /// Valide un avis sur un film (réservé aux administrateurs et aux employés).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à valider.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        public async Task<string> ValidateReviewAsync(int reviewId)
        {
            if (reviewId <= 0)
            {
                throw new ApiException("L'identifiant de l'avis doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            var review = await _unitOfWork.MovieRatings.GetByIdAsync(reviewId);
            if (review == null) throw new ApiException("Avis introuvable.", StatusCodes.Status404NotFound);

            review.IsValidated = true;
            await _unitOfWork.MovieRatings.UpdateAsync(review);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Avis avec l'ID {ReviewId} validé avec succès.", reviewId);
            return "Avis validé avec succès.";
        }

        /// <summary>
        /// Supprime un avis sur un film (réservé aux administrateurs et employées).
        /// </summary>
        /// <param name="reviewId">L'identifiant de l'avis à supprimer.</param>
        /// <returns>Une réponse indiquant le succès de l'opération.</returns>
        public async Task<string> DeleteReviewAsync(int reviewId)
        {
            if (reviewId <= 0)
            {
                throw new ApiException("L'identifiant de l'avis doit être un nombre positif.", StatusCodes.Status400BadRequest);
            }

            var review = await _unitOfWork.MovieRatings.GetByIdAsync(reviewId);
            if (review == null) throw new ApiException("Avis introuvable.", StatusCodes.Status404NotFound);

            await _unitOfWork.MovieRatings.DeleteReviewAsync(reviewId);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Avis avec l'ID {ReviewId} supprimé avec succès.", reviewId);
            return "Avis supprimé avec succès.";
        }


    }
}
