/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Comparers;

#region using directives

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#endregion using directives

/// <summary>A comparer which considers two references equal if they point to the same instance.</summary>
/// <typeparam name="T">The value type.</typeparam>
/// <remarks>Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.</remarks>
internal class ObjectReferenceComparer<T> : IEqualityComparer<T>
    where T : class
{
    /// <summary>Determines whether the specified objects are equal.</summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns><see langword="true"/> if the specified objects are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(T? x, T? y)
    {
        return object.ReferenceEquals(x, y);
    }

    /// <summary>Gets a hash code for the specified <paramref name="object"/>.</summary>
    /// <param name="object">The value.</param>
    /// <returns>A hash code for <paramref name="object"/>.</returns>
    public int GetHashCode(T @object)
    {
        return RuntimeHelpers.GetHashCode(@object);
    }
}
