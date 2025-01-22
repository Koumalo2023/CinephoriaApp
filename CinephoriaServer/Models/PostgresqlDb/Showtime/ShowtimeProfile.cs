using AutoMapper;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class ShowtimeProfile : Profile
    {
        public ShowtimeProfile()
        {
            // Mapping de CreateShowtimeDto vers Showtime
            CreateMap<CreateShowtimeDto, Showtime>()
                .ForMember(dest => dest.Price, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Mapping de UpdateShowtimeDto vers Showtime
            CreateMap<UpdateShowtimeDto, Showtime>()
                .ForMember(dest => dest.Price, opt => opt.Ignore()) // Le prix sera recalculé dans le service
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Mapping de Showtime vers ShowtimeDto
            CreateMap<Showtime, ShowtimeDto>();
        }
    }

}
