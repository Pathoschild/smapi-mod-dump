/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/Cobalt
**
*************************************************/

using System;
using StardewModdingAPI;

namespace Cobalt.Framework
{
    internal class Log
    {
        public static void trace(String str)
        {
            ModEntry.instance.Monitor.Log(str, LogLevel.Trace);
        }

        public static void debug(String str)
        {
            ModEntry.instance.Monitor.Log(str, LogLevel.Debug);
        }

        public static void info(String str)
        {
            ModEntry.instance.Monitor.Log(str, LogLevel.Info);
        }

        public static void warn(String str)
        {
            ModEntry.instance.Monitor.Log(str, LogLevel.Warn);
        }

        public static void error(String str)
        {
            ModEntry.instance.Monitor.Log(str, LogLevel.Error);
        }
    }
}
