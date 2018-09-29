using StardewModdingAPI;

namespace SplitScreen
{
	public class Monitor
	{
		private static IMonitor monitor;
		
		public Monitor(IMonitor monitor)
		{
			SplitScreen.Monitor.monitor = monitor;
		}

		public static void Log (string message, LogLevel level = LogLevel.Debug)
		{
			monitor.Log(message, level);
		}
	}
}
