/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Threading;

namespace SpriteMaster.Harmonize.Patches.SMAPI;

internal static class DeprecationManager {
	private static readonly object DeprecationManagerLock = new();

	[Harmonize(
		typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade),
		"StardewModdingAPI.Framework.Deprecations.DeprecationManager",
		"Warn",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.First,
		critical: false
	)]
	public static void DeprecationManagerWarnPre(
		object __instance,
		object? source,
		string nounPhrase,
		string version,
		Enum severity,
		string[]? unlessStackIncludes,
		bool logStackTrace
	) {
		Monitor.Enter(DeprecationManagerLock);
	}

	[Harmonize(
		typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade),
		"StardewModdingAPI.Framework.Deprecations.DeprecationManager",
		"Warn",
		Harmonize.Fixation.Finalizer,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static void DeprecationManagerWarnPost(
		object __instance,
		object? source,
		string nounPhrase,
		string version,
		Enum severity,
		string[]? unlessStackIncludes,
		bool logStackTrace
	) {
		Monitor.Exit(DeprecationManagerLock);
	}

	[Harmonize(
		typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade),
		"StardewModdingAPI.Framework.Deprecations.DeprecationManager",
		"PrintQueued",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.First,
		critical: false
	)]
	public static void PrintQueuedPre(object __instance) {
		Monitor.Enter(DeprecationManagerLock);
	}

	[Harmonize(
		typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade),
		"StardewModdingAPI.Framework.Deprecations.DeprecationManager",
		"PrintQueued",
		Harmonize.Fixation.Finalizer,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static void PrintQueuedPost(object __instance) {
		Monitor.Exit(DeprecationManagerLock);
	}
}
