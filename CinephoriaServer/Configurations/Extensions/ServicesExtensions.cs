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


            // injection des Services
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient<IIncidentService, IncidentService>();
            services.AddTransient<ICinemaService, CinemaService>();
            services.AddTransient<IMovieService, MovieService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IShowtimeService, ShowtimeService>();
            services.AddTransient<IReservationService, ReservationService>();
            services.AddTransient<IAdminDashboardService, AdminDashboardService>();



            // Injection du UoW (Unit of Work) pour Entity Framework
            services.AddTransient<IUnitOfWorkPostgres, UnitOfWorkPostgres>();
            services.AddTransient<IUnitOfWorkMongoDb, UnitOfWorkMongoDb>();

            // MongoDB Repositories
            services.AddTransient<IMovieRepository, MovieRepository>();
            services.AddTransient<IShowtimeRepository, ShowtimeRepository>();
            services.AddTransient<IIncidentRepository, IncidentRepository>();
            services.AddTransient<IAdminDashboardRepository, AdminDashboardRepository>();

            // PostgreSQL Repositories
            services.AddTransient<IContactRepository, ContactRepository>();
            services.AddTransient<ICinemaRepository, CinemaRepository>();
            services.AddTransient<IReservationRepository, ReservationRepository>();
        }
    }

}
