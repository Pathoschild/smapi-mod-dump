using StardewModdingAPI;

namespace StackSplitX
{
    public static class LogExtensions
    {
        public static void DebugLog(this IMonitor monitor, string message, LogLevel level = LogLevel.Trace)
        {
#if DEBUG
            monitor.Log(message, level);
#endif
        }
    }
}
