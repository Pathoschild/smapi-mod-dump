/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Enums;

#region using directives

using NetEscapades.EnumGenerators;

#endregion using directives

/// <summary>The star quality of an <see cref="StardewValley.Object"/>.</summary>
[EnumExtensions]
public enum Quality
{
    /// <summary>Regular quality.</summary>
    Regular,

    /// <summary>Silver quality.</summary>
    Silver,

    /// <summary>Gold quality.</summary>
    Gold,

    /// <summary>Iridium quality.</summary>
    Iridium = 4,
}
