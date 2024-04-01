/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using MusicMaster.Configuration;
using MusicMaster.Hashing;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static MusicMaster.Runtime;

namespace MusicMaster;

internal static partial class Debug {
	private readonly record struct TraceOnceElement(string File, int Line);

	private static readonly HashSet<TraceOnceElement> TraceOnceMap = new();

	[MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough, DebuggerHidden]
	internal static bool CheckLogLevel(LogLevel logLevel) => Config.Debug.Logging.LogLevel <= logLevel;

	[Conditional("DEBUG"), Conditional("TRACE"), DebuggerStepThrough, DebuggerHidden]
	internal static void Trace(string message, bool format = true, [CallerMemberName] string caller = "") {
		if (!CheckLogLevel(LogLevel.Trace))
			return;
		DebugWriteStr($"{caller.Format(format)}{message}", LogLevel.Trace);
	}

	[Conditional("DEBUG"), Conditional("TRACE"), DebuggerStepThrough, DebuggerHidden]
	internal static void TraceOnce(string message, bool format = true, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0) {
		if (!CheckLogLevel(LogLevel.Trace))
			return;

		lock (TraceOnceMap) {
			if (!TraceOnceMap.Add(new(path, line))) {
				return;
			}
		}

		DebugWriteStr($"{caller.Format(format)}{message}", LogLevel.Trace);
	}

	[Conditional("DEBUG"), Conditional("TRACE"), DebuggerStepThrough, DebuggerHidden]
	internal static void Trace<T>(T exception, [CallerMemberName] string caller = "") where T : Exception {
		if (!CheckLogLevel(LogLevel.Trace))
			return;
		TraceLn(ParseException(exception), caller: caller);
	}

	[Conditional("DEBUG"), Conditional("TRACE"), DebuggerStepThrough, DebuggerHidden]
	internal static void TraceOnce<T>(T exception, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0) where T : Exception {
		if (!CheckLogLevel(LogLevel.Trace))
			return;

		lock (TraceOnceMap) {
			if (!TraceOnceMap.Add(new(path, line))) {
				return;
			}
		}

		TraceLn(ParseException(exception), caller: caller);
	}

	[Conditional("DEBUG"), Conditional("TRACE"), DebuggerStepThrough, DebuggerHidden]
	internal static void Trace<T>(string message, T exception, [CallerMemberName] string caller = "") where T : Exception {
		if (!CheckLogLevel(LogLevel.Trace))
			return;
		TraceLn($"{message}\n{ParseException(exception)}", caller: caller);
	}

	[Conditional("DEBUG"), Conditional("TRACE"), DebuggerStepThrough, DebuggerHidden]
	internal static void TraceOnce<T>(string message, T exception, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0) where T : Exception {
		if (!CheckLogLevel(LogLevel.Trace))
			return;

		lock (TraceOnceMap) {
			if (!TraceOnceMap.Add(new(path, line))) {
				return;
			}
		}

		TraceLn($"{message}\n{ParseException(exception)}", caller: caller);
	}

	[Conditional("DEBUG"), Conditional("TRACE"), DebuggerStepThrough, DebuggerHidden]
	private static void TraceLn(string message, bool format = true, [CallerMemberName] string caller = "") {
		Trace($"{message}\n", format, caller);
	}

	[Conditional("TRACE"), DebuggerStepThrough, DebuggerHidden]
	internal static void Info(string message, bool format = true, [CallerMemberName] string caller = "") {
		if (!CheckLogLevel(LogLevel.Debug))
			return;
		DebugWriteStr($"{caller.Format(format)}{message}", LogLevel.Debug);
	}

	[Conditional("TRACE"), DebuggerStepThrough, DebuggerHidden]
	internal static void Info<T>(T exception, [CallerMemberName] string caller = "") where T : Exception {
		if (!CheckLogLevel(LogLevel.Debug))
			return;
		InfoLn(ParseException(exception), caller: caller);
	}

	[Conditional("TRACE"), DebuggerStepThrough, DebuggerHidden]
	internal static void Info<T>(string message, T exception, [CallerMemberName] string caller = "") where T : Exception {
		if (!CheckLogLevel(LogLevel.Debug))
			return;
		InfoLn($"{message}\n{ParseException(exception)}", caller: caller);
	}

	[Conditional("TRACE"), DebuggerStepThrough, DebuggerHidden]
	private static void InfoLn(string message, bool format = true, [CallerMemberName] string caller = "") {
		Info($"{message}\n", format, caller);
	}

	//[DebuggerStepThrough, DebuggerHidden]
	internal static void Message(string message, bool format = true, [CallerMemberName] string caller = "") {
		if (!CheckLogLevel(LogLevel.Info))
			return;
		DebugWriteStr($"{caller.Format(format)}{message}", LogLevel.Info);
	}

	[DebuggerStepThrough, DebuggerHidden]
	internal static void Message<T>(T exception, [CallerMemberName] string caller = "") where T : Exception {
		if (!CheckLogLevel(LogLevel.Info))
			return;
		MessageLn(ParseException(exception));
	}

	[DebuggerStepThrough, DebuggerHidden]
	private static void MessageLn(string message, bool format = true, [CallerMemberName] string caller = "") {
		Message($"{message}\n", format, caller);
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.Cold)]
	internal static void Warning(string message, bool format = true, [CallerMemberName] string caller = "") {
		if (!CheckLogLevel(LogLevel.Warn))
			return;
		DebugWrite(LogLevel.Warn, $"{caller.Format(format)}{message}");
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.Cold)]
	internal static void Warning<T>(T exception, [CallerMemberName] string caller = "") where T : Exception {
		if (!CheckLogLevel(LogLevel.Warn))
			return;
		WarningLn(ParseException(exception), caller: caller);
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.Cold)]
	internal static void Warning<T>(string message, T exception, [CallerMemberName] string caller = "") where T : Exception {
		if (!CheckLogLevel(LogLevel.Warn))
			return;
		WarningLn($"{message}\n{ParseException(exception)}", caller: caller);
	}

	[DebuggerStepThrough, DebuggerHidden]
	private static void WarningLn(string message, bool format = true, [CallerMemberName] string caller = "") {
		Warning($"{message}\n", format, caller);
	}

	private static readonly HashSet<ulong> WarningOnceSet = new();

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.Cold)]
	internal static void WarningOnce(string message, bool format = true, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0) {
		if (!CheckLogLevel(LogLevel.Warn) || !WarningOnceSet.Add(HashUtility.Combine(caller, path, line)))
			return;
		DebugWrite(LogLevel.Warn, $"{caller.Format(format)}{message}");
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.Cold)]
	internal static void WarningOnce<T>(T exception, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0) where T : Exception {
		if (!CheckLogLevel(LogLevel.Warn) || !WarningOnceSet.Add(HashUtility.Combine(caller, path, line)))
			return;
		WarningLn(ParseException(exception), caller: caller);
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.Cold)]
	internal static void WarningOnce<T>(string message, T exception, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0) where T : Exception {
		if (!CheckLogLevel(LogLevel.Warn) || !WarningOnceSet.Add(HashUtility.Combine(caller, path, line)))
			return;
		WarningLn($"{message}\n{ParseException(exception)}", caller: caller);
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.Cold)]
	internal static void Error(string message, bool format = true, [CallerMemberName] string caller = "") {
		if (!CheckLogLevel(LogLevel.Error))
			return;
		DebugWrite(LogLevel.Error, $"{caller.Format(format)}{message}");
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.Cold)]
	internal static void Error<T>(T exception, [CallerMemberName] string caller = "") where T : Exception {
		if (!CheckLogLevel(LogLevel.Error))
			return;
		ErrorLn(ParseException(exception), caller: caller);
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.Cold)]
	internal static void Error<T>(string message, T exception, [CallerMemberName] string caller = "") where T : Exception {
		if (!CheckLogLevel(LogLevel.Error))
			return;
		ErrorLn($"{message}\n{ParseException(exception)}", caller: caller);
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ErrorLn(string message, bool format = true, [CallerMemberName] string caller = "") {
		Error($"{message}\n", format, caller);
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.ErrorPath)]
	internal static void Fatal(string message, bool format = true, [CallerMemberName] string caller = "") {
		try {
			if (!CheckLogLevel(LogLevel.Alert))
				return;
			DebugWrite(LogLevel.Alert, $"{caller.Format(format)}{message}");
		}
		finally {
			if (!Config.ForcedDisable) {
				DebugWrite(LogLevel.Alert, "Fatal Error encountered, shutting down MusicMaster");
				Config.ForcedDisable = true;
			}
		}
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.ErrorPath)]
	internal static void Fatal<T>(T exception, [CallerMemberName] string caller = "") where T : Exception {
		if (!CheckLogLevel(LogLevel.Alert))
			return;
		FatalLn(ParseException(exception), caller: caller);
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.ErrorPath)]
	internal static void Fatal<T>(string message, T exception, [CallerMemberName] string caller = "") where T : Exception {
		if (!CheckLogLevel(LogLevel.Alert))
			return;
		FatalLn($"{message}\n{ParseException(exception)}", caller: caller);
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.ErrorPath)]
	private static void FatalLn(string message, bool format = true, [CallerMemberName] string caller = "") {
		Fatal($"{message}\n", format, caller);
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.Cold)]
	internal static void ConditionalError(bool condition, string message, bool format = true, [CallerMemberName] string caller = "") {
		if (condition) {
			Error(message: message, format: format, caller: caller);
		}
		else {
#if DEBUG
			Warning(message: message, format: format, caller: caller);
#else
			Trace(message: message, format: format, caller: caller);
#endif
		}
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.Cold)]
	internal static void ConditionalError<T>(bool condition, T exception, [CallerMemberName] string caller = "") where T : Exception {
		if (condition) {
			Error(exception: exception, caller: caller);
		}
		else {
			Trace(exception: exception, caller: caller);
		}
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.Cold)]
	internal static void ConditionalError<T>(bool condition, string message, T exception, [CallerMemberName] string caller = "") where T : Exception {
		if (condition) {
			Error(message: message, exception: exception, caller: caller);
		}
		else {
			Trace(message: message, caller: caller);
		}
	}

	[DebuggerStepThrough, DebuggerHidden]
	[MethodImpl(MethodImpl.Cold)]
	private static void ConditionalErrorLn(bool condition, string message, bool format = true, [CallerMemberName] string caller = "") {
		if (condition) {
			ErrorLn(message: message, format: format, caller: caller);
		}
		else {
			TraceLn(message: message, format: format, caller: caller);
		}
	}
}
