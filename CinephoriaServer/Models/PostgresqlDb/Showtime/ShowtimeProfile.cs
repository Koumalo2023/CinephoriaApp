using AutoMapper;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class ShowtimeProfile : Profile
    {
        public ShowtimeProfile()
        {
            // Showtime mappings
            CreateMap<Showtime, ShowtimeDto>();
            CreateMap<CreateShowtimeDto, Showtime>();
            CreateMap<UpdateShowtimeDto, Showtime>();
        }
    }

}
