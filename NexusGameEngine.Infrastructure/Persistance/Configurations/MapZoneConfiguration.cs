using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusGameEngine.Domain.Entities;

namespace NexusGameEngine.Infrastructure.Persistance.Configurations;

public class MapZoneConfiguration : IEntityTypeConfiguration<MapZone>
{
    public void Configure(EntityTypeBuilder<MapZone> builder)
    {
        builder.ToTable("MapZones");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedNever();

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(m => m.MaxPlayers)
            .IsRequired();


    }

}
