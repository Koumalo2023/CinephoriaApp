using AutoMapper;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            // Mapping entre l'entité Reservation et ReservationDto
            CreateMap<Reservation, ReservationDto>()
                .ReverseMap();
        }
    }
}
