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

namespace DefaultWindowSize.Utilities
{
    public class Logger
    {
        private readonly string _logPrefix = ":: ";
        private readonly IMonitor _monitor;

        public Logger(IMonitor m)
        {
            this._monitor = m;
        }

        private void Log(string logMessage, string logPrefix, LogLevel logLevel)
        {
            this._monitor.Log(logPrefix + logMessage, logLevel);
        }

        public void Log(string logMessage, LogLevel logLevel = LogLevel.Info)
        {
            this.Log(logMessage, this._logPrefix, logLevel);
        }

        public void Trace(string logMessage)
        {
            this.Log(logMessage, this._logPrefix, LogLevel.Warn);
        }

        public void Warn(string logMessage)
        {
            this.Log(logMessage, this._logPrefix, LogLevel.Warn);
        }

        public void Error(string logMessage)
        {
            this.Log(logMessage, this._logPrefix, LogLevel.Error);
        }

        public void Exception(Exception e)
        {
            this._monitor.Log($"{this._logPrefix} Exception: {e.Message}", LogLevel.Error);
            this._monitor.Log($"{this._logPrefix} Full exception data: \n{e.Data}", LogLevel.Error);
        }
    }
}
