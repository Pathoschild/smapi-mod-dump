/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;

namespace StardewWebApi.Game;

public class SMAPIWrapper
{
    private SMAPIWrapper() { }

    private static SMAPIWrapper? _instance;
    public static SMAPIWrapper Instance => _instance ??= new SMAPIWrapper();

    // The very first thing we do in the mod is call Initialize, so these are never null for the life of the mod
    [NotNull] public IMonitor? Monitor { get; private set; }
    [NotNull] public IModHelper? Helper { get; private set; }

    public void Initialize(IMonitor monitor, IModHelper helper)
    {
        Monitor = monitor;
        Helper = helper;
    }

    public static void LogAlert(string message)
    {
        Instance.Monitor?.Log(message, LogLevel.Alert);
    }

    public static void LogError(string message)
    {
        Instance.Monitor?.Log(message, LogLevel.Error);
    }

    public static void LogWarn(string message)
    {
        Instance.Monitor?.Log(message, LogLevel.Warn);
    }

    public static void LogInfo(string message)
    {
        Instance.Monitor?.Log(message, LogLevel.Info);
    }

    public static void LogDebug(string message)
    {
        Instance.Monitor?.Log(message, LogLevel.Debug);
    }

    public static void LogTrace(string message)
    {
        Instance.Monitor?.Log(message, LogLevel.Trace);
    }

    public IEnumerable<IModInfo> GetAllMods()
    {
        return Helper.ModRegistry.GetAll();
    }
}