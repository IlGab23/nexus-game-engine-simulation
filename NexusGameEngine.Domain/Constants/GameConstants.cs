namespace NexusGameEngine.Domain.Constants;


public static class GameConstants
{
    public static class Actions
    {
        public const string ClaimDailyReward = "ClaimDailyReward";
        public const string UseSpecialSkill = "UseSpecialSkill";
    }

    public static class ActionCooldowns
    {
        public static readonly TimeSpan UseSpecialSkillCooldown = TimeSpan.FromMinutes(2);
    }

    public static class PlayerData
    {
        public const string STAT_MAIN_LEVEL_NAME = "Main Level";
        public const string STAT_STRENGTH_NAME = "Strength";
        public const string STAT_DEXTERITY_NAME = "Dexterity";
        public const string STAT_INTELLIGENCE_NAME = "Intelligence";
        public const string STAT_CONSTITUTION_NAME = "Constitution";

        public const int INITIAL_MONEY = 200;

        public const short INITIAL_HEALTH = 100;
        public const short INITIAL_MAX_HEALTH = 100;
        public const short HEALTH_ON_RESPAWN = 30;

        public const short INITIAL_STAMINA = 100;
        public const short INITIAL_MAX_STAMINA = 100;
        public const short INITIAL_STAMINA_REGEN_RATE = 2;
    }
}

