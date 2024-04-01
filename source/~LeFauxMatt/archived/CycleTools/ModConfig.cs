/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.CycleTools;

using StardewModdingAPI.Utilities;

/// <summary>
///     Mod config data for Cycle Tools.
/// </summary>
internal sealed class ModConfig
{
    /// <summary>
    ///     Gets or sets the key to hold to cycle through tools.
    /// </summary>
    public KeybindList ModifierKey { get; set; } = new(SButton.LeftShift);
}