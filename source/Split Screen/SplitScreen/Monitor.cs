/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/SplitScreen
**
*************************************************/

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
