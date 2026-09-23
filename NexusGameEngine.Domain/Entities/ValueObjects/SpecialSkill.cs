using NexusGameEngine.Domain.Enums;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Domain.Entities.ValueObjects;

public sealed record SpecialSkill
{
    public string Name { get; init; }
    public StatType TargetStat { get; init; }

    public double BonusPercentage { get; init; }

    private SpecialSkill(string name, StatType targetStat, double bonusPercentage)
    {
        Name = name;
        TargetStat = targetStat;
        BonusPercentage = bonusPercentage;
    }

    public static Result<SpecialSkill> Create(string name, StatType targetStat, double bonusPercentage)
    {
        if (string.IsNullOrWhiteSpace(name)) return Error.Validation("SpecialSkill.NameEmpty", "The name of the skill cannot be empty");
        if (bonusPercentage <= 0) return Error.Validation("SpecialSkill.InvalidBonus", "Bonus percentage must be greater than 0");

        return new SpecialSkill(name, targetStat, bonusPercentage);
    }
}

