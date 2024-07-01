/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace weizinai.StardewValleyMod.Common.Log;

internal static class Log
{
    private static IMonitor monitor = null!;

    public static void Init(IMonitor _monitor)
    {
        monitor = _monitor;
    }

    public static void Trace(string message)
    {
        monitor.Log(message);
    }

    public static void Debug(string message)
    {
        monitor.Log(message, LogLevel.Debug);
    }

    public static void Info(string message)
    {
        monitor.Log(message, LogLevel.Info);
    }

    public static void Warn(string message)
    {
        monitor.Log(message, LogLevel.Warn);
    }

    public static void Error(string message)
    {
        monitor.Log(message, LogLevel.Error);
    }

    public static void Alert(string message)
    {
        monitor.Log(message, LogLevel.Alert);
    }
}