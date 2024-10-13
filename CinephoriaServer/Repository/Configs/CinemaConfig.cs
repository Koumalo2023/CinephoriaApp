using CinephoriaServer.Models.MongooDb;
using CinephoriaServer.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.Repository
{
    public class CinemaConfig : IEntityTypeConfiguration<Cinema>
    {
        public void Configure(EntityTypeBuilder<Cinema> builder)
        {
            // Clé primaire
            builder.HasKey(c => c.CinemaId);

            // Propriétés
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.Address)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(c => c.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.Property(c => c.City)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.Country)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.OpeningHours)
                   .IsRequired()
                   .HasMaxLength(50);

            // Relation un-à-plusieurs avec Showtime
            builder.HasMany<Showtime>()
                   .WithOne()
                   .HasForeignKey(s => s.CinemaId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation un-à-plusieurs avec Theater
            builder.HasMany<Theater>()
                   .WithOne()
                   .HasForeignKey(t => t.CinemaId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
