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

using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for generic objects.</summary>
public static class GenericExtensions
{
    /// <summary>Determine if the instance is contained by the collection.</summary>
    /// <param name="items"><typeparamref name="T" /> objects to check.</param>
    public static bool IsIn<T>(this T item, params T[] items) =>
        items.Contains(item);

    /// <summary>Determine if the instance is contained by the collection.</summary>
    /// <param name="collection">A sequence of <typeparamref name="T" /> objects to check.</param>
    public static bool IsIn<T>(this T item, IEnumerable<T> collection) =>
        collection.Contains(item);

    /// <summary>Enumerate this and specified items.</summary>
    /// <param name="items"><typeparamref name="T"/> items to add to the collection.</param>
    public static IEnumerable<T> Collect<T>(this T item, params T[] items)
    {
        yield return item;
        foreach (var next in items) yield return next;
    }

    /// <summary>Enumerate this and specified <see cref="IEnumerable{T}"/>.</summary>
    /// <param name="collection">A sequence of <typeparamref name="T"/> items to concatenate to the collection.</param>
    public static IEnumerable<T> Collect<T>(this T item, IEnumerable<T> collection)
    {
        yield return item;
        foreach (var next in collection) yield return next;
    }
}