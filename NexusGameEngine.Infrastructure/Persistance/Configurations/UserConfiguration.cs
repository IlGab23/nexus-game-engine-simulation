using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusGameEngine.Domain.Entities;
using NexusGameEngine.Domain.Entities.ValueObjects;

namespace NexusGameEngine.Infrastructure.Persistance.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.HasOne(u => u.SystemRole)
            .WithMany()
            .HasForeignKey(u => u.SystemRoleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(40);

        builder.HasIndex(u => u.UserName)
            .IsUnique();


        builder.Property(u => u.Email)
            .HasConversion(
                emailObj => emailObj.Value,
                dbString => Email.Create(dbString).Value)
            .HasColumnName("Email")
            .IsRequired()
            .HasMaxLength(254);

        builder.HasIndex(u => u.Email)
            .IsUnique();


        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(128);
    }

}
