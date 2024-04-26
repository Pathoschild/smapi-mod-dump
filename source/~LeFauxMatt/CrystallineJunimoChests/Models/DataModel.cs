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

/// <summary>The data model for the cost, sound, and colors.</summary>
internal sealed class DataModel(int cost, string sound, ColorData[] colors)
{
    /// <summary>Gets or sets the sound.</summary>
    public string Sound { get; set; } = sound;

    /// <summary>Gets or sets the colors.</summary>
    public ColorData[] Colors { get; set; } = colors;
}