/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace AchtuurCore;

public static class Logger
{
    /// <summary>
    /// Calls <see cref="IMonitor.Log"/> with <see cref="LogLevel.Debug"/>, if configuration is set to Build.
    /// </summary>
    /// <param name="monitor">IMonitor instance, should be accessible by ModEntry.Instance</param>
    /// <param name="msg">Message to display</param>
    public static void DebugLog(IMonitor monitor, string msg)
    {
#if DEBUG
        monitor.Log(msg, LogLevel.Debug);
#endif
    }

    public static void TraceLog(IMonitor monitor, string msg)
    {
        monitor.Log(msg, LogLevel.Trace);
    }

    public static void ErrorLog(IMonitor monitor, string msg)
    {
        monitor.Log(msg, LogLevel.Error);
    }

    public static void WarningLog(IMonitor monitor, string msg)
    {
        monitor.Log(msg, LogLevel.Warn);
    }

    public static void DebugPrintDictionary<K, V>(IMonitor monitor, IDictionary<K, V> dict, string name = null)
    {
        DebugPrintDictionary(monitor, dict as Dictionary<K, V>, name);
    }

    public static void DebugPrintDictionary<K, V>(IMonitor monitor, Dictionary<K, V> dict, string name = null)
    {
        if (name is not null)
        {
            DebugLog(monitor, $"Printing entries of {name}");
        }
        int i = 0;

        DebugLog(monitor, $"({dict.Count})");
        DebugLog(monitor, "{");
        foreach (KeyValuePair<K, V> item in dict)
        {
            DebugLog(monitor, $"\t({i++}): {item.Key} -> {item.Value}");
        }

        DebugLog(monitor, "}");
    }

    public static void DebugPrintList<T>(IMonitor monitor, IList<T> list, string name = null)
    {
        DebugPrintList(monitor, list as List<T>, name);
    }

    public static void DebugPrintList<T>(IMonitor monitor, List<T> list, string name = null)
    {
        if (name is not null)
        {
            DebugLog(monitor, $"Printing entries of {name}");
        }
        int i = 0;

        DebugLog(monitor, $"({list.Count})");
        DebugLog(monitor, "{");
        foreach (T item in list)
        {
            DebugLog(monitor, $"\t({i++}): {item}");
        }

        DebugLog(monitor, "}");
    }
}
