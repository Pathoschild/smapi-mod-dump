/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using ItemExtensions.Models.Items;
using StardewModdingAPI;
using StardewValley.Internal;
using StardewValley;
using xTile.Dimensions;

namespace ItemExtensions.Additions;

public static class Sorter
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    
    internal static int GetMaxFeatures(int level)
    {
        if (level % 20 == 0)
            return 0;

        if (level == 77377)
            return 15;

        var remainder = level % 30;
        return remainder;
    }
    /// <summary>
    /// Grabs all ores that match a random double. (E.g, all with a chance bigger than 0.x, starting from smallest)
    /// </summary>
    /// <param name="randomDouble"></param>
    /// <param name="canApply"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    internal static IList<string> GetAllForThisDouble(double randomDouble, Dictionary<string, double> canApply)
    {
        try
        {
            var validEntries = new Dictionary<string, double>();
            foreach (var (id, chance) in canApply)
            {
                //e.g. if randomDouble is 0.56 and this ore's chance is 0.3, it'll be skipped
                if (randomDouble > chance)
                    continue;
                validEntries.Add(id, chance);
            }

            if (validEntries.Any() == false)
                return ArraySegment<string>.Empty;

            //turns sorted to list. we do this instead of calculating directly because IOrdered has no indexOf, and I'm too exhausted to think of something better (perhaps optimize in the future)
            var convertedSorted = GetAscending(validEntries);

            var result = new List<string>();
            for (var i = 0; i < convertedSorted.Count; i++)
            {
                result.Add(convertedSorted[i]);
                if (i + 1 >= convertedSorted.Count)
                    break;
                
                var current = convertedSorted[i];
                var next = convertedSorted[i + 1];
#if DEBUG
                Log($"Added node with {convertedSorted[i]} chance to list.");
#endif

                //if next one has higher %
                //because doubles are always a little off, we do a comparison of difference
                if (Math.Abs(validEntries[next] - validEntries[current]) > 0.0000001)
                {
                    break;
                }
            }

            return result;
        }
        catch (Exception e)
        {
            Log($"Error while sorting spawn chances: {e}.\n  Will be skipped.", LogLevel.Warn);
            return new List<string>();
        }
    }
    
    /// <summary>
    /// Sorts by less chance to bigger.
    /// </summary>
    /// <param name="data">Data to sort.</param>
    /// <returns>A list with only the IDs.</returns>
    private static List<string> GetAscending(Dictionary<string, double> data)
    {
        //sorts by smallest to biggest
        var sorted = from entry in data orderby entry.Value select entry;
        
        var result = new List<string>();
        foreach (var pair in sorted)
        {
            result.Add(pair.Key);
#if DEBUG
            Log($"Added {pair.Key} to sorted list ({pair.Value})");
#endif
        }

        return result;
    }

    internal static bool GetItem(ExtraSpawn data, ItemQueryContext context, out Item item)
    {
        item = null;
        
        try
        {
            if (data.Chance < Game1.random.NextDouble())
                return false;

            if (string.IsNullOrWhiteSpace(data.Condition) && GameStateQuery.CheckConditions(data.Condition, context.Location, context.Player) == false)
                return false;

            var solvedQuery = ItemQueryResolver.TryResolve(data.ItemId, context, data.Filter, data.PerItemCondition, avoidRepeat: data.AvoidRepeat);

            var chosenItem = solvedQuery.FirstOrDefault()?.Item;

            if (string.IsNullOrWhiteSpace(chosenItem.QualifiedItemId))
                return false;

            item = ItemRegistry.Create(chosenItem.QualifiedItemId, chosenItem.Stack, data.Quality);
            return true;
        }
        catch(Exception ex)
        {
            Log($"Exception while sorting item query: {ex}.", LogLevel.Warn);
            return false;
        }
    }
}