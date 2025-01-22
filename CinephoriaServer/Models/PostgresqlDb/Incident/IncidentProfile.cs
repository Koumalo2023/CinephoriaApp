using AutoMapper;
using static CinephoriaServer.Configurations.EnumConfig;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class IncidentProfile : Profile
    {
        public IncidentProfile()
        {
            // --- Mapping de Incident vers IncidentDto ---
            CreateMap<Incident, IncidentDto>()
                .ForMember(dest => dest.TheaterName, opt => opt.MapFrom(src => src.Theater.Name))
                .ForMember(dest => dest.ReportedBy, opt => opt.MapFrom(src => $"{src.ReportedBy.FirstName} {src.ReportedBy.LastName}"))
                .ForMember(dest => dest.ResolvedBy, opt => opt.MapFrom(src => src.ResolvedBy != null ? $"{src.ResolvedBy.FirstName} {src.ResolvedBy.LastName}" : null))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls));

            // --- Mapping de CreateIncidentDto vers Incident ---
            // --- Mapping de CreateIncidentDto vers Incident ---
            CreateMap<CreateIncidentDto, Incident>()
                .ForMember(dest => dest.TheaterId, opt => opt.MapFrom(src => src.TheaterId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls))
                .ForMember(dest => dest.ReportedById, opt => opt.MapFrom(src => src.ReportedBy)) // Mapper ReportedBy à ReportedById
                .ForMember(dest => dest.ResolvedById, opt => opt.Ignore()) // Ignorer ResolvedById car il sera défini plus tard
                .ForMember(dest => dest.ReportedBy, opt => opt.Ignore()) // Ignorer la navigation ReportedBy
                .ForMember(dest => dest.Theater, opt => opt.Ignore()); // Ignorer la navigation Theater

            // --- Mapping de Incident vers IncidentDetailsDto ---
            CreateMap<Incident, IncidentDetailsDto>()
                .ForMember(dest => dest.TheaterName, opt => opt.MapFrom(src => src.Theater.Name))
                .ForMember(dest => dest.ReportedBy, opt => opt.MapFrom(src => $"{src.ReportedBy.FirstName} {src.ReportedBy.LastName}"))
                .ForMember(dest => dest.ResolvedBy, opt => opt.MapFrom(src => src.ResolvedBy != null ? $"{src.ResolvedBy.FirstName} {src.ResolvedBy.LastName}" : null))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls));

            // --- Mapping de UpdateIncidentDto vers Incident ---
            CreateMap<UpdateIncidentDto, Incident>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls))
                .ForMember(dest => dest.ResolvedAt, opt => opt.MapFrom(src => src.Status == IncidentStatus.Resolved ? DateTime.UtcNow : (DateTime?)null));
        }
    }

}
