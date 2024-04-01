/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.BetterChests.Enums;

using NetEscapades.EnumGenerators;

/// <summary>Sorting type for items.</summary>
[EnumExtensions]
public enum SortBy
{
    /// <summary>Default sorting.</summary>
    Default = 0,

    /// <summary>Sort by type.</summary>
    Type = 1,

    /// <summary>Sort by quality.</summary>
    Quality = 2,

    /// <summary>Sort by quantity.</summary>
    Quantity = 3,
}