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

/// <summary>Specifies how animation will be handled.</summary>
[EnumExtensions]
public enum Animate
{
    /// <summary>No animation will be used.</summary>
    None = 1,

    /// <summary>Frames will be animated every 5 ticks.</summary>
    Fast = 5,

    /// <summary>Frames will be animated every 10 ticks.</summary>
    Medium = 10,

    /// <summary>Frames will be animated every 20 ticks.</summary>
    Slow = 20,
}