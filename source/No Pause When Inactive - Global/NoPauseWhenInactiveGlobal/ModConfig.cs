/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thakyZ/StardewValleyMods
**
*************************************************/

namespace NoPauseWhenInactiveGlobal;

/// <summary>
/// The mod's config class.
/// </summary>
internal class ModConfig
{
    /// <summary>
    /// An option whether or not to disable the game from pausing when outside a save game.
    /// Enabled by default as it is normally wanted if one has this mod.
    /// </summary>
    public bool DisableGamePause { get; set; } = true;
}
