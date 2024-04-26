/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services;

using StardewModdingAPI.Events;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Models;

/// <summary>Service for managing cache tables.</summary>
internal sealed class CacheManager
{
    private readonly List<CacheTable> cacheTables = [];

    private int lastTicks;

    /// <summary>Initializes a new instance of the <see cref="CacheManager" /> class.</summary>
    /// <param name="eventSubscriber">The event subscriber used for subscribing to events.</param>
    public CacheManager(IEventSubscriber eventSubscriber) =>
        eventSubscriber.Subscribe<DayEndingEventArgs>(this.OnDayEnding);

    /// <summary>Retrieves a cache table of type T.</summary>
    /// <typeparam name="T">The type of objects stored in the cache table.</typeparam>
    /// <returns>The cache table of type T.</returns>
    public ICacheTable<T> GetCacheTable<T>()
    {
        var cacheTable = new CacheTable<T>();
        this.cacheTables.Add(cacheTable);
        return cacheTable;
    }

    private void OnDayEnding(DayEndingEventArgs e)
    {
        foreach (var cacheTable in this.cacheTables)
        {
            cacheTable.RemoveBefore(this.lastTicks);
        }

        this.lastTicks = Game1.ticks;
    }
}