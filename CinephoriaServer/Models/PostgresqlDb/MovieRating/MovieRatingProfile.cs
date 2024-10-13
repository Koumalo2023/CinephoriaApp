using AutoMapper;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class MovieRatingProfile : Profile
    {
        public MovieRatingProfile()
        {
            CreateMap<MovieRating, MovieRatingDto>()
                .ReverseMap();
        }
    }
}
