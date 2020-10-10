/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/CustomCrops
**
*************************************************/

using StardewModdingAPI;
using System;

namespace CustomCrops
{
    class Log
    {
        public static void trace(String str)
        {
            Mod.instance.Monitor.Log(str, LogLevel.Trace);
        }

        public static void debug(String str)
        {
            Mod.instance.Monitor.Log(str, LogLevel.Debug);
        }

        public static void info(String str)
        {
            Mod.instance.Monitor.Log(str, LogLevel.Info);
        }

        public static void warn(String str)
        {
            Mod.instance.Monitor.Log(str, LogLevel.Warn);
        }

        public static void error(String str)
        {
            Mod.instance.Monitor.Log(str, LogLevel.Error);
        }
    }
}
