using AutoMapper;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            // Reservation mappings
            CreateMap<Reservation, ReservationDto>();
            CreateMap<CreateReservationDto, Reservation>();
            CreateMap<UpdateReservationDto, Reservation>();
        }
    }
}
