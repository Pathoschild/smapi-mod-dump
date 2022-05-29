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
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Harmonize.Patches.SMAPI;

internal static class LogMonitor {
#if !SHIPPING
	private static readonly HashSet<string> SilencedMods = new();

	private static bool SilencedMod(string? source) {
		if (source is null) {
			return false;
		}

		var configSilencedMods = Config.Debug.Logging.SilencedMods;
		if (SilencedMods.Count == 0 && configSilencedMods.Length != 0) {
			SilencedMods.AddRange(configSilencedMods);
		}

		return SilencedMods.Contains(source);
	}

	[Harmonize(
		typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade),
		"StardewModdingAPI.Framework.Monitor",
		"LogImpl",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static bool LogImplPre(IMonitor __instance, string? source, string? message, object level) {
		if (!Config.Debug.Logging.SilenceOtherMods) {
			return true;
		}

		if ((int)level != (int)LogLevel.Trace) {
			return true;
		}

		return !SilencedMod(source);
	}
#endif

	private static readonly Func<object, StreamWriter?>? GetLogFileStream = typeof(IMonitor).Assembly.GetType("StardewModdingAPI.Framework.Logging.LogFileManager")?.GetFieldGetter<object, StreamWriter>("Stream");

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static void FlushFileStream(object logFile) {
		var streamWriter = GetLogFileStream!(logFile);
		if (streamWriter is null) {
			return;
		}
		streamWriter.Flush();
	}

	[Harmonize(
		typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade),
		"StardewModdingAPI.Framework.Monitor",
		"LogImpl",
		Harmonize.Fixation.Finalizer,
		Harmonize.PriorityLevel.Last,
		critical: true
	)]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static void LogImplFinalizer(IMonitor __instance, object ___LogFile, string? source, string? message, object level) {
		if (GetLogFileStream is null) {
			return;
		}

		switch ((int)level) {
			case (int)LogLevel.Warn:
			case (int)LogLevel.Error:
			case (int)LogLevel.Alert:
				FlushFileStream(___LogFile);
				break;
			default:
				if (message?.Contains("Exception") ?? false) {
					FlushFileStream(___LogFile);
				}
				break;
		}
	}
}
