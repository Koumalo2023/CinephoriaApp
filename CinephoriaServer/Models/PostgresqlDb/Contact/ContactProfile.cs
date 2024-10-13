using AutoMapper;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class ContactProfile : Profile
    {
        public ContactProfile()
        {
            CreateMap<ContactViewModel, Contact>()
                .ReverseMap();

            // Mappage du modèle Contact vers le DTO et vice versa
            CreateMap<Contact, ContactDto>()
                .ForMember(dest => dest.AppUser, opt => opt.Ignore());
        }

    }
}
