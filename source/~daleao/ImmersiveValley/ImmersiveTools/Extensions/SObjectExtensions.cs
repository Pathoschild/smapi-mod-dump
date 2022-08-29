/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Tools.Extensions;

#region using directives

using StardewValley.Objects;

#endregion using directives

public static class SObjectExtensions
{
    /// <summary>Get whether a given object is a twig.</summary>
    public static bool IsTwig(this SObject obj) =>
        obj?.ParentSheetIndex is 294 or 295;

    /// <summary>Get whether a given object is a stone.</summary>
    public static bool IsStone(this SObject obj) =>
        obj?.Name == "Stone";

    /// <summary>Get whether a given object is a weed.</summary>
    public static bool IsWeed(this SObject obj) =>
        obj is not Chest && obj?.Name == "Weeds";
}