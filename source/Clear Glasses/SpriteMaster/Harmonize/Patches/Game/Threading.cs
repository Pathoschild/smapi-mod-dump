/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using HarmonyLib;
using LinqFasterer;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Extensions.Reflection;
using System;
using System.Collections.Generic;
using static SpriteMaster.Harmonize.Harmonize;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Harmonize.Patches.Game;

internal static class Threading {
	private static readonly Func<List<Action>> ThreadingActionsGet = 
		typeof(Microsoft.Xna.Framework.Threading).
		GetFieldGetter<List<Action>>("actions") ??
			throw new NullReferenceException(nameof(ThreadingActionsGet));
	
	// TODO : find a nice, generic way to do this without [ModuleInitializer]
	private static readonly bool Functional = true;

	[Harmonize(
		typeof(Microsoft.Xna.Framework.Threading),
		"Run",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool Run() {
		if (!Functional || !Config.IsUnconditionallyEnabled || !Config.Extras.OptimizeEngineTaskRunner) {
			return true;
		}

		var actions = ThreadingActionsGet();
		var localActions = actions.ExchangeClearLocked();

		foreach (var action in localActions) {
			action();
		}

		return false;
	}

	[HarmonizeTranspile(
		typeof(Microsoft.Xna.Framework.Threading),
		"EnsureUIThread",
		argumentTypes: new Type[] {},
		platform: Platform.MonoGame,
		instance: false
	)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<CodeInstruction> EnsureUIThreadTranspiler(
		IEnumerable<CodeInstruction> instructions,
		ILGenerator generator
	) {
		var isMainThreadField = typeof(ThreadingExt).GetField(nameof(ThreadingExt.IsMainThread), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static) ??
			throw new NullReferenceException($"Could not access field '{nameof(ThreadingExt.IsMainThread)}'");

		var throwLabel = generator.DefineLabel();
		yield return new(OpCodes.Ldsfld, isMainThreadField);
		yield return new(OpCodes.Brfalse_S, throwLabel);
		yield return new(OpCodes.Ret);

		yield return new(OpCodes.Ldstr, "Operation not called on UI thread.") {labels = new() {throwLabel}};
		yield return new(OpCodes.Call, new Action<string>(ThrowHelper.ThrowInvalidOperationException).Method);
		yield return new(OpCodes.Ret);
	}
}
