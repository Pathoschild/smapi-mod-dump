/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/SpecialOrdersExtended
**
*************************************************/

using System.Reflection;

namespace SpecialOrdersExtended;

internal class StatsManager
{
    Dictionary<string, PropertyInfo> propertyInfos = new();

    //remove these stats, they make no sense.
    private readonly string[] denylist = { "AverageBedtime", "TimesUnconscious", "TotalMoneyGifted" };


    /// <summary>
    /// Populate the propertyInfos cache.
    /// </summary>
    public void GrabProperties()
    {
        propertyInfos = typeof(Stats).GetProperties()
            .Where((PropertyInfo p) => p.CanRead && p.PropertyType.Equals(typeof(uint)) && !denylist.Contains(p.Name))
            .ToDictionary((PropertyInfo p) => p.Name.ToLowerInvariant(), p => p);
    }

    public void ClearProperties() => propertyInfos.Clear();

    public uint GrabBasicProperty(string key, Stats stats)
    {
        if (propertyInfos.Count.Equals(0)) { GrabProperties(); }
        try
        {
            if (propertyInfos.TryGetValue(key.ToLowerInvariant(), out PropertyInfo property))
            {
                return (uint)property.GetValue(stats);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"{I18n.StatCacheFail(key: key, atra: "https://github.com/atravita-mods/SpecialOrdersExtended/issues")}\n\n{ex}", LogLevel.Error);
        }
        if (stats.stat_dictionary.TryGetValue(key, out uint result)) { return result; }
        ModEntry.ModMonitor.Log(I18n.StatNotFound(key), LogLevel.Trace);
        return 0u;
    }

    public void ConsoleListProperties(string command, string[] args)
    {
        if (propertyInfos.Count.Equals(0)) { GrabProperties(); }
        ModEntry.ModMonitor.Log($"{I18n.CurrentKeysFound()}: \n    {I18n.Hardcoded()}:{String.Join(", ", Utilities.ContextSort(propertyInfos.Keys))}\n    {I18n.Dictionary()}:{String.Join(", ", Utilities.ContextSort(Game1.player.stats.stat_dictionary.Keys))}", LogLevel.Info);
    }
}
