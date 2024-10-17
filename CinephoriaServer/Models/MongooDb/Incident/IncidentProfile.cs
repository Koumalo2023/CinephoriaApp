using AutoMapper;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository;
using MongoDB.Bson;

namespace CinephoriaServer.Models.MongooDb
{
    public class IncidentProfile : Profile
    {
        public IncidentProfile()
        {
            // Mapping de IncidentDto vers Incident
            CreateMap<IncidentDto, Incident>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Id) ? new ObjectId(src.Id) : ObjectId.Empty))
                .ForMember(dest => dest.ReportedBy, opt => opt.Ignore());

            // Mapping de Incident vers IncidentDto
            CreateMap<Incident, IncidentDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ReportedBy, opt => opt.MapFrom(src => src.ReportedBy));
        }
    }

}
