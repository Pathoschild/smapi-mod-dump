/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using StardewModdingAPI;

namespace JoysOfEfficiency.Utils
{
    public class Logger
    {
        public static IMonitor Monitor
        {
            get; private set;

        }

        public static void Init(IMonitor monitor)
        {
            Monitor = monitor;
        }

        private string Name { get; }
        public Logger(string loggerName)
        {
            Name = loggerName;
        }

        public void Log(string text, LogLevel level = LogLevel.Trace)
        {
            Monitor.Log($"[{Name}]{text}", level);
        }

        public void Error(string text)
        {
            Log(text, LogLevel.Error);
        }
    }
}
