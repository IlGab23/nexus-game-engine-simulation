using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusGameEngine.Domain.Entities;

namespace NexusGameEngine.Infrastructure.Persistance.Configurations;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("Players");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();
        builder.HasOne(p => p.User)
            .WithOne(u => u.Player)
            .HasForeignKey<Player>(p => p.Id);

        builder.ComplexProperty(p => p.MainLevel, statBuilder =>
        {
            statBuilder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(30);

            statBuilder.Property(s => s.Experience)
                .IsRequired();

            statBuilder.Property(s => s.Level)
                .IsRequired();
        });

        builder.ComplexProperty(p => p.Strength, statBuilder =>
        {
            statBuilder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(30);

            statBuilder.Property(s => s.Experience)
                .IsRequired();

            statBuilder.Property(s => s.Level)
                .IsRequired();
        });

        builder.ComplexProperty(p => p.Dexterity, statBuilder =>
        {
            statBuilder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(30);

            statBuilder.Property(s => s.Experience)
                .IsRequired();

            statBuilder.Property(s => s.Level)
                .IsRequired();
        });

        builder.ComplexProperty(p => p.Intelligence, statBuilder =>
        {
            statBuilder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(30);

            statBuilder.Property(s => s.Experience)
                .IsRequired();

            statBuilder.Property(s => s.Level)
                .IsRequired();
        });

        builder.ComplexProperty(p => p.Constitution, statBuilder =>
        {
            statBuilder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(30);

            statBuilder.Property(s => s.Experience)
                .IsRequired();

            statBuilder.Property(s => s.Level)
                .IsRequired();
        });

        builder.ComplexProperty(p => p.PlayerHealth, healthBuidler =>
        {
            healthBuidler.Property(h => h.CurrentHealth)
                .HasColumnName("Health")
                .IsRequired();

            healthBuidler.Property(h => h.MaxHealth)
                .HasColumnName("MaxHealth")
                .IsRequired();
        });

        builder.ComplexProperty(p => p.PlayerStamina, staminaBuilder =>
        {
            staminaBuilder.Property(st => st.CurrentStamina)
                .HasColumnName("Stamina")
                .IsRequired();

            staminaBuilder.Property(st => st.MaxStamina)
                .HasColumnName("MaxStamina")
                .IsRequired();

            staminaBuilder.Property(st => st.RegenRatePerSecond)
                .HasColumnName("StaminaRegenRate")
                .IsRequired();
        });

        builder.Property(p => p.Money)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.IsAlive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.ComplexProperty(p => p.ActiveSpecialSkill, skillBuilder =>
        {
            skillBuilder.IsRequired(false);
            skillBuilder.Property(sk => sk.Name)
                .HasMaxLength(50)
                .HasColumnName("SpecialSkill_Name");

            skillBuilder.Property(sk => sk.TargetStat)
                .HasColumnName("SpecialSkill_TargetStat");

            skillBuilder.Property(sk => sk.BonusPercentage)
                .HasColumnName("SpecialSkill_BonusPercentage")
                .HasPrecision(5, 2);
        });

        builder.ComplexCollection(p => p.Cooldowns, cooldownBuilder => cooldownBuilder.ToJson("Cooldowns"));
    }

}
