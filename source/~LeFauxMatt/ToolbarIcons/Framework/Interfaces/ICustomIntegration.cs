/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ToolbarIcons.Framework.Interfaces;

/// <summary>Represents an integration which is directly supported by this mod.</summary>
internal interface ICustomIntegration
{
    /// <summary>Gets the index of the icon on the sprite sheet.</summary>
    int Index { get; }

    /// <summary>Gets the text used when hovering over the toolbar icon.</summary>
    string HoverText { get; }
}