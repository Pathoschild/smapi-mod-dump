namespace Common
{
	using StardewModdingAPI;

	/// <summary>Methods to alter the save game data.</summary>
	internal static class Logger
	{
		private static IMonitor monitor;

		/// <summary>Initialize the logger.</summary>
		/// <param name="m">Instance to use for logging.</param>
		public static void Init(IMonitor m)
		{
			monitor = m;
		}

		/// <summary>A message indicating something went wrong.</summary>
		/// <param name="message">Text to log.</param>
		public static void Error(string message)
		{
			monitor.Log(message, LogLevel.Error);
		}

		/// <summary>An issue the player should be aware of. This should be used rarely.</summary>
		/// <param name="message">Text to log.</param>
		public static void Warn(string message)
		{
			monitor.Log(message, LogLevel.Warn);
		}

		/// <summary>Info relevant to the player. This should be used judiciously.</summary>
		/// <param name="message">Text to log.</param>
		public static void Info(string message)
		{
			monitor.Log(message, LogLevel.Info);
		}

		/// <summary>Troubleshooting info that may be relevant to the player.</summary>
		/// <param name="message">Text to log.</param>
		public static void Debug(string message)
		{
			monitor.Log(message, LogLevel.Debug);
		}

		/// <summary>Tracing info intended for developers.</summary>
		/// <param name="message">Text to log.</param>
		public static void Trace(string message)
		{
			monitor.Log(message, LogLevel.Trace);
		}
	}
}
