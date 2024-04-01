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

/// <summary>The method used to select items.</summary>
[EnumExtensions]
public enum FilterMethod
{
    /// <summary>no transformation will be applied.</summary>
    Default = 0,

    /// <summary>Selected items will be sorted first.</summary>
    Sorted = 1,

    /// <summary>Gray out unselected items.</summary>
    GrayedOut = 2,

    /// <summary>Hide unselected items.</summary>
    Hidden = 3,
}