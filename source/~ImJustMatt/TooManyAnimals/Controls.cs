/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.TooManyAnimals;

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

/// <summary>
///     Controls config data.
/// </summary>
internal class Controls
{
    /// <summary>
    ///     Gets or sets controls to switch to next page.
    /// </summary>
    public KeybindList NextPage { get; set; } = new(SButton.DPadRight);

    /// <summary>
    ///     Gets or sets controls to switch to previous page.
    /// </summary>
    public KeybindList PreviousPage { get; set; } = new(SButton.DPadLeft);
}