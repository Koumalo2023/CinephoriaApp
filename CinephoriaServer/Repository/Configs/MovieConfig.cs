﻿using CinephoriaServer.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.Repository
{
    public class MovieConfig : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            // Clé primaire
            builder.HasKey(m => m.MovieId);

            // Propriétés
            builder.Property(m => m.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(m => m.Description)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(m => m.Genre)
                   .IsRequired();

            builder.Property(m => m.Duration)
                   .IsRequired();

            builder.Property(m => m.Director)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(m => m.ReleaseDate)
                   .IsRequired();

            builder.Property(m => m.MinimumAge)
                   .IsRequired();

            builder.Property(m => m.IsFavorite)
                   .HasDefaultValue(false);

            builder.Property(m => m.AverageRating)
                   .HasDefaultValue(0.0);

            // Relation avec Showtime
            builder.HasMany(m => m.Showtimes)
                   .WithOne(s => s.Movie)
                   .HasForeignKey(s => s.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation avec MovieRating
            builder.HasMany(m => m.MovieRatings)
                   .WithOne(mr => mr.Movie)
                   .HasForeignKey(mr => mr.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
