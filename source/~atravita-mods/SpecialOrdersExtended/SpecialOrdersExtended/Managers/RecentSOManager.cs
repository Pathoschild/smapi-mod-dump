/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;

using AtraShared;
using AtraShared.Utils.Extensions;

using SpecialOrdersExtended.DataModels;

namespace SpecialOrdersExtended.Managers;

/// <summary>
/// Handles all references to recently completed special orders.
/// </summary>
internal class RecentSOManager
{
    private static RecentCompletedSO? recentCompletedSO;
    private static List<string>? currentOrderCache;

    /// <summary>
    /// Load the recently completed SO log.
    /// </summary>
    internal static void Load() => recentCompletedSO = RecentCompletedSO.Load();

    /// <summary>
    /// Load the temp version of the recently completed SO log if available.
    /// </summary>
    internal static void LoadTemp()
    {
        if (RecentCompletedSO.LoadTempIfAvailable() is RecentCompletedSO log)
        {
            ModEntry.ModMonitor.Log("Temp log loaded");
            recentCompletedSO = log;
        }
    }

    /// <summary>
    /// Saves the recently completed SO log.
    /// </summary>
    /// <exception cref="SaveNotLoadedError">Save not loaded.</exception>
    internal static void Save()
    {
        if (recentCompletedSO is null)
        {
            ASThrowHelper.ThrowSaveNotLoaded();
        }
        recentCompletedSO.Save();
    }

    /// <summary>
    /// Saves the recently completed SO log to a temp file.
    /// </summary>
    /// <exception cref="SaveNotLoadedError">Save not loaded.</exception>
    internal static void SaveTemp()
    {
        if (recentCompletedSO is null)
        {
            ASThrowHelper.ThrowSaveNotLoaded();
        }
        recentCompletedSO.SaveTemp();
    }

    /// <summary>
    /// Gets all keys that were set within a certain number of days.
    /// </summary>
    /// <param name="days">current number of days played.</param>
    /// <returns>IEnumerable of keys within the given timeframe. May return null.</returns>
    internal static IEnumerable<string>? GetKeys(uint days) => recentCompletedSO?.GetKeys(days);

    /// <summary>
    /// Run at the end of a day, in order to remove older completed orders.
    /// </summary>
    /// <param name="daysplayed">current number of days played.</param>
    /// <exception cref="SaveNotLoadedError">Raised whenver the field is null and should not be. (Save not loaded).</exception>
    /// <remarks>Should remove orders more than seven days old.</remarks>
    internal static void DayUpdate(uint daysplayed)
    {
        if(recentCompletedSO is null)
        {
            ASThrowHelper.ThrowSaveNotLoaded();
            return;
        }
        List<string> keysRemoved = recentCompletedSO.dayUpdate(daysplayed);
        DialogueManager.ClearRepeated(keysRemoved);
        ModEntry.ModMonitor.LogIfVerbose(() => $"Keys removed from Recent Completed SOs: {string.Join(", ", keysRemoved)}");
    }

    /// <summary>
    /// Gets the newest recently completed orders. Runs every 10 in-game minutes
    /// Grabs both the current orders marked as complete and looks for orders dismissed.
    /// </summary>
    /// <returns>true if an order got added to RecentCompletedSO, false otherwise.</returns>
    internal static bool GrabNewRecentlyCompletedOrders()
    {
        if (!Context.IsWorldReady)
        {
            return false;
        }

        Dictionary<string, SpecialOrder>? currentOrders = Game1.player?.team?.specialOrders?.ToDictionaryIgnoreDuplicates(a => a.questKey.Value, a => a)
            ?? SaveGame.loaded?.specialOrders?.ToDictionaryIgnoreDuplicates(a => a.questKey.Value, a => a);
        if (currentOrders is null)
        { // Save is not loaded
            return false;
        }
        List<string> currentOrderKeys = currentOrders.Keys.OrderBy(a => a).ToList();

        bool updatedCache = false;

        // Check for any completed orders in the current orders.
        foreach (SpecialOrder order in currentOrders.Values)
        {
            if (order.questState.Value == SpecialOrder.QuestState.Complete)
            {
                if (TryAdd(order.questKey.Value))
                {
                    updatedCache = true;
                }
            }
        }
        if (currentOrderKeys == currentOrderCache)
        {// No one has been added or dismissed
            return updatedCache;
        }

        // Grab my completed orders
        var completedOrders = Game1.player?.team?.completedSpecialOrders;

        if (completedOrders is null)
        { // This should not happen, but just in case?
            return updatedCache;
        }

        // Check to see if any quest has been recently dismissed.
        if (currentOrderCache is not null)
        {
            foreach (string cachedOrder in currentOrderCache)
            {
                if (!currentOrders.ContainsKey(cachedOrder) && completedOrders.ContainsKey(cachedOrder))
                {// A quest previously in the current quests is gone now
                 // and seems to have appeared in the completed orders
                    if (TryAdd(cachedOrder))
                    {
                        updatedCache = true;
                    }
                }
            }
        }
        currentOrderCache = currentOrderKeys;
        return updatedCache;
    }

    /// <summary>
    /// Tries to add a QuestKey to the RecentCompletedSO data model
    /// If it's already there, does nothing.
    /// </summary>
    /// <param name="questkey">Quest key (exact).</param>
    /// <returns>True if successfully added, false otherwise.</returns>
    /// <exception cref="SaveNotLoadedError">Save is not loaded.</exception>
    internal static bool TryAdd(string questkey)
    {
        if (!Context.IsWorldReady)
        {
            ASThrowHelper.ThrowSaveNotLoaded();
            return false;
        }
        if (recentCompletedSO!.TryAdd(questkey, Game1.stats.DaysPlayed))
        {
            ModEntry.ModMonitor.DebugOnlyLog($"Added {questkey} to the recently completed SOs", LogLevel.Info);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Attempt to remove a questkey from the RecentCompletedSO.
    /// </summary>
    /// <param name="questkey">Quest key to search for.</param>
    /// <returns>True if removed successfully, false otherwise.</returns>
    /// <exception cref="SaveNotLoadedError">If called when teh save is not loaded.</exception>
    internal static bool TryRemove(string questkey)
    {
        if (!Context.IsWorldReady)
        {
            ASThrowHelper.ThrowSaveNotLoaded();
            return false;
        }
        return recentCompletedSO!.TryRemove(questkey);
    }

    /// <summary>
    /// Returns whether the specific questkey was completed in the last X days.
    /// </summary>
    /// <param name="questkey">Quest Key.</param>
    /// <param name="days">number of days to check.</param>
    /// <returns>True if questkey found and was completed in X days, false otherwise.</returns>
    /// <exception cref="SaveNotLoadedError">Save not loaded.</exception>
    /// <remarks>The data model will delete any entries older than 7 days, so beyond that it just won't know.</remarks>
    internal static bool IsWithinXDays(string questkey, uint days)
    {
        if (!Context.IsWorldReady)
        {
            ASThrowHelper.ThrowSaveNotLoaded();
        }
        return recentCompletedSO!.IsWithinXDays(questkey, days);
    }
}
