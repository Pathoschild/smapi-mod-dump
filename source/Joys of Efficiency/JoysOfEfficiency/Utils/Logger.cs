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
