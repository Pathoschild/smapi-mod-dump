/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using static SpriteMaster.Runtime;

namespace SpriteMaster;

internal static partial class Debug {
	private static readonly string ModuleName = typeof(Debug).Namespace!;

	private static class Color {
		internal const ConsoleColor Trace = ConsoleColor.Gray;
		internal const ConsoleColor Info = ConsoleColor.White;
		internal const ConsoleColor Warning = ConsoleColor.Yellow;
		internal const ConsoleColor Error = ConsoleColor.Red;
		internal const ConsoleColor Fatal = ConsoleColor.Red;
	}

	private static readonly string LocalLogPath = Path.Combine(Config.LocalRoot, $"{ModuleName}.log");
	private static readonly StreamWriter? LogFile = null;

	private static readonly object IOLock = new();

	static Debug() {
		if (Config.Debug.Logging.OwnLogFile) {
			// For some reason, on Linux it breaks if the log file could not be created?
			try {
				Directory.CreateDirectory(Path.GetDirectoryName(LocalLogPath)!);
				LogFile = new StreamWriter(
					path: LocalLogPath,
					append: false
				);
			}
			catch {
				WarningLn($"Could not create log file at {LocalLogPath}");
			}
		}
	}

	// Logging Stuff

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.Cold)]
	private static string ParseException(Exception exception) {
		var output = new StringBuilder();
		output.AppendLine($"Exception: {exception.GetType().Name} : {exception.Message}\n{exception.GetStackTrace()}");
		Exception currentException = exception;
		var exceptionSet = new HashSet<Exception>() { exception };
		while (currentException.InnerException is not null && exceptionSet.Add(currentException.InnerException)) {
			var innerException = currentException.InnerException;
			output.AppendLine("---");
			output.AppendLine($"InnerException: {innerException.GetType().Name} : {innerException.Message}\n{innerException.GetStackTrace()}");
			currentException = innerException;
		}
		return output.ToString();
	}

	[DebuggerStepThrough, DebuggerHidden]
	private static string Format(this string? memberName, bool format = true) {
		return (!format || memberName is null) ? "" : $"[{memberName}] ";
	}

	[DebuggerStepThrough, DebuggerHidden]
	internal static void Flush() => _ = Console.Error.FlushAsync();

	[DebuggerStepThrough, DebuggerHidden]
	[Conditional("DEBUG")]
	internal static void Break() => Debugger.Break();
}
