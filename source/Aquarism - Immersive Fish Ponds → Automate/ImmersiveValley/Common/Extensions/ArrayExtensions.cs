/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Common.Extensions;

#region using directives

using System.Linq;

#endregion using directives

/// <summary>Extensions for generic arrays of objects.</summary>
public static class ArrayExtensions
{
    /// <summary>Get a sub-array from the instance.</summary>
    /// <param name="offset">The starting index.</param>
    /// <param name="length">The length of the sub-array.</param>
    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        return array.Skip(offset).Take(length).ToArray();
    }
}