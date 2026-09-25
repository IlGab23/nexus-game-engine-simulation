using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Domain.Entities;

public sealed class MonsterTemplate
{
    public Guid Id { get; init; }
    public string Name { get; private set; }
    public int BaseHealth { get; private set; }
    public int BaseDamage { get; private set; }
    public float AttackRange { get; private set; }
    public bool IsBoss { get; init; } = false;
    public string LootTablePayload { get; private set; }

    private MonsterTemplate(Guid id, string name, int baseHealth, int baseDamage, float attackRange, bool isBoss, string lootTablePayload)
    {
        Id = id;
        Name = name;
        BaseHealth = baseHealth;
        BaseDamage = baseDamage;
        AttackRange = attackRange;
        IsBoss = isBoss;
        LootTablePayload = lootTablePayload;
    }

    public static Result<MonsterTemplate> Create(string name, int baseHealth, int baseDamage, float attackRange, string lootTablePayload, bool isBoss = false)
    {
        if (string.IsNullOrWhiteSpace(name)) return Error.Validation("MonsterTemplateCreate.EmptyName", "Name cannot be empty");
        if (name.Length <= 3) return Error.Validation("MonsterTemplateCreate.InvalidName", "Name cannot be shorter than 4 char");
        if (name.Length >= 256) return Error.Validation("MonsterTemplateCreate.InvalidName", "Name cannot be longer than 255 char");
        if (baseHealth <= 0) return Error.Validation("MonsterTemplateCreate.InvalidBaseHealth", "Base Health cannot be 0 or less");
        if (baseDamage <= 0) return Error.Validation("MonsterTemplateCreate.InvalidBaseDamage", "Base Damage cannot be 0 or less");
        if (attackRange <= 0) return Error.Validation("MonsterTemplateCreate.InvalidAttackRange", "Attack Range cannot be 0 or less");
        if (attackRange > 10.0f) return Error.Validation("MonsterTemplateCreate.InvalidAttackRange", "Attack Range cannot be more than 10.0, he is not shooting with a gun");
        if (baseHealth <= 100 && baseDamage <= 20 && isBoss) return Error.Conflict("MonsterTemplateCreate.InvalidBoss", "This entity cannot be a boss, his stats are too low to be one");
        if (string.IsNullOrWhiteSpace(lootTablePayload)) return Error.Validation("MonsterTemplateCreate.InvalidLootPayload", "This entity have to drop something, his payload cannot be empty");

        return new MonsterTemplate(Guid.NewGuid(), name, baseHealth, baseDamage, attackRange, isBoss, lootTablePayload);
    }
}
