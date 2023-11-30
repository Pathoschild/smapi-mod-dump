/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using System.Reflection;
using StardewModdingAPI;

namespace BirbCore.Attributes;

public class Log
{
    private static readonly Dictionary<string, IMonitor> Monitors = new();

    internal static void Init(IMonitor monitor, Assembly caller)
    {
        string assembly = caller.FullName;
        if (Monitors.ContainsKey(assembly))
        {
            monitor.Log($"Assembly {assembly} has already initialized Log...Are there two dlls with the same assembly name?", LogLevel.Error);
            return;
        }
        Monitors.Add(assembly, monitor);
    }
    public static void Debug(string str)
    {
        Monitors[Assembly.GetCallingAssembly().FullName].Log(str, LogLevel.Debug);
    }
    public static void Trace(string str)
    {
        Monitors[Assembly.GetCallingAssembly().FullName].Log(str, LogLevel.Trace);
    }
    public static void Info(string str)
    {
        Monitors[Assembly.GetCallingAssembly().FullName].Log(str, LogLevel.Info);
    }
    public static void Warn(string str)
    {
        Monitors[Assembly.GetCallingAssembly().FullName].LogOnce(str, LogLevel.Warn);
    }
    public static void Error(string str)
    {
        Monitors[Assembly.GetCallingAssembly().FullName].LogOnce(str, LogLevel.Error);
    }
    public static void Alert(string str)
    {
        Monitors[Assembly.GetCallingAssembly().FullName].LogOnce(str, LogLevel.Alert);
    }
}
