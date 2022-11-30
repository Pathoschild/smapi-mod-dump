/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace ConfigurableSpecialOrdersUnlock;

// Credit to spacechase0
internal class Log
{
    public static IMonitor Monitor;

    public static void Verbose(object obj) => Monitor.VerboseLog(obj.ToString()!);

    public static void Trace(object obj) => Monitor.Log(obj.ToString()!);

    public static void Debug(object obj) => Monitor.Log(obj.ToString()!, LogLevel.Debug);

    public static void Info(object obj) => Monitor.Log(obj.ToString()!, LogLevel.Info);

    public static void Warn(object obj) => Monitor.Log(obj.ToString()!, LogLevel.Warn);

    public static void Error(object obj) => Monitor.Log(obj.ToString()!, LogLevel.Error);
}
