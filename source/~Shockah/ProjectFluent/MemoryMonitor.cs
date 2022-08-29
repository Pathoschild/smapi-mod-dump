/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace Shockah.ProjectFluent
{
	internal class MemoryMonitor : IMonitor
	{
		public bool IsVerbose
			=> true;

		private List<(string message, LogLevel? level, bool once, bool verbose)> Logs { get; set; } = new();

		public void Log(string message, LogLevel level = LogLevel.Trace)
			=> Logs.Add((message, level, once: false, verbose: false));

		public void LogOnce(string message, LogLevel level = LogLevel.Trace)
			=> Logs.Add((message, level, once: true, verbose: false));

		public void VerboseLog(string message)
			=> Logs.Add((message, level: null, once: false, verbose: true));

		public void Clear()
			=> Logs.Clear();

		public void FlushToMonitor(IMonitor monitor, bool clear = true)
		{
			foreach (var (message, level, once, verbose) in Logs)
			{
				if (verbose)
					monitor.VerboseLog(message);
				else if (once)
					monitor.LogOnce(message, level!.Value);
				else
					monitor.Log(message, level!.Value);
			}
			if (clear)
				Clear();
		}
	}
}