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