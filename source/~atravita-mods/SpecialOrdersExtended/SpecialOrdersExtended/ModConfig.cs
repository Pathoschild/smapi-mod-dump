/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Integrations.GMCMAttributes;

namespace SpecialOrdersExtended;

/// <summary>
/// Config class for this mod.
/// </summary>
internal sealed class ModConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether to be verbose or not.
    /// </summary>
    /// <remarks>Use this setting for anything that would be useful for other mod authors.</remarks>
    [GMCMDefaultIgnore]
    public bool Verbose { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether or not to suppress board updates before
    /// the board is opened.
    /// </summary>
    public bool SurpressUnnecessaryBoardUpdates { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not the tag cache should be used.
    /// </summary>
    public bool UseTagCache { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not Qi orders should avoid repeating until
    /// you've seen all of them.
    /// </summary>
    public bool AvoidRepeatingQiOrders { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not the other quest will be made available
    /// once all quests of the type are finished.
    /// </summary>
    public bool AllowNewQuestWhenFinished { get; set; } = true;
}