/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Enums;

using NetEscapades.EnumGenerators;

/// <summary>Gets actions that can be performed in the expression editor.</summary>
[EnumExtensions]
internal enum ExpressionChange
{
    /// <summary>Add a new group expression.</summary>
    AddGroup,

    /// <summary>Add a new not expression.</summary>
    AddNot,

    /// <summary>Add a new expression term.</summary>
    AddTerm,

    /// <summary>Change an attribute.</summary>
    ChangeAttribute,

    /// <summary>Change a value.</summary>
    ChangeValue,

    /// <summary>Remove an expression.</summary>
    Remove,

    /// <summary>Toggle an expression group type.</summary>
    ToggleGroup,
}