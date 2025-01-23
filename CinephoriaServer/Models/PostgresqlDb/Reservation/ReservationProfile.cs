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
                .ForMember(dest => dest.Seats, opt => opt.Ignore()); // Ignorer les sièges lors de la création, car ils seront ajoutés séparément

            CreateMap<UpdateReservationDto, Reservation>()
                .ForMember(dest => dest.Seats, opt => opt.Ignore()); // Ignorer les sièges lors de la mise à jour

            // Mappings pour Showtime
            CreateMap<Showtime, ShowtimeDto>()
                .ForMember(dest => dest.Reservations, opt => opt.MapFrom(src => src.Reservations)); // Mapping des réservations associées

            // Mappings pour Seat
            CreateMap<Seat, SeatDto>();

            // Mappings pour les DTO de calcul de prix
            CreateMap<CalculatePriceRequestDto, Showtime>()
                .ForMember(dest => dest.ShowtimeId, opt => opt.MapFrom(src => src.ShowtimeId))
                .ForMember(dest => dest.AvailableSeats, opt => opt.Ignore()); // Ignorer les sièges, car ils ne sont pas nécessaires pour le calcul

            CreateMap<Reservation, CalculatePriceResponseDto>()
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));

            // Mappings pour les DTO de blocage de sièges
            CreateMap<HoldSeatsRequestDto, Showtime>()
                .ForMember(dest => dest.ShowtimeId, opt => opt.MapFrom(src => src.ShowtimeId))
                .ForMember(dest => dest.AvailableSeats, opt => opt.Ignore()); // Ignorer les sièges, car ils seront gérés séparément

            // Mappings pour les DTO d'annulation de réservation
            CreateMap<CancelReservationDto, Reservation>()
                .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.ReservationId));

            // Mappings pour les DTO de réponse de création de réservation
            CreateMap<Reservation, CreateReservationResponseDto>()
                .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.ReservationId))
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.ShowtimeId, opt => opt.MapFrom(src => src.ShowtimeId))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.QrCode, opt => opt.MapFrom(src => src.QrCode))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.NumberOfSeats, opt => opt.MapFrom(src => src.NumberOfSeats));

            // Mappings pour les DTO de réservation utilisateur
            CreateMap<Reservation, UserReservationDto>()
                .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.ReservationId))
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.ShowtimeId, opt => opt.MapFrom(src => src.ShowtimeId))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.QrCode, opt => opt.MapFrom(src => src.QrCode))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.NumberOfSeats, opt => opt.MapFrom(src => src.NumberOfSeats));
        }
    }
}
