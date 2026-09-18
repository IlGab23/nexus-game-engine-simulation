using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusGameEngine.Domain.Entities;

namespace NexusGameEngine.Infrastructure.Persistance.Configurations;

public class InventorySlotConfiguration : IEntityTypeConfiguration<InventorySlot>
{
    public void Configure(EntityTypeBuilder<InventorySlot> builder)
    {
        builder.ToTable("InventorySlots");
        builder.HasKey(inv => inv.Id);
        builder.Property(inv => inv.Id)
            .ValueGeneratedNever();

        builder.HasOne(inv => inv.Player)
            .WithMany(p => p.InventorySlots)
            .HasForeignKey(inv => inv.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(inv => inv.Item)
            .WithMany()
            .HasForeignKey(inv => inv.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(inv => inv.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(inv => inv.StatePayload)
            .IsRequired();
    }

}
