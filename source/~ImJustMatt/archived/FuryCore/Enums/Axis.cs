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

using NetEscapades.EnumGenerators;

/// <summary>
///     Describes an axis in a 2-dimensional coordinate system.
/// </summary>
[EnumExtensions]
public enum Axis
{
    /// <summary>An axis going from top to bottom.</summary>
    Vertical,

    /// <summary>An axis going from left to right.</summary>
    Horizontal,
}