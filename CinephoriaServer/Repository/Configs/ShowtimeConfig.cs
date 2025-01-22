using CinephoriaServer.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.Repository
{
    public class ShowtimeConfig : IEntityTypeConfiguration<Showtime>
    {
        public void Configure(EntityTypeBuilder<Showtime> builder)
        {
            // Clé primaire
            builder.HasKey(s => s.ShowtimeId);

            // Propriétés
            builder.Property(s => s.StartTime)
                   .IsRequired();

            builder.Property(s => s.EndTime)
                   .IsRequired();

            builder.Property(s => s.Quality)
                   .IsRequired();

            builder.Property(s => s.Price)
                   .IsRequired()
                   .HasColumnType("decimal(18, 2)");

            // Relation avec Movie
            builder.HasOne(s => s.Movie)
                   .WithMany(m => m.Showtimes)
                   .HasForeignKey(s => s.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation avec Theater
            builder.HasOne(s => s.Theater)
                   .WithMany(t => t.Showtimes)
                   .HasForeignKey(s => s.TheaterId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation avec Reservation
            builder.HasMany(s => s.Reservations)
                   .WithOne(r => r.Showtime)
                   .HasForeignKey(r => r.ShowtimeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
