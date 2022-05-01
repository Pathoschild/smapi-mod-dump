/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;

namespace SmartBuilding.Utilities
{
    public class Logger
    {
        private IMonitor monitor;
        private string logPrefix = ":: ";

        public Logger(IMonitor m)
        {
            monitor = m;
        }

        private void Log(string logMessage, string logPrefix, LogLevel logLevel)
        {
            monitor.Log(logPrefix + logMessage, logLevel);
        }

        public void Log(string logMessage, LogLevel logLevel = LogLevel.Info)
        {
            this.Log(logMessage, logPrefix, logLevel);
        }

        public void Exception(Exception e)
        {
            monitor.Log($"{logPrefix} Exception: {e.Message}", LogLevel.Error);
            monitor.Log($"{logPrefix} Full exception data: \n{e.Data}", LogLevel.Error);
        }
    }
}