/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.SpritePatcher.Framework.Enums;

using NetEscapades.EnumGenerators;

/// <summary>Represents a direction relative to an object.</summary>
[EnumExtensions]
public enum Direction
{
    /// <summary>Above the object.</summary>
    Up,

    /// <summary>Below the object.</summary>
    Down,

    /// <summary>Left of the object.</summary>
    Left,

    /// <summary>Right of the object.</summary>
    Right,
}