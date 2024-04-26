/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlameHorizon/ProductionStats
**
*************************************************/

using StardewValley;
using System.Diagnostics.CodeAnalysis;

namespace ProductionStats;

/// <summary>
/// Compares items using <see cref="Item.QualifiedItemId"/> property.
/// </summary>
internal class ItemEqualityComparer : IEqualityComparer<Item>
{
    public bool Equals(Item? x, Item? y) 
        => x.QualifiedItemId.Equals(y.QualifiedItemId) 
           && x.Quality == y.Quality;

    public int GetHashCode([DisallowNull] Item obj) 
        => obj.QualifiedItemId.GetHashCode() + obj.Quality;
}