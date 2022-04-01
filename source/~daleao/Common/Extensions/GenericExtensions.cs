/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Common.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for generic objects.</summary>
public static class GenericExtensions
{
    /// <summary>Determine if the instance is equivalent to any of the objects in a sequence.</summary>
    /// <param name="candidates">A sequence of <typeparamref name="T" /> objects.</param>
    public static bool IsAnyOf<T>(this T item, params T[] candidates)
    {
        return candidates.Contains(item);
    }

    /// <summary>Determine if the instance is equivalent to any of the objects in a sequence.</summary>
    /// <param name="candidates">A sequence of <typeparamref name="T" /> objects.</param>
    public static bool IsAnyOf<T>(this T item, IEnumerable<T> candidates)
    {
        return candidates.Contains(item);
    }

    /// <summary>Enumerate this and specified items.</summary>
    /// <param name="otherItems">Other items to add to the collection.</param>
    public static IEnumerable<T> Collect<T>(this T item, params T[] otherItems)
    {
        yield return item;
        foreach (var otherItem in otherItems) yield return otherItem;
    }
}