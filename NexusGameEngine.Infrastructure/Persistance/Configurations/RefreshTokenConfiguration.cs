using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusGameEngine.Domain.Entities;

namespace NexusGameEngine.Infrastructure.Persistance.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id)
            .ValueGeneratedNever();

        builder.Property(rt => rt.UserId)
            .IsRequired();
        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId);

        builder.Property(rt => rt.TokenHash)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(rt => rt.Expiry)
            .IsRequired();

        builder.Property(rt => rt.RevokeAt)
            .IsRequired(false);

        builder.Property(rt => rt.ReplacedByTokenHash)
            .IsRequired(false)
            .HasMaxLength(128);
    }

}
