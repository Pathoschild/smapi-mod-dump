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
    internal bool Verbose { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether or not to surpress board updates before
    /// the board is opened.
    /// </summary>
    internal bool SurpressUnnecessaryBoardUpdates { get; set; } = true;
}