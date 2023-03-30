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

namespace Fireworks;

// Credit to spacechase0
internal class Log
{
    public static IMonitor Monitor;

    public static void Verbose(string str) => Monitor.VerboseLog(str);

    public static void Trace(string str) => Monitor.Log(str);

    public static void Debug(string str) => Monitor.Log(str, LogLevel.Debug);

    public static void Info(string str) => Monitor.Log(str, LogLevel.Info);

    public static void Warn(string str) => Monitor.Log(str, LogLevel.Warn);

    public static void Error(string str) => Monitor.Log(str, LogLevel.Error);
}
