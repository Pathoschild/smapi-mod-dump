/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

namespace ItemExtensions.Models.Enums;

public enum Modifier
{
    Set, // null or =
    Sum, // +
    Substract, // -
    Divide, // : or / or \
    Multiply, // * or x
    Percentage // %
}