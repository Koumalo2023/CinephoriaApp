using CinephoriaServer.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CinephoriaServer.Repository
{
    public class ContactConfig : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            // Clé primaire
            builder.HasKey(c => c.ContactId);

            // Propriétés
            builder.Property(c => c.UserName)
                   .HasMaxLength(100);

            builder.Property(c => c.Title)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(c => c.Description)
                   .IsRequired();

            builder.Property(c => c.CreatedAt)
                   .IsRequired();

            // Relation facultative avec AppUser
            builder.HasOne(c => c.AppUser)
                   .WithMany()
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
