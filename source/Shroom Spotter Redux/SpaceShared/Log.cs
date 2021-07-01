/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;

namespace SpaceShared
{
    class Log
    {
        public static IMonitor Monitor;

        public static void verbose(String str)
        {
            Monitor.VerboseLog(str);
        }

        public static void trace(String str)
        {
            Monitor.Log(str, LogLevel.Trace);
        }

        public static void debug(String str)
        {
            Monitor.Log(str, LogLevel.Debug);
        }

        public static void info(String str)
        {
            Monitor.Log(str, LogLevel.Info);
        }

        public static void warn(String str)
        {
            Monitor.Log(str, LogLevel.Warn);
        }

        public static void error(String str)
        {
            Monitor.Log(str, LogLevel.Error);
        }
    }
}
