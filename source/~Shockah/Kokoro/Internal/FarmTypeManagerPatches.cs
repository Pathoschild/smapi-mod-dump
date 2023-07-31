/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Shockah.Kokoro;

internal static class FarmTypeManagerPatches
{
	internal static void Apply(Harmony harmony)
	{
		if (!Kokoro.Instance.Helper.ModRegistry.IsLoaded("Esca.FarmTypeManager"))
			return;

		var modEntryType = AccessTools.TypeByName("FarmTypeManager.ModEntry, FarmTypeManager");
		if (modEntryType is null)
		{
			Kokoro.Instance.Monitor.Log("Tried to patch Farm Type Manager, but failed.", LogLevel.Error);
		}
		else
		{
			harmony.TryPatch(
				monitor: Kokoro.Instance.Monitor,
				original: () => AccessTools.Method(modEntryType, "DayStarted"),
				transpiler: new HarmonyMethod(AccessTools.Method(typeof(FarmTypeManagerPatches), nameof(FarmTypeManager_ModEntry_DayStarted_Transpiler)))
			);
			harmony.TryPatch(
				monitor: Kokoro.Instance.Monitor,
				original: () => AccessTools.Method(modEntryType, "TimeChanged"),
				transpiler: new HarmonyMethod(AccessTools.Method(typeof(FarmTypeManagerPatches), nameof(FarmTypeManager_ModEntry_TimeChanged_Transpiler)))
			);

#if DEBUG
			var generationType = AccessTools.Inner(modEntryType, "Generation");

			harmony.TryPatch(
				monitor: Kokoro.Instance.Monitor,
				original: () => AccessTools.Method(generationType, "SpawnTimedSpawns"),
				prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmTypeManagerPatches), nameof(FarmTypeManager_ModEntry_Generation_SpawnTimedSpawns_Prefix)))
			);
#endif
		}
	}

	private static IEnumerable<CodeInstruction> FarmTypeManager_ModEntry_DayStarted_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod, ILGenerator il)
	{
		try
		{
			var modEntryType = AccessTools.TypeByName("FarmTypeManager.ModEntry, FarmTypeManager")!;
			var stardewTimeType = AccessTools.Inner(modEntryType, "StardewTime");

			var timeLocal = il.DeclareLocal(typeof(int));
			var whileLoopStartLabel = il.DefineLabel();
			var whileLoopCheckLabel = il.DefineLabel();
			var notFullHourLabel = il.DefineLabel();

			return new SequenceBlockMatcher<CodeInstruction>(instructions)
				.AsGuidAnchorable()
				// split into two `Find`s, because current published FTM is built in debug and the instructions differ between these parts:
				// just `bne.un.s` vs `ceq` + `stloc.s` + `ldloc.s` + `brfalse.s`
				.Find(
					ILMatches.Call("get_StartOfDay").WithAutoAnchor(out Guid callGetStartOfDayAnchor),
					ILMatches.Stloc(stardewTimeType, originalMethod.GetMethodBody()!.LocalVariables).WithAutoAnchor(out Guid stlocStardewTimeAnchor),
					ILMatches.Ldloca(stardewTimeType, originalMethod.GetMethodBody()!.LocalVariables),
					ILMatches.Call("get_Time").WithAutoAnchor(out Guid callGetTimeAnchor)
				)
				.Find(
					ILMatches.Ldsfld("TimedSpawns").WithAutoAnchor(out Guid ldsfldTimedSpawnsAnchor),
					ILMatches.LdcI4(600),
					ILMatches.Call("op_Implicit").WithAutoAnchor(out Guid callOpImplicitAnchor),
					ILMatches.Newobj(AccessTools.Constructor(typeof(Nullable<>).MakeGenericType(stardewTimeType), new Type[] { stardewTimeType })).WithAutoAnchor(out Guid newobjNullableStardewTimeAnchor),
					ILMatches.Call("SpawnTimedSpawns").WithAutoAnchor(out Guid callSpawnTimedSpawnsAnchor)
				)
				.PointerMatcher(SequenceMatcherRelativeElement.Last)
				.EncompassUntil(callGetStartOfDayAnchor)
				.AnchorBlock(out Guid findAnchor)

				.PointerMatcher(callGetStartOfDayAnchor).Element(out var callGetStartOfDayInstruction)
				.PointerMatcher(stlocStardewTimeAnchor).Element(out var stlocStardewTimeInstruction).CreateLdlocaInstruction(out var ldlocaStardewTimeInstruction)
				.PointerMatcher(callGetTimeAnchor).Element(out var callGetTimeInstruction)
				.PointerMatcher(ldsfldTimedSpawnsAnchor).Element(out var ldsfldTimedSpawnsInstruction)
				.PointerMatcher(callOpImplicitAnchor).Element(out var callOpImplicitInstruction)
				.PointerMatcher(newobjNullableStardewTimeAnchor).Element(out var newobjNullableStardewTimeInstruction)
				.PointerMatcher(callSpawnTimedSpawnsAnchor).Element(out var callSpawnTimedSpawnsInstruction)

				.BlockMatcher(findAnchor)
				.Replace(
					new CodeInstruction(OpCodes.Ldc_I4, 600),
					new CodeInstruction(OpCodes.Stloc, timeLocal.LocalIndex),
					new CodeInstruction(OpCodes.Br, whileLoopCheckLabel),

					ldsfldTimedSpawnsInstruction.Clone().WithLabels(whileLoopStartLabel),
					new CodeInstruction(OpCodes.Ldloc, timeLocal.LocalIndex),
					callOpImplicitInstruction.Clone(),
					newobjNullableStardewTimeInstruction.Clone(),
					callSpawnTimedSpawnsInstruction.Clone(),

					new CodeInstruction(OpCodes.Ldloc, timeLocal.LocalIndex),
					new CodeInstruction(OpCodes.Ldc_I4, 10),
					new CodeInstruction(OpCodes.Add),
					new CodeInstruction(OpCodes.Stloc, timeLocal.LocalIndex),

					new CodeInstruction(OpCodes.Ldloc, timeLocal.LocalIndex),
					new CodeInstruction(OpCodes.Ldc_I4, 100),
					new CodeInstruction(OpCodes.Rem),
					new CodeInstruction(OpCodes.Ldc_I4, 60),
					new CodeInstruction(OpCodes.Bne_Un, notFullHourLabel),

					new CodeInstruction(OpCodes.Ldloc, timeLocal.LocalIndex),
					new CodeInstruction(OpCodes.Ldc_I4, 40),
					new CodeInstruction(OpCodes.Add),
					new CodeInstruction(OpCodes.Stloc, timeLocal.LocalIndex),

					new CodeInstruction(OpCodes.Ldloc, timeLocal.LocalIndex).WithLabels(whileLoopCheckLabel, notFullHourLabel),
					callGetStartOfDayInstruction.Clone(),
					stlocStardewTimeInstruction.Clone(),
					ldlocaStardewTimeInstruction.Clone(),
					callGetTimeInstruction.Clone(),
					new CodeInstruction(OpCodes.Ble, whileLoopStartLabel)
				)

				.AllElements();
		}
		catch (Exception ex)
		{
			Kokoro.Instance.Monitor.Log($"Could not patch method {originalMethod} - {Kokoro.Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
			return instructions;
		}
	}

	private static IEnumerable<CodeInstruction> FarmTypeManager_ModEntry_TimeChanged_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod, ILGenerator il)
	{
		try
		{
			var modEntryType = AccessTools.TypeByName("FarmTypeManager.ModEntry, FarmTypeManager")!;
			var stardewTimeType = AccessTools.Inner(modEntryType, "StardewTime");

			var timeLocal = il.DeclareLocal(typeof(int));
			var whileLoopStartLabel = il.DefineLabel();
			var whileLoopCheckLabel = il.DefineLabel();
			var notFullHourLabel = il.DefineLabel();

			return new SequenceBlockMatcher<CodeInstruction>(instructions)
				.AsGuidAnchorable()
				.Find(
					ILMatches.Call("get_StartOfDay").WithAutoAnchor(out Guid callGetStartOfDayAnchor)
				)
				.Find(
					ILMatches.Ldsfld("TimedSpawns").WithAutoAnchor(out Guid ldsfldTimedSpawnsAnchor),
					ILMatches.Ldarg(2),
					ILMatches.Call("get_NewTime"),
					ILMatches.Call("op_Implicit").WithAutoAnchor(out Guid callOpImplicitAnchor),
					ILMatches.Newobj(AccessTools.Constructor(typeof(Nullable<>).MakeGenericType(stardewTimeType), new Type[] { stardewTimeType })).WithAutoAnchor(out Guid newobjNullableStardewTimeAnchor),
					ILMatches.Call("SpawnTimedSpawns").WithAutoAnchor(out Guid callSpawnTimedSpawnsAnchor)
				)
				.AnchorBlock(out Guid findAnchor)

				.PointerMatcher(callGetStartOfDayAnchor).Element(out var callGetStartOfDayInstruction)
				.PointerMatcher(ldsfldTimedSpawnsAnchor).Element(out var ldsfldTimedSpawnsInstruction)
				.PointerMatcher(callOpImplicitAnchor).Element(out var callOpImplicitInstruction)
				.PointerMatcher(newobjNullableStardewTimeAnchor).Element(out var newobjNullableStardewTimeInstruction)
				.PointerMatcher(callSpawnTimedSpawnsAnchor).Element(out var callSpawnTimedSpawnsInstruction)

				.BlockMatcher(findAnchor)
				.Replace(
					new CodeInstruction(OpCodes.Ldarg_2),
					new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(TimeChangedEventArgs), nameof(TimeChangedEventArgs.OldTime))),
					new CodeInstruction(OpCodes.Stloc, timeLocal.LocalIndex),
					new CodeInstruction(OpCodes.Br, whileLoopCheckLabel),

					new CodeInstruction(OpCodes.Ldloc, timeLocal.LocalIndex).WithLabels(whileLoopStartLabel),
					new CodeInstruction(OpCodes.Ldc_I4, 10),
					new CodeInstruction(OpCodes.Add),
					new CodeInstruction(OpCodes.Stloc, timeLocal.LocalIndex),

					new CodeInstruction(OpCodes.Ldloc, timeLocal.LocalIndex),
					new CodeInstruction(OpCodes.Ldc_I4, 100),
					new CodeInstruction(OpCodes.Rem),
					new CodeInstruction(OpCodes.Ldc_I4, 60),
					new CodeInstruction(OpCodes.Bne_Un, notFullHourLabel),

					new CodeInstruction(OpCodes.Ldloc, timeLocal.LocalIndex),
					new CodeInstruction(OpCodes.Ldc_I4, 40),
					new CodeInstruction(OpCodes.Add),
					new CodeInstruction(OpCodes.Stloc, timeLocal.LocalIndex),

					ldsfldTimedSpawnsInstruction.Clone().WithLabels(notFullHourLabel),
					new CodeInstruction(OpCodes.Ldloc, timeLocal.LocalIndex),
					callOpImplicitInstruction.Clone(),
					newobjNullableStardewTimeInstruction.Clone(),
					callSpawnTimedSpawnsInstruction.Clone(),

					new CodeInstruction(OpCodes.Ldloc, timeLocal.LocalIndex).WithLabels(whileLoopCheckLabel),
					new CodeInstruction(OpCodes.Ldarg_2),
					new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(TimeChangedEventArgs), nameof(TimeChangedEventArgs.NewTime))),
					new CodeInstruction(OpCodes.Blt, whileLoopStartLabel)
				)

				.AllElements();
		}
		catch (Exception ex)
		{
			Kokoro.Instance.Monitor.Log($"Could not patch method {originalMethod} - {Kokoro.Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
			return instructions;
		}
	}

#if DEBUG
	private static void FarmTypeManager_ModEntry_Generation_SpawnTimedSpawns_Prefix(object? time)
	{
		if (time is null)
		{
			Kokoro.Instance.Monitor.Log("FTM called SpawnTimedSpawns for time: null", LogLevel.Debug);
			return;
		}

		var modEntryType = AccessTools.TypeByName("FarmTypeManager.ModEntry, FarmTypeManager")!;
		var stardewTimeType = AccessTools.Inner(modEntryType, "StardewTime");
		var timeGetter = AccessTools.PropertyGetter(stardewTimeType, "Time");
		Kokoro.Instance.Monitor.Log($"FTM called SpawnTimedSpawns for time: {(int)timeGetter.Invoke(time, null)!}", LogLevel.Debug);
	}
#endif
}