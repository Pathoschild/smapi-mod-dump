/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NormanPCN/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;

namespace Helpers
{
	public class Logger
	{
		private readonly IMonitor Monitor;
		private readonly string Prefix;
		private readonly LogLevel Level;

		public Logger(IMonitor m, LogLevel level = LogLevel.Trace, string p = "")
		{
			Monitor = m;
			Prefix = p;
			Level = level;
		}

		private void LogIt(string logMessage, LogLevel logLevel)
		{
			Monitor.Log(Prefix + logMessage, logLevel);
		}

		public void LogOnce(string logMessage, LogLevel logLevel = LogLevel.Info)
		{
			Monitor.LogOnce(Prefix + logMessage, logLevel);
		}

		public void Log(string logMessage)
		{
			this.LogIt(logMessage, Level);
		}

		public void Info(string logMessage)
		{
			this.LogIt(logMessage, LogLevel.Info);
		}

		public void Debug(string logMessage)
		{
			this.LogIt(logMessage, LogLevel.Debug);
		}

		public void Trace(string logMessage)
		{
			this.LogIt(logMessage, LogLevel.Trace);
		}

		public void Warn(string logMessage)
		{
			this.LogIt(logMessage, LogLevel.Warn);
		}

		public void Error(string logMessage)
		{
			this.LogIt(logMessage, LogLevel.Error);
		}

		public void Exception(Exception e)
		{
			Monitor.Log($"{Prefix} Exception: {e.Message}", LogLevel.Error);
			Monitor.Log($"{Prefix} Full exception data: \n{e.Data}", LogLevel.Error);
		}
	}
}