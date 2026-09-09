using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Domain.Entities.ValueObjects;

public readonly record struct Health
{
    public short CurrentHealth { get; init; }
    public short MaxHealth { get; init; }

    public Health(short currentHealth, short maxHealth)
    {
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
    }

    public static Result<Health> Create(int currentHealth, short maxHealth)
    {
        if (maxHealth <= 0) return Error.Validation("Max Health invalid value", "The Max Health Value cannot be 0 or less");

        if (currentHealth < 0) currentHealth = 0;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        return new Health((short)currentHealth, maxHealth);
    }

    public Result<Health> Add(int healthDiff)
    {
        int newHealthInteger = CurrentHealth + healthDiff;

        return Health.Create(newHealthInteger, MaxHealth);
    }
}
