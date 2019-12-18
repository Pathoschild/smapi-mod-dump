using DiscordRPC.Logging;
using StardewModdingAPI;
using System;
using RPCLogLevel = DiscordRPC.Logging.LogLevel;
using SDVLogLevel = StardewModdingAPI.LogLevel;

namespace SVRichPresence {
	public class MonitorLogger : ILogger {
		public RPCLogLevel Level { get; set; }
		private readonly IMonitor Monitor;

		public MonitorLogger(IMonitor monitor) {
			Monitor = monitor;
		}

		public void Trace(string message, params object[] args) {
			Log(message, args, SDVLogLevel.Trace);
		}

		public void Info(string message, params object[] args) {
			Log(message, args, SDVLogLevel.Info);
		}

		public void Warning(string message, params object[] args) {
			Log(message, args, SDVLogLevel.Warn);
		}

		public void Error(string message, params object[] args) {
			Log(message, args, SDVLogLevel.Error);
		}

		private void Log(string message, object[] args, SDVLogLevel level) {
			if (Monitor.IsVerbose)
				Monitor.Log("DISCORD: " + String.Format(message, args), level);
		}
	}
}
