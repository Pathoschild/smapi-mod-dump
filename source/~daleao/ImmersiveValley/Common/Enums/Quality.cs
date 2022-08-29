/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Common.Enums;

#region using directives

using NetEscapades.EnumGenerators;

#endregion using directives

/// <summary>The star quality of an <see cref="StardewValley.Object"/>.</summary>
[EnumExtensions]
internal enum Quality
{
    Regular,
    Silver,
    Gold,
    Iridium = 4
}