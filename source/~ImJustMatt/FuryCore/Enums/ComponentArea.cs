/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Enums;

/// <summary>
///     Designates certain components to automatically align to an area around the menu.
/// </summary>
public enum ComponentArea
{
    /// <summary>Above the ItemsToGrabMenu.</summary>
    Top = 0,

    /// <summary>To the right of the ItemsToGrabMenu.</summary>
    Right = 1,

    /// <summary>Below the ItemsToGrabMenu.</summary>
    Bottom = 2,

    /// <summary>To the left of the ItemsToGrabMenu.</summary>
    Left = 3,

    /// <summary>A Custom area.</summary>
    Custom = -1,
}