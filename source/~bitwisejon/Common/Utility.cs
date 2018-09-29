using StardewModdingAPI;
using System.Diagnostics;

namespace BitwiseJonMods.Common
{
    public static class Utility
    {
        private static IMonitor _monitor;

        public static void InitLogging(IMonitor monitor)
        {
            _monitor = monitor;
        }

        [ConditionalAttribute("DEBUG")]
        public static void Log(string msg, LogLevel level = LogLevel.Debug)
        {
            _monitor.Log(msg, level);
        }
    }
}
