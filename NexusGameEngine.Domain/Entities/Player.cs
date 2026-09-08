using NexusGameEngine.Domain.Constants;
using NexusGameEngine.Domain.Entities.ValueObjects;
using NexusGameEngine.Domain.Enums;
using NexusGameEngine.Domain.ResultPattern;
namespace NexusGameEngine.Domain.Entities;

public sealed class Player
{
    /* 
     * TODO: DA SVILUPPARE PER LA EPIC 3
     * 
     * 1. CHIAVE PRIMARIA CONDIVISA (Shared Primary Key):
     *    - La classe avrà un singolo campo: public Guid Id { get; private set; }
     *    - Questo Id sarà sia la Primary Key del Player, sia la Foreign Key 
     *      che punta all'Id dello User. I due Guid coincideranno.
     *    - Ricordarsi di mappare la relazione 1:1 con User tramite Fluent API.
     * 
     * 2. INVENTARIO E OGGETTI (Task INV-01):
     *    - L'inventario si baserà sull'entità relazionale `InventorySlot`.
     *    - L'entità `Item` fungerà da catalogo di sola lettura.
     *    - L'entità `InventorySlot` mapperà la relazione tra `Player` e `Item` aggiungendo la quantità.
     *    - Il Player avrà quindi presumibilmente: public List<InventorySlot> InventorySlots { get; private set; }
     *    - NOTA ARCHITETTURALE: Tutte le configurazioni EF Core (Fluent API) relative a queste 
     *      nuove entità dovranno risiedere ESCLUSIVAMENTE nell'Infrastructure Layer.
     */

    public Guid Id { get; init; }
    public User User { get; private set; }

    public Stat MainLevel { get; private set; }
    public Stat Strength { get; private set; }
    public Stat Dexterity { get; private set; }
    public Stat Intelligence { get; private set; }
    public Stat Constitution { get; private set; }

    public Health PlayerHealth { get; private set; }
    public Stamina PlayerStamina { get; private set; }

    public int Money { get; private set; }

    public bool IsAlive { get; private set; } = true;

    // public List<InventorySlot> InventorySlots {get; private set; } //TODO: Enable when entity InventorySlots has been added

    public Player(Guid id, Stat mainLevel, Stat strength, Stat dexterity, Stat intelligence, Stat constitution, Health playerHealth, Stamina playerStamina, int money)
    {
        Id = id;
        MainLevel = mainLevel;
        Strength = strength;
        Dexterity = dexterity;
        Intelligence = intelligence;
        Constitution = constitution;
        PlayerHealth = playerHealth;
        PlayerStamina = playerStamina;
        Money = money;
    }

    public static Result<Player> Create(Guid userId, DateTimeOffset currentTime)
    {
        Stat mainLevel = Stat.Create(PlayerData.STAT_MAIN_LEVEL_NAME);
        Stat strength = Stat.Create(PlayerData.STAT_STRENGTH_NAME);
        Stat dexterity = Stat.Create(PlayerData.STAT_DEXTERITY_NAME);
        Stat intelligence = Stat.Create(PlayerData.STAT_INTELLIGENCE_NAME);
        Stat constitution = Stat.Create(PlayerData.STAT_CONSTITUTION_NAME);

        var healthResult = Health.Create(PlayerData.INITIAL_HEALTH, PlayerData.INITIAL_MAX_HEALTH);
        if (healthResult.IsFailure) return healthResult.ErrorList;

        var staminaResult = Stamina.Create(PlayerData.INITIAL_STAMINA, PlayerData.INITIAL_MAX_STAMINA, PlayerData.INITIAL_STAMINA_REGEN_RATE, currentTime);
        if (staminaResult.IsFailure) return staminaResult.ErrorList;

        return new Player(userId, mainLevel, strength, dexterity, intelligence, constitution, healthResult.Value, staminaResult.Value, PlayerData.INITIAL_MONEY);
    }

    public Result<bool> GainExperience(StatType type, int ammount)
    {
        if (!IsAlive) return Error.Validation("Cannot Perform Action", "Player is dead and cannot gain experience");
        switch (type)
        {
            case StatType.MainLevel:
                var result = MainLevel.AddExperience(ammount);
                if (result.IsSuccess)
                {
                    MainLevel = result.Value;
                }
                break;
            case StatType.Strength:
                var resultStr = Strength.AddExperience(ammount);
                if (resultStr.IsSuccess)
                {
                    Strength = resultStr.Value;
                }
                break;
            case StatType.Dexterity:
                var resultDex = Dexterity.AddExperience(ammount);
                if (resultDex.IsSuccess)
                {
                    Dexterity = resultDex.Value;
                }
                break;
            case StatType.Intelligence:
                var resultInt = Intelligence.AddExperience(ammount);
                if (resultInt.IsSuccess)
                {
                    Intelligence = resultInt.Value;
                }
                break;
            case StatType.Constitution:
                var resultCon = Constitution.AddExperience(ammount);
                if (resultCon.IsSuccess)
                {
                    Constitution = resultCon.Value;
                }
                break;
        }

        return true;
    }

    public Result<bool> TakeDamage(int damage)
    {
        if (!IsAlive) return Error.Validation("Cannot Perform Action", "Player is dead and cannot take damage");
        if (damage <= 0) return false;

        var newHealth = PlayerHealth.Add(damage * -1); // multiplied for -1 to make it negative number

        if (newHealth.IsSuccess)
        {
            PlayerHealth = newHealth.Value;
            if (PlayerHealth.CurrentHealth <= 0) IsAlive = false;
            return true;
        }

        return false;
    }

    public Result<bool> Heal(int hpGain)
    {
        if (!IsAlive) return Error.Validation("Cannot Perform Action", "Player is dead and cannot Heal");
        if (hpGain <= 0) return false;

        var newHealth = PlayerHealth.Add(hpGain);

        if (newHealth.IsSuccess)
        {
            PlayerHealth = newHealth.Value;
            return true;
        }

        return false;
    }

    public Result<bool> ConsumeStamina(short ammount, DateTimeOffset currentTime)
    {
        if (!IsAlive) return Error.Validation("Cannot Perform Action", "Player is dead and cannot consume Stamina");
        short actualStamina = PlayerStamina.GetActualStamina(currentTime);
        short currentMaxStamina = PlayerStamina.MaxStamina;
        short currentRegenRate = PlayerStamina.RegenRatePerSecond;

        if (actualStamina < ammount) return false;

        var staminaResult = Stamina.Create((short)(actualStamina - ammount), currentMaxStamina, currentRegenRate, currentTime);

        if (staminaResult.IsSuccess)
        {
            PlayerStamina = staminaResult.Value;
            return true;
        }

        return false;
    }

    public Result<bool> AddMoney(int ammount)
    {
        if (ammount <= 0) return Error.Validation("Add Money Invalid Value", "Money Added are 0 or less");

        Money += ammount;

        return true;
    }

    public Result<bool> SpendMoney(int ammount)
    {
        if (!IsAlive) return Error.Validation("Cannot Perform Action", "Player is dead and cannot spend money");
        if (ammount <= 0) return Error.Validation("Spend Money Invalid Value", "Cannot spend 0 money or less", [$"Money tried to be spent: {ammount}"]);

        if (Money < ammount) return Error.Validation("Spend Money Invalid Value", "Cannot spend more than you have");

        Money -= ammount;

        return true;
    }

    public Result<bool> PlayerRespawned(DateTimeOffset currentTime)
    {
        if (IsAlive) return Error.Validation("Cannot Respawn", "Player is already alive and cannot respawn");

        var newHealthResult = Health.Create(PlayerData.HEALTH_ON_RESPAWN, PlayerHealth.MaxHealth);
        if (newHealthResult.IsFailure) return newHealthResult.ErrorList;

        var newStaminaResult = Stamina.Create(PlayerStamina.MaxStamina, PlayerStamina.MaxStamina, PlayerStamina.RegenRatePerSecond, currentTime);
        if (newStaminaResult.IsFailure) return newStaminaResult.ErrorList;

        IsAlive = true;

        PlayerHealth = newHealthResult.Value;
        PlayerStamina = newStaminaResult.Value;

        return true;
    }
}
