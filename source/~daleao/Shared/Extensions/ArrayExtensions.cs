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

#region using directives

using System.Linq;
using DaLion.Shared.Exceptions;

#endregion using directives

/// <summary>Extensions for generic arrays of objects.</summary>
public static class ArrayExtensions
{
    /// <summary>Gets a sub-array of <paramref name="length"/> starting at <paramref name="offset"/>.</summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array.</param>
    /// <param name="offset">The starting index.</param>
    /// <param name="length">The length of the sub-array.</param>
    /// <returns>A new array formed by taking <paramref name="length"/> elements of the original after skipping <paramref name="offset"/>.</returns>
    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        if (length - offset > array.Length)
        {
            ThrowHelperExtensions.ThrowIndexOutOfRangeException();
        }

        return array.Skip(offset).Take(length).ToArray();
    }

    /// <summary>Shifts all elements of the <paramref name="array"/> one unit to the right.</summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array.</param>
    /// <remarks>The last element of the original array becomes the first element of the shifted array.</remarks>
    public static void ShiftRight<T>(this T[] array)
    {
        var temp = array[^1];
        Array.Copy(array, 0, array, 1, array.Length - 1);
        array[0] = temp;
    }

    /// <summary>Shifts all elements of the <paramref name="array"/> <paramref name="count"/> units to the right.</summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array.</param>
    /// <param name="count">The number of shifts to perform.</param>
    public static void ShiftRight<T>(this T[] array, int count)
    {
        count %= array.Length;
        if (count == 0)
        {
            return;
        }

        for (var i = 0; i < count; i++)
        {
            array.ShiftRight();
        }
    }

    /// <summary>Shifts all elements of the <paramref name="array"/> one unit to the left.</summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array.</param>
    public static void ShiftLeft<T>(this T[] array)
    {
        var temp = array[0];
        Array.Copy(array, 1, array, 0, array.Length - 1);
        array[^1] = temp;
    }

    /// <summary>Shifts all elements of the <paramref name="array"/> <paramref name="count"/> units to the left.</summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array.</param>
    /// <param name="count">The number of shifts to perform.</param>
    public static void ShiftLeft<T>(this T[] array, int count)
    {
        count %= array.Length;
        if (count == 0)
        {
            return;
        }

        for (var i = 0; i < count; i++)
        {
            array.ShiftLeft();
        }
    }

    /// <summary>Chooses a random element from the <paramref name="array"/>.</summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array.</param>
    /// <param name="r">A <see cref="Random"/> number generator.</param>
    /// <returns>A random element from the <paramref name="array"/>.</returns>
    public static T Choose<T>(this T[] array, Random? r = null)
    {
        r ??= new Random(Guid.NewGuid().GetHashCode());
        return array[r.Next(array.Length)];
    }
}
