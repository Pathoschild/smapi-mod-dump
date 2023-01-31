/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using MusicMaster.Extensions;
using MusicMaster.Extensions.Reflection;
using StardewModdingAPI;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace MusicMaster;

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
		var originalColor = Console.ForegroundColor;
		Console.ForegroundColor = level.GetColor();
		try {
			DebugWriteStr(str, level);
		}
		finally {
			Console.ForegroundColor = originalColor;
		}
	}

	private static IMonitor? GetTemporaryMonitor() {
		object? sCoreInstance = null;

		if (ReflectionExt.GetTypeExt("StardewModdingAPI.Framework.SCore")?.GetStaticVariable("Instance") is not {} instanceInfo) {
			return null;
		}
		sCoreInstance = instanceInfo.GetValue(null);

		if (ReflectionExt.GetTypeExt("StardewModdingAPI.Framework.Logging.LogManager") is not {} logManagerType) {
			return null;
		}

		if (logManagerType.GetMethod(
			"GetMonitor",
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy,
			null,
			new [] {typeof(string)},
			null
		) is not { } getMonitorInfo) {
			return null;
		}

		if (sCoreInstance is null || ReflectionExt.GetTypeExt("StardewModdingAPI.Framework.SCore")?.GetInstanceVariable("LogManager") is not {} logManagerInfo) {
			return null;
		}

		if (logManagerInfo.GetValue(sCoreInstance) is not {} logManager) {
			return null;
		};

		try {
			return getMonitorInfo.Invoke(logManagerInfo, new object[] { "MusicMaster" }) as IMonitor;
		}
		catch {
			return null;
		}
	}

	private static volatile IMonitor? TemporaryMonitor = null;
	//[DebuggerStepThrough, DebuggerHidden]
	private static void DebugWriteStr(string str, LogLevel level) {
		if (str.Contains("\n\n")) {
			var builder = new StringBuilder();

			builder.EnsureCapacity(str.Length);

			char lastChar = '\0';
			foreach (var c in str)
			{
				if (c == '\n' && lastChar == '\n') {
					continue;
				}

				lastChar = c;
				builder.Append(c);
			}

			str = builder.ToString();
		}

		lock (IoLock) {
			if (MusicMaster.Self.Monitor is not {} monitor) {
				if (TemporaryMonitor is not { } tempMonitor) {
					tempMonitor = GetTemporaryMonitor();
				}

				monitor = tempMonitor;
			}
			else {
				TemporaryMonitor = null;
			}

			try {
				if (monitor is not null) {
					monitor.Log(str, level);
					return;
				}
			}
			catch {
				// Swallow Exceptions
			}
			Console.WriteLine(str);
		}

	}
}
