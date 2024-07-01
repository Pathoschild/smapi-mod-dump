/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the primitive <see cref="int"/> type.</summary>
public static class Int32Extensions
{
    /// <summary>Determines whether the <paramref name="value"/> is contained by the closed set defined by the specified <paramref name="range"/>.</summary>
    /// <param name="value">The value to check.</param>
    /// <param name="range">A range of integers.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is greater than or equal to the <paramref name="range"/> start value and less than or equal to the end value, otherwise <see langword="false"/>.</returns>
    public static bool IsIn(this int value, Range range)
    {
        return value >= range.Start.Value && value <= range.End.Value;
    }

    /// <summary>Determines whether the <paramref name="value"/> contains the specified <paramref name="digit"/>.</summary>
    /// <param name="value">The value to check.</param>
    /// <param name="digit">The desired digit.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> contains <paramref name="digit"/>, otherwise <see langword="false"/>.</returns>
    public static bool ContainsDigit(this int value, int digit)
    {
        while (value != 0)
        {
            var current = value % 10;
            if (current == digit)
            {
                return true;
            }

            value /= 10;
        }

        return false;
    }

    /// <summary>Finds the first common <see cref="int"/>eger between the <paramref name="collection"/> and the specified <paramref name="candidates"/>. If none are found, returns the specified <paramref name="default"/> value.</summary>
    /// <param name="collection">A <see cref="ICollection{T}"/> of <see cref="int"/>egers.</param>
    /// <param name="candidates">The candidate <see cref="int"/>egers.</param>
    /// <param name="default">The default value in case no match is found.</param>
    /// <returns>The first common <see cref="int"/>eger between <paramref name="collection"/> and <paramref name="candidates"/>, or <paramref name="default"/> if none are found.</returns>
    public static int FirstOrDefault(this ICollection<int> collection, IEnumerable<int> candidates, int @default)
    {
        return collection.Intersect(candidates).DefaultIfEmpty(-1).First();
    }

    /// <summary>Adds the specified <paramref name="values"/> ensuring that the result does not overflow.</summary>
    /// <param name="values">The <see cref="int"/> values to be added.</param>
    /// <returns>The sum of <paramref name="values"/>, or <see cref="int.MaxValue"/> is that sum would be greater than <see cref="int.MaxValue"/>.</returns>
    public static int AddWithoutOverflow(params int[] values)
    {
        long sum = 0;
        for (var i = 0; i < values.Length && (sum += values[i]) <= int.MaxValue; i++) ;
        return sum > int.MaxValue ? int.MaxValue : (int)sum;
    }
}
