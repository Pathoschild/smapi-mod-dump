/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using StardewModdingAPI;

namespace StardewSurvivalProject.source
{
    internal class LogHelper
    {
        public static IMonitor Monitor;

        public static void Verbose(string str)
        {
            LogHelper.Monitor.VerboseLog(str);
        }

        public static void Trace(string str)
        {
            LogHelper.Monitor.Log(str, LogLevel.Trace);
        }

        public static void Debug(string str)
        {
            LogHelper.Monitor.Log(str, LogLevel.Debug);
        }

        public static void Info(string str)
        {
            LogHelper.Monitor.Log(str, LogLevel.Info);
        }

        public static void Warn(string str)
        {
            LogHelper.Monitor.Log(str, LogLevel.Warn);
        }

        public static void Error(string str)
        {
            LogHelper.Monitor.Log(str, LogLevel.Error);
        }
    }
}
