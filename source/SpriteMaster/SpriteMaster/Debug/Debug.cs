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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using static SpriteMaster.Runtime;

namespace SpriteMaster;

internal static partial class Debug {
	private static class Color {
		internal const ConsoleColor Trace = ConsoleColor.Gray;
		internal const ConsoleColor Info = ConsoleColor.White;
		internal const ConsoleColor Warning = ConsoleColor.Yellow;
		internal const ConsoleColor Error = ConsoleColor.Red;
		internal const ConsoleColor Fatal = ConsoleColor.Red;
	}

	private static readonly object IoLock = new();

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
