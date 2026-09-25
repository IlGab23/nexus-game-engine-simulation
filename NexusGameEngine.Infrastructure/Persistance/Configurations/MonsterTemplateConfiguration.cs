using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusGameEngine.Domain.Entities;
namespace NexusGameEngine.Infrastructure.Persistance.Configurations;

public class MonsterTemplateConfiguration : IEntityTypeConfiguration<MonsterTemplate>
{
    public void Configure(EntityTypeBuilder<MonsterTemplate> builder)
    {
        builder.ToTable("MonsterTemplates");
        builder.HasKey(mt => mt.Id);
        builder.Property(mt => mt.Id)
            .ValueGeneratedNever();

        builder.Property(mt => mt.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(mt => mt.BaseHealth)
            .IsRequired();

        builder.Property(mt => mt.BaseDamage)
            .IsRequired();

        builder.Property(mt => mt.AttackRange)
            .IsRequired();

        builder.Property(mt => mt.IsBoss)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(mt => mt.LootTablePayload)
            .IsRequired();
    }

}
