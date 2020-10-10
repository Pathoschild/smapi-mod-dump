/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cilekli-link/SDVMods
**
*************************************************/

using StardewModdingAPI;
public struct Logger
{
    public static IMonitor monitor;
    public static void Log(string message)
    {
        monitor.Log(message);
    }
    public static void LogError(string message)
    {
        monitor.Log(message, LogLevel.Error);
    }
}