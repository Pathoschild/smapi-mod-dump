/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System.Diagnostics;
using StardewModdingAPI;


namespace TraktoriShared.Utils
{
	/// <summary>
	/// A wrapper of the class System.Diagnostics.Stopwatch for logging the result.
	/// </summary>
	internal class StopWatchHelper
	{
		private readonly IMonitor monitor;
		private readonly Stopwatch stopwatch;
		private readonly long nanoSecondsPerTick;


		public StopWatchHelper(IMonitor monitor)
		{
			this.monitor = monitor;

			stopwatch = new Stopwatch();
			nanoSecondsPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;
		}


		/// <summary>
		/// Starts, or resumes, measuring elapsed time for an interval.
		/// </summary>
		public void Start()
		{
			stopwatch.Start();
		}


		/// <summary>
		/// Stops measuring elapsed time for an interval and prints the provided message with the elapsed time.
		/// </summary>
		/// <param name="logMessage">A message to log. If not null, will print in the format of "$logMessage took $elapsedTime μs."</param>
		public void Stop(string? logMessage = null)
		{
			stopwatch.Stop();

			if (logMessage is not null)
			{
				LogToMonitor(logMessage);
			}
		}


		/// <summary>
		/// Stops time interval measurement, resets the elapsed time to zero, and starts measuring elapsed time.
		/// </summary>
		public void Restart()
		{
			stopwatch.Restart();
		}


		/// <summary>
		/// Stops time interval measurement and resets the elapsed time to zero.
		/// </summary>
		public void Reset()
		{
			stopwatch.Reset();
		}


		/// <summary>
		/// Logs the elapsed time interval in micro seconds with the provided message.
		/// </summary>
		/// <param name="logMessage">A message to log in the format of "$logMessage took $elapsedTime μs.</param>
		private void LogToMonitor(string? logMessage = null)
		{
			if (logMessage is not null)
			{
				monitor.Log($"{logMessage} took {stopwatch.ElapsedTicks * nanoSecondsPerTick / 1000} μs", LogLevel.Debug);
			}
			else
			{
				monitor.Log($"The stopwatch interval was {stopwatch.ElapsedTicks * nanoSecondsPerTick / 1000} μs", LogLevel.Debug);
			}
		}
	}
}
