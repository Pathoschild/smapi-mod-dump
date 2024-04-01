/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.CrystallineJunimoChests.Models;

using Microsoft.Xna.Framework;

/// <summary>The data model for the color and items.</summary>
internal sealed class ColorData(string name, string item, Color color)
{
    /// <summary>Gets or sets the name of the color.</summary>
    public string Name { get; set; } = name;

    /// <summary>Gets or sets the item required to change to the color.</summary>
    public string Item { get; set; } = item;

    /// <summary>Gets or sets the color.</summary>
    public Color Color { get; set; } = color;
}