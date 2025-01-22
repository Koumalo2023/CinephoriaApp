using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository;
using CinephoriaServer.Services;
using Microsoft.AspNetCore.Identity;

namespace CinephoriaServer.Configurations
{
    public static class ServicesExtensions
    {
        public static void AddDbServiceInjection(this IServiceCollection services)
        {
            // Pour accéder au contexte HTTP si nécessaire
            services.AddHttpContextAccessor();

            // Ajouter la gestion des utilisateurs et des connexions pour utilisateurs
            services.AddTransient<UserManager<AppUser>>();
            services.AddTransient<SignInManager<AppUser>>();

            services.AddTransient<EmailService>();


            // injection des Services
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ISeatService, SeatService>();
            services.AddTransient<IIncidentService, IncidentService>();
            services.AddTransient<ITheaterService, TheaterService>();
            services.AddTransient<ICinemaService, CinemaService>();
            services.AddTransient<IMovieService, MovieService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IShowtimeService, ShowtimeService>();
            services.AddTransient<IReservationService, ReservationService>();
            services.AddTransient<IAdminDashboardService, AdminDashboardService>();
            services.AddTransient<IRoleService, RoleService>();


            // Injection du UoW (Unit of Work) pour Entity Framework
            services.AddTransient<IUnitOfWorkPostgres, UnitOfWorkPostgres>();
            services.AddTransient<IUnitOfWorkMongoDb, UnitOfWorkMongoDb>();

            // MongoDB Repositories
            
            services.AddTransient<IAdminDashboardRepository, AdminDashboardRepository>();



        }
    }

}
