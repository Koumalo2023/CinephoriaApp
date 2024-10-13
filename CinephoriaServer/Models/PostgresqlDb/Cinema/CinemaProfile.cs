using AutoMapper;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class CinemaProfile : Profile
    {
        public CinemaProfile()
        {
            CreateMap<Cinema, CinemaDto>()
                .ReverseMap();
        }
    }
}
