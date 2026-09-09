using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Domain.Entities.ValueObjects;

public readonly record struct Stamina
{
    public short CurrentStamina { get; init; }
    public short MaxStamina { get; init; }
    public short RegenRatePerSecond { get; init; }

    public DateTimeOffset LastUpdateTime { get; init; }

    public Stamina(short currentStamina, short maxStamina, short regenRatePerSecond, DateTimeOffset lastUpdate)
    {
        CurrentStamina = currentStamina;
        MaxStamina = maxStamina;
        RegenRatePerSecond = regenRatePerSecond;
        LastUpdateTime = lastUpdate;
    }

    public static Result<Stamina> Create(short currentStamina, short maxStamina, short regenRatePerSecond, DateTimeOffset lastUpdate)
    {
        if (maxStamina <= 0) return Error.Validation("Max Stamina invalid value", "The Max Stamina Value cannot be 0 or less");
        if (regenRatePerSecond <= 0) return Error.Validation("Stamina Regen Rate invalid value", "The Regen Rate cannot be 0 or less");
        if (lastUpdate == DateTimeOffset.MinValue) return Error.Validation("LastUpdateTime Invalid", "The Last Update Time cannot be default value");

        if (currentStamina < 0) currentStamina = 0;
        if (currentStamina >= maxStamina) currentStamina = maxStamina;

        return new Stamina(currentStamina, maxStamina, regenRatePerSecond, lastUpdate);
    }

    public short GetActualStamina(DateTimeOffset now)
    {
        double secondPassed = (now - LastUpdateTime).TotalSeconds;

        double regenerated = secondPassed * RegenRatePerSecond;

        double actual = CurrentStamina + regenerated;

        if (actual > MaxStamina) return MaxStamina;
        
        return (short)actual;
    }



}
