/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.EasyAccess;

using StardewModdingAPI.Utilities;

/// <summary>
///     Controls config data.
/// </summary>
internal sealed class Controls
{
    /// <summary>
    ///     Gets or sets controls to collect items from producers.
    /// </summary>
    public KeybindList CollectItems { get; set; } = new(SButton.Delete);

    /// <summary>
    ///     Gets or sets controls to dispense items into producers.
    /// </summary>
    public KeybindList DispenseItems { get; set; } = new(SButton.Insert);
}