/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Common.Extensions.Collections;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>Extensions for generic lists of objects.</summary>
public static class ListExtensions
{
    /// <inheritdoc cref="List{T}.AddRange"/>
    /// <param name="items">The elements to be added.</param>
    public static void AddRange<T>(this List<T> list, params T[] items)
    {
        list.AddRange(items);
    }
}