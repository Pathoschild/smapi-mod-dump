using StardewModdingAPI;
using System;

namespace EquivalentExchange
{
    //unabashedly stolen in its entirety from Cooking Skill. Full of useful logging methods I'll need since I'm cannibalizing so much from Cooking - thank you spacechase0
    class Log
    {
        public static void trace(String str)
        {
            EquivalentExchange.instance.Monitor.Log(str, LogLevel.Trace);
        }

        public static void debug(String str)
        {
            EquivalentExchange.instance.Monitor.Log(str, LogLevel.Debug);
        }

        public static void info(String str)
        {
            EquivalentExchange.instance.Monitor.Log(str, LogLevel.Info);
        }

        public static void warn(String str)
        {
            EquivalentExchange.instance.Monitor.Log(str, LogLevel.Warn);
        }

        public static void error(String str)
        {
            EquivalentExchange.instance.Monitor.Log(str, LogLevel.Error);
        }
    }
}
