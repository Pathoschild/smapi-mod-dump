/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/siweipancc/InstantAnimals
**
*************************************************/

using StardewModdingAPI;

namespace InstantAnimals.wrapper;

public class Logger
{
    private static IMonitor? _monitor;

    private Logger()
    {
    }


    public static void Initialize(IMonitor? monitor) => _monitor = monitor;

    public static void Debug(string message)
    {
        Log(message);
    }

    public static void Info(string message)
    {
        Log(message, LogLevel.Info);
    }
    public static void Error(string message)
    {
        Log(message, LogLevel.Error);
    }

    public static void Log(string message, LogLevel logLevel = LogLevel.Debug)
    {
        _monitor?.Log(message, logLevel);
    }
}