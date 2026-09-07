using NexusGameEngine.Domain.Exceptions;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Domain.Entities.ValueObjects;

public sealed record Stat
{
    private const int BASE_EXPERIENCE_TO_LEVEL = 2000;
    private const double XP_CURVE_EXPONENT = 1.5;
    private const byte MAX_LEVEL = 100;

    public string Name { get; init; }
    public int Experience { get; private set; }
    private int experienceCapForNextLevel;
    public byte Level { get; private set; }

    public bool CanAddXp => Level < MAX_LEVEL;

    private Stat(string name)
    {
        Name = name;
        Experience = 0;
        Level = 0;
        experienceCapForNextLevel = GetExpCapForNextLevel();
    }

    private Stat(string name, int experience, byte level)
    {
        Name = name;
        Experience = experience;
        Level = level;
        experienceCapForNextLevel = GetExpCapForNextLevel();
    }

    public static Stat Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Empty Name for Stat: The name of the Stat cannot be empty", 500, "Empty Stat Name String");

        return new Stat(name);
    }

    public static Stat CreateFull(string name, int exp, byte level)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Empty Name for Stat: The name of the Stat cannot be empty", 500, "Empty Stat Name String");

        return new Stat(name, exp, level);
    }

    public Result<bool> AddExperience(int expToAdd)
    {
        if (expToAdd <= 0) return false;
        if (!CanAddXp) return false;

        Experience += expToAdd;

        while (Experience >= experienceCapForNextLevel && Level < MAX_LEVEL)
        {
            LevelUp();
        }

        if (Level >= MAX_LEVEL) Experience = 0;

        return true;
    }
    public void LevelUp()
    {
        Level++;
        Experience -= experienceCapForNextLevel;
        experienceCapForNextLevel = GetExpCapForNextLevel();
    }
    private int GetExpCapForNextLevel()
    {
        int targetLevel = Level + 1;
        return (int)(BASE_EXPERIENCE_TO_LEVEL * Math.Pow(targetLevel, XP_CURVE_EXPONENT));
    }

}
