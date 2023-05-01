/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;

using CommunityToolkit.Diagnostics;

using NetEscapades.EnumGenerators;

using static System.Numerics.BitOperations;

namespace AtraShared.ConstantsAndEnums;

/// <summary>
/// Wallet items as flags....
/// </summary>
[Flags]
[EnumExtensions]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Should be obvious.")]
public enum Skills
{
    None = 0,

    Farming = 0b1 << Farmer.farmingSkill,
    Mining = 0b1 << Farmer.miningSkill,
    Fishing = 0b1 << Farmer.fishingSkill,
    Foraging = 0b1 << Farmer.foragingSkill,
    Combat = 0b1 << Farmer.combatSkill,
    Luck = 0b1 << Farmer.luckSkill,
}

/// <summary>
/// Extensions for the Skills enum.
/// </summary>
public static partial class SkillsExtensions
{
    private static readonly Skills[] _all = GetValues().Where(a => PopCount((uint)a) == 1).ToArray();

    /// <summary>
    /// Gets a span containing all wallet items.
    /// </summary>
    public static ReadOnlySpan<Skills> All => new(_all);

    /// <summary>
    /// Checks if this specific farmer has a specific skill level
    /// </summary>
    /// <param name="farmer">Farmer to check.</param>
    /// <param name="skills">Skill to check for.</param>
    /// <param name="includeBuffs">Whether or not to include buffs.</param>
    /// <returns>True if that farmer has this wallet item.</returns>
    public static int GetSkillLevelFromEnum(this Farmer farmer, Skills skills, bool includeBuffs = true)
    {
        Guard.IsEqualTo(PopCount((uint)skills), 1);

        switch (skills)
        {
            case Skills.Mining:
                return includeBuffs ? farmer.MiningLevel : farmer.miningLevel.Value;
            case Skills.Farming:
                return includeBuffs ? farmer.FarmingLevel : farmer.farmingLevel.Value;
            case Skills.Foraging:
                return includeBuffs ? farmer.ForagingLevel : farmer.foragingLevel.Value;
            case Skills.Combat:
                return includeBuffs ? farmer.CombatLevel : farmer.combatLevel.Value;
            case Skills.Fishing:
                return includeBuffs ? farmer.FishingLevel : farmer.fishingLevel.Value;
            case Skills.Luck:
                return includeBuffs ? farmer.LuckLevel : farmer.luckLevel.Value;
        }

        ThrowHelper.ThrowArgumentOutOfRangeException($"{skills.ToStringFast()} does not correspond to a single vanilla skill!");
        return 0;
    }
}