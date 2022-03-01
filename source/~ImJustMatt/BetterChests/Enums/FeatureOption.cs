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
///     Indicates if a feature is enabled, disabled, or will inherit from a parent config.
/// </summary>
internal enum FeatureOption
{
    /// <summary>Feature is disabled.</summary>
    Disabled = 0,

    /// <summary>Feature inherits from a parent config.</summary>
    Default = 1,

    /// <summary>Feature is enabled.</summary>
    Enabled = 2,
}