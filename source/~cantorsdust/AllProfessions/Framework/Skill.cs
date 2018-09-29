using StardewValley;

namespace AllProfessions.Framework
{
    /// <summary>A player skill type.</summary>
    internal enum Skill
    {
        /// <summary>The farming skill.</summary>
        Farming = Farmer.farmingSkill,

        /// <summary>The fishing skill.</summary>
        Fishing = Farmer.fishingSkill,

        /// <summary>The foraging skill.</summary>
        Foraging = Farmer.foragingSkill,

        /// <summary>The mining skill.</summary>
        Mining = Farmer.miningSkill,

        /// <summary>The combat skill.</summary>
        Combat = Farmer.combatSkill,

        /// <summary>The (disabled) luck skill.</summary>
        Luck = Farmer.luckSkill
    }
}
