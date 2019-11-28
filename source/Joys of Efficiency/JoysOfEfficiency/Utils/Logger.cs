using StardewModdingAPI;

namespace JoysOfEfficiency.Utils
{
    internal class Logger
    {
        private static IMonitor Monitor;
        private readonly string _loggerName;

        public static void Init(IMod mod)
        {
            Monitor = mod.Monitor;
        }

        public Logger(string name)
        {
            _loggerName = name;
        }

        public void Log(string str)
        {
            Log(str, LogLevel.Trace);
        }

        public void Error(string str)
        {
            Log(str, LogLevel.Error);
        }

        public void Debug(string str)
        {
            Log(str, LogLevel.Debug);
        }

        public void Info(string str)
        {
            Log(str, LogLevel.Info);
        }

        public void Alert(string str)
        {
            Log(str, LogLevel.Alert);
        }

        public void Warn(string str)
        {
            Log(str, LogLevel.Warn);
        }
        private void Log(string str, LogLevel level)
        {
            Monitor.Log($"[{_loggerName}] {str}", level);
        }
    }
}
