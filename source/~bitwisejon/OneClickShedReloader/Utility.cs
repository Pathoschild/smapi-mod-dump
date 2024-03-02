/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bitwisejon/StardewValleyMods
**
*************************************************/

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

        public static void LogImportant(string msg, LogLevel level = LogLevel.Debug)
        {
            _monitor.Log(msg, level);
        }
    }
}
