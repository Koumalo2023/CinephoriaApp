using AutoMapper;

namespace CinephoriaServer.Models.PostgresqlDb
{
    public class AppUserProfile : Profile
    {
        public AppUserProfile()
        {
            CreateMap<RegisterViewModel, AppUser>();
            CreateMap<AppUser, UserInfos>();
        }
    }
}
