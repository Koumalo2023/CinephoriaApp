﻿using AutoMapper;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class TheaterProfile : Profile
    {
        public TheaterProfile()
        {
            // Mapping de Theater vers TheaterDto
            CreateMap<Theater, TheaterDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.Seats));

            // Mapping de CreateTheaterDto vers Theater
            CreateMap<CreateTheaterDto, Theater>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Mapping de UpdateTheaterDto vers Theater
            CreateMap<UpdateTheaterDto, Theater>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Theater, TheaterIncidentsDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Incidents, opt => opt.MapFrom(src => src.Incidents));

            CreateMap<Theater, TheaterWithIncidentsDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Cinema.Name))
                .ForMember(dest => dest.Incidents, opt => opt.MapFrom(src => src.Incidents));

            CreateMap<Theater, TheaterWithSeatsDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.Seats));
        }
    }
}
