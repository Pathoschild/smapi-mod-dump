/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Enums;

using NetEscapades.EnumGenerators;

/// <summary>
///     Align a component to an area around the menu.
/// </summary>
[EnumExtensions]
public enum ComponentArea
{
    /// <summary>Above the menu.</summary>
    Top = 0,

    /// <summary>To the right of the menu.</summary>
    Right = 1,

    /// <summary>Below the menu.</summary>
    Bottom = 2,

    /// <summary>To the left of the menu.</summary>
    Left = 3,

    /// <summary>A Custom area.</summary>
    Custom = -1,
}