using NexusGameEngine.Domain.Exceptions;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Domain.Entities.ValueObjects;

public readonly record struct Stat
{
    private const int BASE_EXPERIENCE_TO_LEVEL = 2000;
    private const double XP_CURVE_EXPONENT = 1.5;
    private const byte MAX_LEVEL = 100;

    public string Name { get; init; }
    public int Experience { get; init; }
    public byte Level { get; init; }

    public bool CanAddXp => Level < MAX_LEVEL;
    public int ExperienceCapForNextLevel => GetExpCapForNextLevel(Level);

    private Stat(string name)
    {
        Name = name;
        Experience = 0;
        Level = 0;
    }

    private Stat(string name, int experience, byte level)
    {
        Name = name;
        Experience = experience;
        Level = level;
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

    public Result<Stat> AddExperience(int expToAdd)
    {
        if (expToAdd <= 0) return this;
        if (!CanAddXp) return this;

        long tempExp = (long)Experience + expToAdd;
        byte tempLevel = Level;

        while (tempExp >= GetExpCapForNextLevel(tempLevel) && tempLevel < MAX_LEVEL)
        {
            tempExp -= GetExpCapForNextLevel(tempLevel);
            tempLevel++;
        }

        if (tempLevel >= MAX_LEVEL) tempExp = 0;

        return Stat.CreateFull(Name, (int)tempExp, tempLevel);
    }
    private static int GetExpCapForNextLevel(byte currentLevel)
    {
        int targetLevel = currentLevel + 1;
        return (int)(BASE_EXPERIENCE_TO_LEVEL * Math.Pow(targetLevel, XP_CURVE_EXPONENT));
    }

}
