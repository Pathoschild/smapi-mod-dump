/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/ExpFromMonsterKillsOnFarm
**
*************************************************/

namespace ExpFromMonsterKillsOnFarm;

/// <summary>
/// The configuration class for this mod.
/// </summary>
internal class ModConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether to grant monster kill xp on the farm.
    /// </summary>
    public bool GainExp { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether billboard quests will be updated by farm kills.
    /// </summary>
    public bool QuestCompletion { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether special order objects will be updated by farm kills.
    /// </summary>
    public bool SpecialOrderCompletion { get; set; } = true;
}