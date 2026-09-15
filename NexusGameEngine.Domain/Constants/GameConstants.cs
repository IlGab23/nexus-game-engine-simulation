using NexusGameEngine.Domain.Entities.ValueObjects;
using NexusGameEngine.Domain.Enums;

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

    public static class SpecialSkillsCatalog
    {
        public static readonly SpecialSkill StrengthLevel1 = SpecialSkill.Create("Ogre's Might", StatType.Strength, 1.2).Value;
        public static readonly SpecialSkill StrengthLevel2 = SpecialSkill.Create("Giant's Wrath", StatType.Strength, 1.5).Value;
        public static readonly SpecialSkill StrengthLevel3 = SpecialSkill.Create("Titan's Fury", StatType.Strength, 1.8).Value;
        public static readonly SpecialSkill ConstitutionLevel1 = SpecialSkill.Create("Stonewall", StatType.Constitution, 1.21).Value;
        public static readonly SpecialSkill ConstitutionLevel2 = SpecialSkill.Create("Dragon Scales", StatType.Constitution, 1.56).Value;
        public static readonly SpecialSkill IntelligenceLevel1 = SpecialSkill.Create("Scholar's Insight", StatType.Intelligence, 2).Value;
        public static readonly SpecialSkill IntelligenceLevel2 = SpecialSkill.Create("Arcane Awakening", StatType.Intelligence, 4).Value;
        public static readonly SpecialSkill IntelligenceLevel3 = SpecialSkill.Create("Cosmic Resonance", StatType.Intelligence, 6).Value;
        public static readonly SpecialSkill DexterityLevel1 = SpecialSkill.Create("Wind Walker", StatType.Dexterity, 1.4).Value;
        public static readonly SpecialSkill DexterityLevel2 = SpecialSkill.Create("Shadow Dancer", StatType.Dexterity, 1.8).Value;
        public static readonly SpecialSkill DexterityLevel3 = SpecialSkill.Create("Lightning Phantom", StatType.Dexterity, 2.2).Value;
    }

    public static class DailyRewardData
    {
        public static readonly int[] RandomMoney = [100, 200, 300, 500, 1000];
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

