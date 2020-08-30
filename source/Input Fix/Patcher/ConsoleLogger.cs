using System;

namespace Patcher
{
    public class ConsoleLogger
    {
        private LogLevel maxLevel;
        private LangHelper helper;

        internal ConsoleLogger(LangHelper helper, LogLevel maxLevel = LogLevel.Trace)
        {
            this.maxLevel = maxLevel;
            this.helper = helper;
        }

        internal void LogTrans(string resxPath, LogLevel level = LogLevel.Info, params string[] args)
        {
            Log(helper.GetString(resxPath, args), level);
        }

        internal void Log(string text, LogLevel level = LogLevel.Info, params string[] args)
        {
            if (maxLevel < level) return;
            text = string.Format(text, args);
            switch (level)
            {
                case LogLevel.Info:
                    LogInfo(text);
                    break;

                case LogLevel.Warn:
                    LogWarn(text);
                    break;

                case LogLevel.Error:
                    LogError(text);
                    break;

                case LogLevel.Trace:
                    LogTrace(text);
                    break;

                default:
                    break;
            }
        }

        public void Pause()
        {
            Console.WriteLine(helper.GetString("L_C_PAUSE"));
            Console.ReadKey();
        }

        public void LogInfo(string text)
        {
            _Log(text, "Info", ConsoleColor.White);
        }

        public void LogWarn(string text)
        {
            _Log(text, "Warn", ConsoleColor.Yellow);
        }

        public void LogError(string text)
        {
            _Log(text, "Error", ConsoleColor.Red);
        }

        public void LogTrace(string text)
        {
            _Log(text, "Trace", ConsoleColor.White);
        }

        private void _Log(string text, string prefix, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(string.Format("[{0}] {1}", prefix, text));
            Console.ResetColor();
        }

        internal enum LogLevel
        {
            Info,
            Warn,
            Error,
            Trace
        }
    }
}