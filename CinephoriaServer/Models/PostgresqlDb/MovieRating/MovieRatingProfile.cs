using AutoMapper;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class MovieRatingProfile : Profile
    {
        public MovieRatingProfile()
        {
            // Showtime mappings
            CreateMap<Showtime, ShowtimeDto>();
            CreateMap<CreateShowtimeDto, Showtime>();
            CreateMap<UpdateShowtimeDto, Showtime>();
        }
    }
}
