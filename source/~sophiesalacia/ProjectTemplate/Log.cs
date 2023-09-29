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
// ReSharper disable UnusedMember.Global

namespace ProjectTemplate;

// Credit to kittycatcasey for initial version. i've iterated on it a bit :3
internal class Log
{
	public static IMonitor Monitor;

	public static void Verbose(object obj) => Monitor.VerboseLog(obj.ToString() ?? string.Empty);

	// Only log Trace messages if compiled in Debug mode.
	public static void Trace(object obj)
	{
#if DEBUG
		Monitor.Log(obj.ToString() ?? string.Empty);
#endif
	}

	public static void Debug(object obj) => Monitor.Log(obj.ToString() ?? string.Empty, LogLevel.Debug);

	public static void Info(object obj) => Monitor.Log(obj.ToString() ?? string.Empty, LogLevel.Info);

	public static void Warn(object obj) => Monitor.Log(obj.ToString() ?? string.Empty, LogLevel.Warn);

	public static void Error(object obj) => Monitor.Log(obj.ToString() ?? string.Empty, LogLevel.Error);
}
