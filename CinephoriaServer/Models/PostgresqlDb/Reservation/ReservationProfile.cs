using AutoMapper;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            // Mappings pour Reservation
            CreateMap<Reservation, ReservationDto>()
                .ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.Seats)); // Mapping des sièges

            CreateMap<CreateReservationDto, Reservation>()
                .ForMember(dest => dest.Seats, opt => opt.Ignore()) // Ignorer les sièges lors du mapping
                .ForMember(dest => dest.NumberOfSeats, opt => opt.MapFrom(src => src.SeatNumbers.Count)); // Mapper le nombre de sièges


            // Mappings pour Showtime
            CreateMap<Showtime, ShowtimeDto>()
                .ForMember(dest => dest.Reservations, opt => opt.MapFrom(src => src.Reservations)); // Mapping des réservations associées

            // Mappings pour Seat
            // Mapping de Seat vers SeatDto
            CreateMap<Seat, SeatDto>()
                .ForMember(dest => dest.SeatNumber, opt => opt.MapFrom(src => src.SeatNumber))
                .ForMember(dest => dest.IsAccessible, opt => opt.MapFrom(src => src.IsAccessible))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable));

            
            // Mappings pour les DTO de réservation utilisateur
            CreateMap<Reservation, UserReservationDto>()
                .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.ReservationId))
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.ShowtimeId, opt => opt.MapFrom(src => src.ShowtimeId))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.QrCode, opt => opt.MapFrom(src => src.QrCode))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.NumberOfSeats, opt => opt.MapFrom(src => src.NumberOfSeats))
                .ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.Seats));
        }
    }
}
