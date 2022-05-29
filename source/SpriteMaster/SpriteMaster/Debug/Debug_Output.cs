/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using StardewModdingAPI;
using System;
using System.Diagnostics;

namespace SpriteMaster;

internal static partial class Debug {
	[DebuggerStepThrough, DebuggerHidden]
	private static ConsoleColor GetColor(this LogLevel @this) {
		return @this switch {
			LogLevel.Debug => Color.Trace,
			LogLevel.Info => Color.Info,
			LogLevel.Warn => Color.Warning,
			LogLevel.Error => Color.Error,
			LogLevel.Alert => Color.Fatal,
			_ => ConsoleColor.White,
		};
	}

	[DebuggerStepThrough, DebuggerHidden]
	private static void DebugWrite(LogLevel level, string str) {
		if (LogFile is not null) {
			try {
				var prefix = level switch {
					LogLevel.Debug => 'T',
					LogLevel.Info => 'I',
					LogLevel.Warn => 'W',
					LogLevel.Error => 'E',
					LogLevel.Alert => 'F',
					_ => '?',
				};

				LogFile.Write($"[{prefix}] {str}");
			}
			catch { /* ignore errors */ }
		}

		var originalColor = Console.ForegroundColor;
		Console.ForegroundColor = level.GetColor();
		try {
			DebugWriteStr(str, level);
		}
		finally {
			Console.ForegroundColor = originalColor;
		}
	}

	[DebuggerStepThrough, DebuggerHidden]
	private static void DebugWriteStr(string str, LogLevel level) {
		var lines = str.Lines(removeEmpty: true);
		var fullString = string.Join("\n", lines);
		lock (IOLock) {
			SpriteMaster.Self.Monitor.Log(fullString, level);
			/*
			foreach (var line in lines) {
				SpriteMaster.Self.Monitor.Log(line.TrimEnd(), level);
			}
			*/
		}

	}
}
