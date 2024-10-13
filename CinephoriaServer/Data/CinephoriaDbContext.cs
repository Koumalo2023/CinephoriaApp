using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using CinephoriaServer.Repository;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaBackEnd.Data
{
    public class CinephoriaDbContext : IdentityDbContext<AppUser>
    {
        #region Common 
        public DbSet<MovieRating> MovieRatings => Set<MovieRating>();
        public DbSet<Cinema> Cinemas => Set<Cinema>();
        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<Reservation> Reservations => Set<Reservation>();

        #endregion

        public CinephoriaDbContext(DbContextOptions<CinephoriaDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CinemaConfig());
            modelBuilder.ApplyConfiguration(new ContactConfig());
            modelBuilder.ApplyConfiguration(new MovieRatingConfig());
            modelBuilder.ApplyConfiguration(new ReservationConfig());


            modelBuilder.Ignore<Movie>();
            modelBuilder.Ignore<Incident>();
            modelBuilder.Ignore<Review>();
            modelBuilder.Ignore<Theater>();
            modelBuilder.Ignore<Showtime>();
            modelBuilder.Ignore<AdminDashboard>();
            modelBuilder.Ignore<EmployeeAccount>();

        }
    }
}
