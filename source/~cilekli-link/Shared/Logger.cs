using StardewModdingAPI;
public struct Logger
{
    static IMonitor monitor;
    public static void Log(string message)
    {
        monitor.Log(message);
    }
}