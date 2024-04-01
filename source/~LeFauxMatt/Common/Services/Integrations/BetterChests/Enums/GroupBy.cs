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

/// <summary>Grouping type for items.</summary>
[EnumExtensions]
public enum GroupBy
{
    /// <summary>Default grouping.</summary>
    Default = 0,

    /// <summary>Group by category.</summary>
    Category = 1,

    /// <summary>Group by color.</summary>
    Color = 2,

    /// <summary>Group by name.</summary>
    Name = 3,
}