/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Extensions;

/// <summary>Extensions for the primitive <see cref="int"/> type.</summary>
public static class Int32Extensions
{
    /// <summary>Determines whether the <paramref name="value"/> is contained by the specified <paramref name="range"/>.</summary>
    /// <param name="value">The value to check.</param>
    /// <param name="range">A range of integers.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is greater than or equal to the <paramref name="range"/> start value and less than or equal to the end value, otherwise <see langword="false"/>.</returns>
    public static bool IsIn(this int value, Range range)
    {
        return value >= range.Start.Value && value <= range.End.Value;
    }
}
