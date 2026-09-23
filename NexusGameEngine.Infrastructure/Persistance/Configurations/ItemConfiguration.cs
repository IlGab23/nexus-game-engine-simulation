using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusGameEngine.Domain.Entities;

namespace NexusGameEngine.Infrastructure.Persistance.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("ItemsCatalog");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .ValueGeneratedNever();

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.Description)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(i => i.MaxStackQuantity)
            .IsRequired()
            .HasDefaultValue(1);


        builder.Property(i => i.ItemType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(i => i.ActionPayload)
            .IsRequired();
    }

}
