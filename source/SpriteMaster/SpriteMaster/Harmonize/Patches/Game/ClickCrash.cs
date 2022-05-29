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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SpriteMaster.Harmonize.Patches.Game;

internal static class ClickCrash {
	private const BindingFlags AllMethods = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;

	private static bool HasWindow => StardewValley.Game1.game1.Window is not null;
	private static readonly TimeSpan RunLoopAfter = TimeSpan.FromMilliseconds(100);

	private static readonly Stopwatch SdlUpdate = Stopwatch.StartNew();
	private static readonly Action? SdlLoopMethod;
	private static readonly Func<bool>? IsOnUIThread;
	private static readonly bool IsRunnable = false;

	static ClickCrash() {
		try {
			var platformGetter = typeof(XNA.Game).GetFieldGetter<object, object>("Platform") ?? throw new NullReferenceException("PlatformGetter");
			var platform = platformGetter(StardewValley.GameRunner.instance) ?? throw new NullReferenceException("Platform");
			var sdlGamePlatformType =
				typeof(XColor).Assembly.
				GetType("Microsoft.Xna.Framework.SdlGamePlatform");
			var sdlLoopMethodInfo =
				sdlGamePlatformType?.
				GetMethod("SdlRunLoop", BindingFlags.Instance | AllMethods);
			SdlLoopMethod = sdlLoopMethodInfo?.CreateDelegate<Action>(platform) ?? throw new NullReferenceException(nameof(SdlLoopMethod));
			var isOnUIThreadMethodInfo = typeof(XColor).Assembly.
				GetType("Microsoft.Xna.Framework.Threading")?.
				GetMethod("IsOnUIThread", BindingFlags.Static | AllMethods);
			IsOnUIThread = isOnUIThreadMethodInfo?.CreateDelegate<Func<bool>>() ?? throw new NullReferenceException(nameof(IsOnUIThread));
		}
		catch (Exception ex) {
			Debug.Error("Failed to configure SDL ticker", ex);
		}

		IsRunnable = SdlLoopMethod is not null && IsOnUIThread is not null;
	}

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	internal static void Initialize() {
		// Does nothing, just basically sets up the static constructor;
	}

	[Harmonize(
		typeof(XColor),
		"Microsoft.Xna.Framework.SdlGamePlatform",
		"SdlRunLoop",
		Harmonize.Fixation.Postfix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static void SdlRunLoopPost(object __instance) {
		if (!Config.IsEnabled) {
			return;
		}

		if (IsRunnable) {
			SdlUpdate.Restart();
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static void OnStartTask() {
		if (!IsRunnable || SdlUpdate.Elapsed < RunLoopAfter) {
			return;
		}

		if (!IsOnUIThread!()) {
			return;
		}

		SdlLoopMethod!();

		SdlUpdate.Restart();
	}

	[Harmonize(
		typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade),
		"StardewModdingAPI.Framework.SModHooks",
		"StartTask",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static void StartTaskPre(object __instance, Task task, string id) {
		if (!Config.IsEnabled) {
			return;
		}

		OnStartTask();
	}

	[Harmonize(
		typeof(StardewModdingAPI.Framework.ModLoading.RewriteFacades.AccessToolsFacade),
		"StardewModdingAPI.Framework.SModHooks",
		"StartTask",
		Harmonize.Fixation.Postfix,
		Harmonize.PriorityLevel.Last,
		generic: Harmonize.Generic.Class,
		critical: false
	)]
	public static void StartTaskPre<T>(T __instance, Task<T> task, string id) {
		if (!Config.IsEnabled) {
			return;
		}

		OnStartTask();
	}
}
