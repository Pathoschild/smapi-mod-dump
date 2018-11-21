using StardewModdingAPI;
using System;

namespace CustomizableCartRedux
{
    class Log
    {
        public static void trace(String str)
        {
            CustomizableCartRedux.instance.Monitor.Log(str, LogLevel.Trace);
        }

        public static void debug(String str)
        {
            CustomizableCartRedux.instance.Monitor.Log(str, LogLevel.Debug);
        }

        public static void info(String str)
        {
            CustomizableCartRedux.instance.Monitor.Log(str, LogLevel.Info);
        }

        public static void warn(String str)
        {
            CustomizableCartRedux.instance.Monitor.Log(str, LogLevel.Warn);
        }

        public static void error(String str)
        {
            CustomizableCartRedux.instance.Monitor.Log(str, LogLevel.Error);
        }
    }
}