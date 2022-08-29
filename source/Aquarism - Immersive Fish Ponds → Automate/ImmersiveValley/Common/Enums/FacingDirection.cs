/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Common.Enums;

#region using directives

using NetEscapades.EnumGenerators;

#endregion using directives

/// <summary>The direction which a character is facing.</summary>
[EnumExtensions]
public enum FacingDirection
{
    Up,
    Right,
    Down,
    Left
}

public static partial class FacingDirectionExtensions
{
    public static FacingDirection GetOppositeDirection(this FacingDirection direction) => direction + 2 % 4;
}