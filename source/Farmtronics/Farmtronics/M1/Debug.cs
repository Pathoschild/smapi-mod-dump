/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoeStrout/Farmtronics
**
*************************************************/

using System;
using StardewModdingAPI;

namespace Farmtronics {
	public static class Debug {

		// While debugging, feel free to change the following to
		// LogLevel.Info or even LogLevel.Trace so it all prints.
		// But before committing, please change it back to LogLevel.Error.
		const LogLevel logLevelToPrint = LogLevel.Trace;

		public static void Log(string logMsg, LogLevel logLevel=LogLevel.Info) {
			if (logLevel >= logLevelToPrint) {
				ModEntry.instance.Monitor.Log(logMsg);
			}
		}
	}

}
