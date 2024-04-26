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
using System.Runtime.Serialization;

namespace ProductionStats;

internal class InventoryTracker
{
    public List<TrackedItem> TrackedItems = [];
    private IDateProvider _dateProvider;

    public InventoryTracker(IDateProvider dateProvider, SDate start)
    {
        Start = start;
        _dateProvider = dateProvider;
    }

    public SDate Start { get; }
    public SDate Today => _dateProvider.Now;

    /// <summary>
    /// Adds items tracker with today's (<see cref="StardewDate"/>) date.
    /// </summary>
    /// <param name="item">Name of the item</param>
    /// <param name="count">Number of items</param>
    public void Add(Item item, int count)
    {
        Add(item, count, Today);
    }

    /// <summary>
    /// Adds a new tracked item to the collection.
    /// </summary>
    /// <param name="item">The name of the item to add.</param>
    /// <param name="count">The count of the item being added.</param>
    /// <param name="date">The date associated with the addition of the item.</param>
    public void Add(Item item, int count, SDate date)
    {
        TrackedItem tracked = new(item, count, date);
        TrackedItems.Add(tracked);
    }

    /// <summary>
    /// Retrieves the items produced on a specific date.
    /// </summary>
    /// <param name="date">The date for which to retrieve produced items.</param>
    /// <returns>
    /// An IEnumerable containing item names and their 
    /// corresponding counts produced on the specified date.
    /// </returns>
    public IEnumerable<ItemStock> Produced(SDate date)
    {
        // Filter the tracked items to get those produced on the specified date,
        // then project them into item names and their counts.
        return TrackedItems
            .Where(item => item.Date == date)
            .GroupBy(item => item.Item, new ItemEqualityComparer())
            .Select(group => new ItemStock(group.Key) { Count = group.Sum(item => item.Count) })
            .Where(result => result.Count > 0);
    }

    /// <summary>
    /// Retrieves the items produced today.
    /// </summary>
    /// <returns>
    /// An IEnumerable containing item names and their 
    /// corresponding counts produced today.
    /// </returns>
    public IEnumerable<ItemStock> ProducedToday()
        => Produced(Today);

    /// <summary>
    /// Retrieves the items produced yesterday.
    /// </summary>
    /// <returns>
    /// An IEnumerable containing item names and their
    /// corresponding counts produced yesterday.</returns>
    public IEnumerable<ItemStock> ProducedYesterday()
        => Produced(Today.AddDays(-1));

    /// <summary>
    /// Retrieves the items produced between two specified dates.
    /// </summary>
    /// <param name="start">The start date of the period.</param>
    /// <param name="end">The end date of the period.</param>
    /// <returns>
    /// An IEnumerable containing item names and their 
    /// corresponding counts produced within the specified date range.
    /// </returns>
    public IEnumerable<ItemStock> ProducedInBetween(SDate start, SDate end)
    {
        // Filter the tracked items to get those produced between the start and end dates,
        // then project them into item names and their counts.
        return TrackedItems
            .Where(item => item.Date.IsBetween(start, end))
            .Select(result => (result.Item, result.Count))
            .GroupBy(item => item.Item, new ItemEqualityComparer())
            .Select(group => new ItemStock(group.Key) { Count = group.Sum(item => item.Count) })
            .Where(result => result.Count > 0);
    }

    /// <summary>
    /// Retrieves the items produced during the current week.
    /// </summary>
    /// <returns>
    /// An IEnumerable containing item names and their
    /// corresponding counts produced during the current week.
    /// </returns>
    public IEnumerable<ItemStock> ProducedThisWeek()
    {
        // Get the start and end dates of the current week.
        SDate start = Today.FirstWeekday();
        SDate end = Today.LastWeekday();

        // Retrieve the items produced between the start and end dates of the current week.
        return ProducedInBetween(start, end);
    }

    /// <summary>
    /// Retrieves the items produced during the current season.
    /// </summary>
    /// <returns>
    /// An IEnumerable containing item names and their 
    /// corresponding counts produced during the current season.
    /// </returns>
    public IEnumerable<ItemStock> ProducedThisSeason()
    {
        // Get the start and end dates of the current season.
        SDate start = new(1, Today.Season, Today.Year);
        SDate end = new(28, Today.Season, Today.Year);

        // Retrieve the items produced between the start and end dates of the current season.
        return ProducedInBetween(start, end);
    }

    /// <summary>
    /// Retrieves the items produced during the current year.
    /// </summary>
    /// <returns>
    /// An IEnumerable containing item names and their 
    /// corresponding counts produced during the current year.
    /// </returns>
    public IEnumerable<ItemStock> ProducedThisYear()
    {
        // Get the start and end dates of the current year.
        SDate start = new(1, Season.Spring, Today.Year);
        SDate end = new(28, Season.Winter, Today.Year);

        // Retrieve the items produced between the start and end dates of the current year.
        return ProducedInBetween(start, end);
    }

    /// <param name="context">The deserialization context.</param>
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
        _dateProvider ??= new InGameTimeProvider();
    }
}
