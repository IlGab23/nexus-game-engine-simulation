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

    public Guid? MapZoneId { get; private set; }
    public MapZone MapZone { get; private set; } //Nav property

    public Vector3 WorldPosition { get; private set; }
    public DateTimeOffset LastMove { get; private set; }
    public float BaseMovementSpeed { get; private set; } = 5.0f; //TODO: This prop will be edited in EPIC 5

    public SpecialSkill? ActiveSpecialSkill { get; private set; }

    private readonly List<InventorySlot> _inventorySlots = [];
    public IReadOnlyCollection<InventorySlot> InventorySlots => _inventorySlots.AsReadOnly();

    private readonly List<PlayerCooldown> _cooldowns = [];
    public IList<PlayerCooldown> Cooldowns => _cooldowns.AsReadOnly();

    private Player(Guid id, Stat mainLevel, Stat strength, Stat dexterity, Stat intelligence, Stat constitution, Health playerHealth, Stamina playerStamina, int money)
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

#pragma warning disable CS8618
    private Player() { }
#pragma warning restore CS8618

    public static Result<Player> Create(Guid userId, DateTimeOffset currentTime)
    {
        Stat mainLevel = Stat.Create(GameConstants.PlayerData.STAT_MAIN_LEVEL_NAME);
        Stat strength = Stat.Create(GameConstants.PlayerData.STAT_STRENGTH_NAME);
        Stat dexterity = Stat.Create(GameConstants.PlayerData.STAT_DEXTERITY_NAME);
        Stat intelligence = Stat.Create(GameConstants.PlayerData.STAT_INTELLIGENCE_NAME);
        Stat constitution = Stat.Create(GameConstants.PlayerData.STAT_CONSTITUTION_NAME);

        var healthResult = Health.Create(GameConstants.PlayerData.INITIAL_HEALTH, GameConstants.PlayerData.INITIAL_MAX_HEALTH);
        if (healthResult.IsFailure) return healthResult.ErrorList;

        var staminaResult = Stamina.Create(GameConstants.PlayerData.INITIAL_STAMINA, GameConstants.PlayerData.INITIAL_MAX_STAMINA, GameConstants.PlayerData.INITIAL_STAMINA_REGEN_RATE, currentTime);
        if (staminaResult.IsFailure) return staminaResult.ErrorList;

        return new Player(userId, mainLevel, strength, dexterity, intelligence, constitution, healthResult.Value, staminaResult.Value, GameConstants.PlayerData.INITIAL_MONEY);
    }

    public Result<bool> GainExperience(StatType type, int amount)
    {
        if (!IsAlive) return Error.Validation("Cannot Perform Action", "Player is dead and cannot gain experience");
        switch (type)
        {
            case StatType.MainLevel:
                var result = MainLevel.AddExperience(amount);
                if (result.IsFailure) return result.ErrorList;
                MainLevel = result.Value;
                break;
            case StatType.Strength:
                var resultStr = Strength.AddExperience(amount);
                if (resultStr.IsFailure) return resultStr.ErrorList;
                Strength = resultStr.Value;
                break;
            case StatType.Dexterity:
                var resultDex = Dexterity.AddExperience(amount);
                if (resultDex.IsFailure) return resultDex.ErrorList;
                Dexterity = resultDex.Value;
                break;
            case StatType.Intelligence:
                var resultInt = Intelligence.AddExperience(amount);
                if (resultInt.IsFailure) return resultInt.ErrorList;
                Intelligence = resultInt.Value;
                break;
            case StatType.Constitution:
                var resultCon = Constitution.AddExperience(amount);
                if (resultCon.IsFailure) return resultCon.ErrorList;
                Constitution = resultCon.Value;
                break;
        }

        return true;
    }

    public Result<bool> TakeDamage(int damage)
    {
        if (!IsAlive) return Error.Validation("Cannot Perform Action", "Player is dead and cannot take damage");
        if (damage <= 0) return Error.Validation("TakeDamage.InvalidValue", "Damage must be greater than zero");

        var newHealth = PlayerHealth.Add(damage * -1); // multiplied for -1 to make it negative number

        if (newHealth.IsFailure) return newHealth.ErrorList;

        PlayerHealth = newHealth.Value;
        if (PlayerHealth.CurrentHealth <= 0) IsAlive = false;

        return true;
    }

    public Result<bool> Heal(int hpGain)
    {
        if (!IsAlive) return Error.Validation("Cannot Perform Action", "Player is dead and cannot Heal");
        if (hpGain <= 0) return Error.Validation("Heal.InvalidValue", "Heal amount must be greater than zero");

        var newHealth = PlayerHealth.Add(hpGain);

        if (newHealth.IsFailure) return newHealth.ErrorList;

        PlayerHealth = newHealth.Value;

        return true;
    }

    public Result<bool> ConsumeStamina(short amount, DateTimeOffset currentTime)
    {
        if (!IsAlive) return Error.Validation("Cannot Perform Action", "Player is dead and cannot consume Stamina");
        short actualStamina = PlayerStamina.GetActualStamina(currentTime);
        short currentMaxStamina = PlayerStamina.MaxStamina;
        short currentRegenRate = PlayerStamina.RegenRatePerSecond;

        if (actualStamina < amount) return Error.Validation("Stamina.NotEnough", "Not enough stamina to perform this action");

        var staminaResult = Stamina.Create((short)(actualStamina - amount), currentMaxStamina, currentRegenRate, currentTime);

        if (staminaResult.IsFailure) return staminaResult.ErrorList;

        PlayerStamina = staminaResult.Value;

        return true;
    }

    public Result<bool> GainStamina(short amount, DateTimeOffset currentTime)
    {
        if (!IsAlive) return Error.Validation("Cannot Perform Action", "Player is dead and cannot consume Stamina");
        if (amount <= 0) return Error.Validation("Stamina.InvalidAmount", "Stamina amount to add cannot be 0 or less");

        short actualStamina = PlayerStamina.GetActualStamina(currentTime);
        short currentMaxStamina = PlayerStamina.MaxStamina;

        int newStaminaValue = actualStamina + amount;

        var staminaResult = Stamina.Create((short)(newStaminaValue), currentMaxStamina, PlayerStamina.RegenRatePerSecond, currentTime);

        if (staminaResult.IsFailure) return staminaResult.ErrorList;

        PlayerStamina = staminaResult.Value;

        return true;
    }

    public Result<bool> AddMoney(int amount)
    {
        if (amount <= 0) return Error.Validation("Add Money Invalid Value", "Money Added are 0 or less");

        Money += amount;

        return true;
    }

    public Result<bool> SpendMoney(int amount)
    {
        if (!IsAlive) return Error.Validation("Cannot Perform Action", "Player is dead and cannot spend money");
        if (amount <= 0) return Error.Validation("Spend Money Invalid Value", "Cannot spend 0 money or less", [$"Money tried to be spent: {amount}"]);

        if (Money < amount) return Error.Validation("Spend Money Invalid Value", "Cannot spend more than you have");

        Money -= amount;

        return true;
    }

    public Result<bool> PlayerRespawned(DateTimeOffset currentTime)
    {
        if (IsAlive) return Error.Validation("Cannot Respawn", "Player is already alive and cannot respawn");

        var newHealthResult = Health.Create(GameConstants.PlayerData.HEALTH_ON_RESPAWN, PlayerHealth.MaxHealth);
        if (newHealthResult.IsFailure) return newHealthResult.ErrorList;

        var newStaminaResult = Stamina.Create(PlayerStamina.MaxStamina, PlayerStamina.MaxStamina, PlayerStamina.RegenRatePerSecond, currentTime);
        if (newStaminaResult.IsFailure) return newStaminaResult.ErrorList;

        IsAlive = true;

        PlayerHealth = newHealthResult.Value;
        PlayerStamina = newStaminaResult.Value;

        return true;
    }

    public Result<bool> EquipSpecialSkill(SpecialSkill skill)
    {
        ActiveSpecialSkill = skill;

        return true;
    }

    public Result<bool> AddItemToInventory(Item item, int amount)
    {
        // BUSINESS LOGIC (Domain)
        // Check if the player already has a slot for this item
        var existentSlot = _inventorySlots.FirstOrDefault(iSlot => iSlot.ItemId == item.Id && iSlot.Quantity < item.MaxStackQuantity);

        string initialStatePayload = item.ItemType switch
        {
            ItemType.Weapon => "{}", // TODO EPIC EQUIP: serializzare un nuovo WeaponStatePayload con Durabilità 100
            ItemType.Armor => "{}",  // TODO EPIC EQUIP: serializzare un nuovo ArmorStatePayload
            _ => "{}"                // Consumabili e Misc non avranno mai uno stato dinamico oltre alla quantità
        };

        if (existentSlot is not null)
        {
            // IF SLOT EXISTS: Add quantity up to the MaxStackQuantity
            int actualQuantity = existentSlot.Quantity;
            int AmountToAdd = Math.Min(amount, item.MaxStackQuantity - actualQuantity);


            var addResult = existentSlot.AddQuantity(AmountToAdd, item.MaxStackQuantity);
            if (addResult.IsFailure) return addResult.ErrorList;

            // Calculate the remainder. If there are items left over, create a NEW slot (Multi-slotting)
            int itemOverFlow = amount - AmountToAdd;
            if (itemOverFlow > 0)
            {
                var invSlot = InventorySlot.Create(Id, item, itemOverFlow, initialStatePayload);
                if (invSlot.IsFailure) return invSlot.ErrorList;

                var addSlotResult = AddInventorySlot(invSlot.Value);
                if (addSlotResult.IsFailure) return addSlotResult.ErrorList;
            }
        }
        else
        {
            // IF SLOT DOES NOT EXIST: Create a new slot and add it to the Player
            var invSlot = InventorySlot.Create(Id, item, amount, initialStatePayload);
            if (invSlot.IsFailure) return invSlot.ErrorList;

            var addResult = AddInventorySlot(invSlot.Value);
            if (addResult.IsFailure) return addResult.ErrorList;
        }

        return true;
    }

    public Result<bool> AddInventorySlot(InventorySlot slot)
    {
        if (slot is null) return Error.Validation("Player.InvalidSlot", "The slot cannot be null");
        _inventorySlots.Add(slot);
        return true;
    }

    public Result<bool> RemoveInventorySlot(InventorySlot slot)
    {
        if (slot is null) return Error.Validation("Player.InvalidSlot", "The slot cannot be null");
        _inventorySlots.Remove(slot);
        return true;
    }

    public Result<bool> TryStartCooldown(string actionId, DateTimeOffset newReadyAt, DateTimeOffset currentTime)
    {
        PlayerCooldown? currentCooldown = _cooldowns.FirstOrDefault(c => c.ActionId == actionId);

        if (currentCooldown is not null && currentTime < currentCooldown.ReadyAt) return Error.Conflict("Cooldown.Active", "The action is not avabile yet");

        Result<PlayerCooldown> newCooldownResult = PlayerCooldown.Create(actionId, newReadyAt, currentTime);
        if (newCooldownResult.IsFailure) return newCooldownResult.ErrorList;

        if (currentCooldown is not null) _cooldowns.Remove(currentCooldown);

        _cooldowns.Add(newCooldownResult.Value);

        return true;
    }

    public Result<bool> MoveTo(Vector3 newPos, DateTimeOffset currentTime)
    {
        float distance = this.WorldPosition.CalculateDistance(newPos);

        double elapsedSeconds = (currentTime - this.LastMove).TotalSeconds;

        if (elapsedSeconds < 0) return Error.Conflict("Player.TimeHack", "The current time is older than the last move time");

        double maxAllowedDistance = (elapsedSeconds + 1.0) * this.BaseMovementSpeed;

        if (distance > maxAllowedDistance) return Error.Conflict("Player.SpeedHack", "Movement rejected: The player moved too fast or teleported");

        this.WorldPosition = newPos;
        this.LastMove = currentTime;

        return true;
    }
}
