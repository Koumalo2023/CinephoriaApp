using AutoMapper;
using CinephoriaServer.Models.PostgresqlDb;

namespace CinephoriaServer.Models.MongooDb
{
    public class TheaterProfile : Profile
    {
        public TheaterProfile()
        {
            CreateMap<Theater, TheaterDto>()
                .ReverseMap();
        }
    }
}
