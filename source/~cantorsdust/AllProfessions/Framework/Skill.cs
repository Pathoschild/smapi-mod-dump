/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cantorsdust/StardewMods
**
*************************************************/

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
