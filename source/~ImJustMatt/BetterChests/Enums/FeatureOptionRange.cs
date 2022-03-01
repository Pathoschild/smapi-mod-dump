/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Enums;

/// <summary>
///     Indicates at what range a feature will be enabled.
/// </summary>
internal enum FeatureOptionRange
{
    /// <summary>Feature inherits from a parent config.</summary>
    Default = 0,

    /// <summary>Feature is disabled.</summary>
    Disabled = -1,

    /// <summary>Feature is enabled for storages in the player's inventory.</summary>
    Inventory = 1,

    /// <summary>Feature is enabled for storages in the player's current location.</summary>
    Location = 2,

    /// <summary>Feature is enabled for any storage in an accessible location to the player.</summary>
    World = 3,
}