/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlameHorizon/ProductionStats
**
*************************************************/

using StardewModdingAPI.Utilities;
using StardewValley;

namespace ProductionStats;

internal record TrackedItem(Item Item, int Count, SDate Date)
{
    /// <summary>
    /// Creates instance using tuple.
    /// </summary>
    /// <param name="info">Tuple storing data.</param>
    public TrackedItem((string QualifiedItemId, int Count, SDate Date) info)
            : this(info.QualifiedItemId, 0, info.Count, info.Date)
    {
    }

    /// <summary>
    /// Creates instance using tuple. Supports v2.
    /// </summary>
    /// <param name="info">Tuple storing data.</param>
    public TrackedItem((string QualifiedItemId, int Quality, int Count, SDate Date) info)
        : this(info.QualifiedItemId, info.Quality, info.Count, info.Date)
    {
    }

    /// <summary>
    /// Creates instance using tuple.
    /// </summary>
    /// <param name="qualifiedItemId">Item id using in <see cref="ItemRegistry"/> to spawn an item.</param>
    /// <param name="count">Number of items.</param>
    /// <param name="date">When item was acquired.</param>
    public TrackedItem(string qualifiedItemId, int count, SDate date)
        : this(qualifiedItemId, 0, count, date)
    {
    }

    /// <summary>
    /// Creates instance using specified data. Supports v2.
    /// </summary>
    /// <param name="qualifiedItemId">Item id using in <see cref="ItemRegistry"/> to spawn an item.</param>
    /// <param name="count">Number of items.</param>
    /// <param name="date">When item was acquired.</param>
    public TrackedItem(string qualifiedItemId, int quality, int count, SDate date) : this(
        ItemRegistry.Create(qualifiedItemId, amount: 1, quality: quality),
        count,
        date)
    {
    }

    /// <summary>
    /// Converts <see cref="TrackedItem"/> to a form which can be serialized.
    /// </summary>
    /// <returns>Tuple representing tracked item.</returns>
    internal (string, int, SDate) ToSerializeable()
        => (Item.QualifiedItemId, Count, Date);

    /// <summary>
    /// Converts <see cref="TrackedItem"/> to a form which can be serialized. 
    /// Supports v2.
    /// </summary>
    /// <returns></returns>
    internal (string, int, int, SDate) ToSerializableV2()
        => (Item.QualifiedItemId, Item.Quality, Count, Date);

}