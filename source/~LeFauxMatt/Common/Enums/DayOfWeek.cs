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

/// <summary>
///     The day of the week.
/// </summary>
[EnumExtensions]
public enum DayOfWeek
{
    /// <summary>The first day of the week.</summary>
    Monday = 0,

    /// <summary>The second day of the week.</summary>
    Tuesday = 1,

    /// <summary>The third day of the week.</summary>
    Wednesday = 2,

    /// <summary>The fourth day of the week.</summary>
    Thursday = 3,

    /// <summary>The fifth day of the week.</summary>
    Friday = 4,

    /// <summary>The sixth day of the week.</summary>
    Saturday = 5,

    /// <summary>The seventh day of the week.</summary>
    Sunday = 6,
}