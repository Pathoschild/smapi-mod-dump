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
///     Determines the in-game config menu.
/// </summary>
[EnumExtensions]
public enum InGameMenu
{
    /// <summary>Inherit option from parent.</summary>
    Default = 0,

    /// <summary>Only the Categorize menu will be available.</summary>
    Categorize = 1,

    /// <summary>Only show ChestLabel, Categorize, and Priority.</summary>
    Simple = 2,

    /// <summary>Show all options.</summary>
    Full = 3,

    /// <summary>Show all options and replaces some options with open fields..</summary>
    Advanced = 4,
}