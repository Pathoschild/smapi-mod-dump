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

/// <summary>Indicates if a feature is enabled, disabled, or will inherit from a parent config.</summary>
[EnumExtensions]
public enum FeatureOption
{
    /// <summary>Option is inherited from a parent config.</summary>
    Default = 0,

    /// <summary>Feature is disabled.</summary>
    Disabled = 1,

    /// <summary>Feature is enabled.</summary>
    Enabled = 2,
}