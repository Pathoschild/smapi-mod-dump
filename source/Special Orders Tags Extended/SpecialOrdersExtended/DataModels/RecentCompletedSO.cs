/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/SpecialOrdersExtended
**
*************************************************/

using System.Text;

namespace SpecialOrdersExtended.DataModels;

/// <summary>
/// Internal data model that keeps track of recently completed Special Orders.
/// </summary>
internal class RecentCompletedSO : AbstractDataModel
{
    /// <summary>
    /// constant identifier in filename.
    /// </summary>
    private const string IDENTIFIER = "_SOmemory";

    /// <summary>
    /// Initializes a new instance of the <see cref="RecentCompletedSO"/> class.
    /// </summary>
    /// <param name="savefile">String that corresponds to the directory name for the save.</param>
    public RecentCompletedSO(string savefile)
        : base(savefile)
    {
    }

    /// <summary>
    /// Gets or sets dictionary of Recent Order questkeys & the day they were completed.
    /// </summary>
    public Dictionary<string, uint> RecentOrdersCompleted { get; set; } = new();

    /// <summary>
    /// Loads the RecentCompletedSO data model. Generates a blank one if none exist.
    /// </summary>
    /// <returns>RecentCompletedSO data model.</returns>
    /// <exception cref="SaveNotLoadedError">Save is not loaded.</exception>
    public static RecentCompletedSO Load()
    {
        if (!Context.IsWorldReady)
        {
            throw new SaveNotLoadedError();
        }
        return ModEntry.DataHelper.ReadGlobalData<RecentCompletedSO>(Constants.SaveFolderName + IDENTIFIER)
            ?? new RecentCompletedSO(Constants.SaveFolderName);
    }

    public static RecentCompletedSO LoadTempIfAvailable()
    {
        throw new NotImplementedException();
    }

    public void SaveTemp() => base.SaveTemp(IDENTIFIER);

    /// <summary>
    /// Saves recently completed SO data model.
    /// </summary>
    public void Save() => base.Save(IDENTIFIER);

    /// <summary>
    /// Removes any quest that was completed more than seven days ago.
    /// </summary>
    /// <param name="daysPlayed">Total number of days played.</param>
    /// <returns>A list of removed keys.</returns>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Follows convention used in game.")]
    public List<string> dayUpdate(uint daysPlayed)
    {
        List<string> keysRemoved = new();
        foreach (string key in this.RecentOrdersCompleted.Keys)
        {
            if (daysPlayed > this.RecentOrdersCompleted[key] + 7)
            {
                this.RecentOrdersCompleted.Remove(key);
                keysRemoved.Add(key);
            }
        }
        return keysRemoved;
    }

    /// <summary>
    /// Tries to add a quest key to the data model.
    /// </summary>
    /// <param name="orderKey">Quest key.</param>
    /// <param name="daysPlayed">Total number of days played when quest was completed.</param>
    /// <returns>true if the quest key was successfully added, false otherwise.</returns>
    public bool TryAdd(string orderKey, uint daysPlayed) => this.RecentOrdersCompleted.TryAdd(orderKey, daysPlayed);

    /// <summary>
    /// Try to remove an order key from the recent order completed dictionary.
    /// </summary>
    /// <param name="orderKey">order to remove.</param>
    /// <returns>True if successfully removed, false otherwise.</returns>
    public bool TryRemove(string orderKey) => this.RecentOrdersCompleted.Remove(orderKey);

    [Pure]
    public bool IsWithinXDays(string orderKey, uint days)
    {
        if (this.RecentOrdersCompleted.TryGetValue(orderKey, out uint dayCompleted))
        {
            return dayCompleted + days > Game1.stats.daysPlayed;
        }
        return false;
    }

    /// <summary>
    /// Gets all keys that were set within a certain number of days.
    /// </summary>
    /// <param name="days">Number of days to look at.</param>
    /// <returns>IEnumerable of keys within the given timeframe.</returns>
    [Pure]
    public IEnumerable<string> GetKeys(uint days)
    {
        return this.RecentOrdersCompleted.Keys
            .Where(a => this.RecentOrdersCompleted[a] + days >= Game1.stats.DaysPlayed);
    }

    /// <inheritdoc/>
    [Pure]
    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine($"RecentCompletedSO{this.Savefile}");
        foreach (string key in Utilities.ContextSort(this.RecentOrdersCompleted.Keys))
        {
            stringBuilder.AppendLine($"{key} completed on Day {this.RecentOrdersCompleted[key]}");
        }
        return stringBuilder.ToString();
    }
}
