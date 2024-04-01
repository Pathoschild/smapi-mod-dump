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

/// <summary>The amount of debugging information that will be logged to the console.</summary>
[EnumExtensions]
public enum SimpleLogLevel
{
    /// <summary>No debugging information will be logged to the console.</summary>
    None = 0,

    /// <summary>Less debugging information will be logged to the console.</summary>
    Less = 1,

    /// <summary>More debugging information will be logged to the console.</summary>
    More = 2,
}