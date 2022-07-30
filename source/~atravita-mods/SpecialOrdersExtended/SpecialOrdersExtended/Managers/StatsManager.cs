/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using System.Text;
using AtraUtils = AtraShared.Utils.Utils;

namespace SpecialOrdersExtended.Managers;

/// <summary>
/// Manages the handling of stats
/// Caches the available stats at first use, but clears at the end of each day.
/// </summary>
/// <remarks>Clearing the cache should only be handled by one player in splitscreen.</remarks>
internal static class StatsManager
{
    // Remove these stats, they make no sense.
    private static readonly string[] DENYLIST = { "AverageBedtime", "TimesUnconscious", "TotalMoneyGifted" };

    /// <summary>
    /// Internal dictionary to store the getters on the Stats dictionary.
    /// </summary>
    /// <remarks>Use the getter, that will autopopulate the cache if empty.</remarks>
    private static Dictionary<string, Func<Stats, uint>> propertyGetters = new();

    /// <summary>
    /// Gets the propertyGetters dictionary.
    /// </summary>
    /// <remarks>Grabs properties if needed (if the dictionary is empty).</remarks>
    public static Dictionary<string, Func<Stats, uint>> PropertyGetters
    {
        get
        {
            if (propertyGetters.Count.Equals(0))
            {
                GrabProperties();
            }
            return propertyGetters;
        }
    }

    /// <summary>
    /// Clears the list of possible stats.
    /// </summary>
    public static void ClearProperties() => propertyGetters.Clear();

    /// <summary>
    /// Get the value of the stat in the specific stat object.
    /// Looks through both the hardcoded stats and the stats dictionary
    /// but ignores the monster dictionary.
    /// </summary>
    /// <param name="key">The key of the stat to search for.</param>
    /// <param name="stats">The stats to look through (which farmer do I want?).</param>
    /// <returns>Value of the stat.</returns>
    public static uint GrabBasicProperty(string key, Stats stats)
    {
        try
        {
            if (PropertyGetters.TryGetValue(key, out Func<Stats, uint>? property))
            {
                return property(stats);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"{I18n.StatCacheFail(key: key, atra: "https://github.com/atravita-mods/SpecialOrdersExtended/issues")}\n\n{ex}", LogLevel.Error);
        }
        if (stats.stat_dictionary.TryGetValue(key, out uint result))
        {
            return result;
        }
        ModEntry.ModMonitor.Log(I18n.StatNotFound(key), LogLevel.Trace);
        return 0u;
    }

    /// <summary>
    /// Console command to list all stats found, both hardcoded and in the stats dictionary.
    /// Note that this will include stats that other mods add as well.
    /// </summary>
    /// <param name="command">The name of the command.</param>
    /// <param name="args">Any arguments (none for this command).</param>
    [SuppressMessage("ReSharper", "IDE0060", Justification = "Format expected by console commands")]
    public static void ConsoleListProperties(string command, string[] args)
    {
        StringBuilder sb = new();
        sb.AppendLine(I18n.CurrentKeysFound());
        sb.Append('\t').Append(I18n.Hardcoded()).AppendJoin(", ", AtraUtils.ContextSort(PropertyGetters.Keys)).AppendLine();
        sb.Append('\t').Append(I18n.Dictionary()).AppendJoin(", ", AtraUtils.ContextSort(Game1.player.stats.stat_dictionary.Keys));
        ModEntry.ModMonitor.Log(sb.ToString(), LogLevel.Info);
    }

    /// <summary>
    /// Populate the propertyInfos cache.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:Closing parenthesis should be spaced correctly", Justification = "Stylecop doesn't understand nullable :(")]
    private static void GrabProperties()
    {
        propertyGetters = typeof(Stats).GetProperties()
            .Where((PropertyInfo p) => p.CanRead && p.PropertyType.Equals(typeof(uint)) && !DENYLIST.Contains(p.Name))
            .ToDictionary(
                keySelector: (PropertyInfo p) => p.Name,
                elementSelector: (PropertyInfo p) => p.GetGetMethod()!.CreateDelegate<Func<Stats, uint>>(),
                comparer: StringComparer.OrdinalIgnoreCase);
    }
}
