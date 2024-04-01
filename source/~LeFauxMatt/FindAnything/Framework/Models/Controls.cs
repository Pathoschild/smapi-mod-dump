/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.FindAnything.Framework.Models;

using StardewModdingAPI.Utilities;

/// <summary>Controls config data.</summary>
internal sealed class Controls
{
    /// <summary>Gets or sets controls to toggle search bar on or off.</summary>
    public KeybindList ToggleSearch { get; set; } = new(
        new Keybind(SButton.LeftControl, SButton.F),
        new Keybind(SButton.RightControl, SButton.F));
}