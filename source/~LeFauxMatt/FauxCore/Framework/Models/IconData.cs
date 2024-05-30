/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.FauxCore.Framework.Models;

using Microsoft.Xna.Framework;

/// <summary>Represents an icon on a sprite sheet.</summary>
internal sealed class IconData
{
    /// <summary>Gets or sets the area of the icon.</summary>
    public Rectangle Area { get; set; } = Rectangle.Empty;

    /// <summary>Gets or sets the path to the icon.</summary>
    public string Path { get; set; } = string.Empty;
}