/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Enums;

using NetEscapades.EnumGenerators;

/// <summary>
///     Quality levels for an item.
/// </summary>
[EnumExtensions]
public enum Quality
{
    /// <summary>None quality items.</summary>
    None = 0,

    /// <summary>Silver quality items.</summary>
    Silver = 1,

    /// <summary>Gold quality items.</summary>
    Gold = 2,

    /// <summary>Iridium quality items.</summary>
    Iridium = 4,
}