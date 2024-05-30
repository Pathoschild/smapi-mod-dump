/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Models;

/// <summary>Represents an inventory tab.</summary>
internal sealed class TabData
{
    /// <summary>Gets or sets the id of the tab icon.</summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>Gets or sets the label of the tab.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Gets or sets the search term for the tab.</summary>
    public string SearchTerm { get; set; } = string.Empty;
}