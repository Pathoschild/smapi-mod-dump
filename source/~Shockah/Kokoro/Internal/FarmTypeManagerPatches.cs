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
		if (!ModEntry.Instance.Helper.ModRegistry.IsLoaded("Esca.FarmTypeManager"))
			return;

		var modEntryType = AccessTools.TypeByName("FarmTypeManager.ModEntry, FarmTypeManager");
		if (modEntryType is null)
		{
			ModEntry.Instance.Monitor.Log("Tried to patch Farm Type Manager, but failed.", LogLevel.Error);
		}
		else
		{
			harmony.TryPatch(
				monitor: ModEntry.Instance.Monitor,
				original: () => AccessTools.DeclaredMethod(modEntryType, "DayStarted"),
				transpiler: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(FarmTypeManagerPatches), nameof(FarmTypeManager_ModEntry_DayStarted_Transpiler)))
			);
			harmony.TryPatch(
				monitor: ModEntry.Instance.Monitor,
				original: () => AccessTools.DeclaredMethod(modEntryType, "TimeChanged"),
				transpiler: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(FarmTypeManagerPatches), nameof(FarmTypeManager_ModEntry_TimeChanged_Transpiler)))
			);

#if DEBUG
			var generationType = AccessTools.Inner(modEntryType, "Generation");

			harmony.TryPatch(
				monitor: ModEntry.Instance.Monitor,
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
				// split into two `Find`s, because current published FTM is built in debug and the instructions differ between these parts:
				// just `bne.un.s` vs `ceq` + `stloc.s` + `ldloc.s` + `brfalse.s`
				.Find(
					ILMatches.Call("get_StartOfDay").Anchor(out var callGetStartOfDayAnchor).SelectElement(out var callGetStartOfDay, i => i.Clone()),
					ILMatches.Stloc(stardewTimeType, originalMethod).CreateLdlocaInstruction(out var ldlocaStardewTime).CreateStlocInstruction(out var stlocStardewTime),
					ILMatches.Ldloca(stardewTimeType, originalMethod),
					ILMatches.Call("get_Time").SelectElement(out var callGetTime, i => i.Clone())
				)
				.Find(
					ILMatches.Ldsfld("TimedSpawns").SelectElement(out var ldsfldTimedSpawns, i => i.Clone()),
					ILMatches.LdcI4(600),
					ILMatches.Call("op_Implicit").SelectElement(out var callOpImplicit, i => i.Clone()),
					ILMatches.Newobj(AccessTools.Constructor(typeof(Nullable<>).MakeGenericType(stardewTimeType), [stardewTimeType])).SelectElement(out var newobjNullableStardewTime, i => i.Clone()),
					ILMatches.Call("SpawnTimedSpawns").SelectElement(out var callSpawnTimedSpawns, i => i.Clone())
				)
				.PointerMatcher(SequenceMatcherRelativeElement.Last)
				.Anchors().EncompassUntil(callGetStartOfDayAnchor)
				.Replace(
					new CodeInstruction(OpCodes.Ldc_I4, 600),
					new CodeInstruction(OpCodes.Stloc, timeLocal.LocalIndex),
					new CodeInstruction(OpCodes.Br, whileLoopCheckLabel),

					ldsfldTimedSpawns.Value.WithLabels(whileLoopStartLabel),
					new CodeInstruction(OpCodes.Ldloc, timeLocal.LocalIndex),
					callOpImplicit.Value,
					newobjNullableStardewTime.Value,
					callSpawnTimedSpawns.Value,

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
					callGetStartOfDay.Value,
					stlocStardewTime.Value,
					ldlocaStardewTime.Value,
					callGetTime.Value,
					new CodeInstruction(OpCodes.Ble, whileLoopStartLabel)
				)

				.AllElements();
		}
		catch (Exception ex)
		{
			ModEntry.Instance.Monitor.Log($"Could not patch method {originalMethod} - {ModEntry.Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
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
				.Find(
					ILMatches.Call("get_StartOfDay").SelectElement(out var callGetStartOfDay, i => i.Clone())
				)
				.Find(
					ILMatches.Ldsfld("TimedSpawns").SelectElement(out var ldsfldTimedSpawns, i => i.Clone()),
					ILMatches.Ldarg(2),
					ILMatches.Call("get_NewTime"),
					ILMatches.Call("op_Implicit").SelectElement(out var callOpImplicit, i => i.Clone()),
					ILMatches.Newobj(AccessTools.Constructor(typeof(Nullable<>).MakeGenericType(stardewTimeType), [stardewTimeType])).SelectElement(out var newobjNullableStardewTime, i => i.Clone()),
					ILMatches.Call("SpawnTimedSpawns").SelectElement(out var callSpawnTimedSpawns, i => i.Clone())
				)
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

					ldsfldTimedSpawns.Value.WithLabels(notFullHourLabel),
					new CodeInstruction(OpCodes.Ldloc, timeLocal.LocalIndex),
					callOpImplicit.Value,
					newobjNullableStardewTime.Value,
					callSpawnTimedSpawns.Value,

					new CodeInstruction(OpCodes.Ldloc, timeLocal.LocalIndex).WithLabels(whileLoopCheckLabel),
					new CodeInstruction(OpCodes.Ldarg_2),
					new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(TimeChangedEventArgs), nameof(TimeChangedEventArgs.NewTime))),
					new CodeInstruction(OpCodes.Blt, whileLoopStartLabel)
				)

				.AllElements();
		}
		catch (Exception ex)
		{
			ModEntry.Instance.Monitor.Log($"Could not patch method {originalMethod} - {ModEntry.Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
			return instructions;
		}
	}

#if DEBUG
	private static void FarmTypeManager_ModEntry_Generation_SpawnTimedSpawns_Prefix(object? time)
	{
		if (time is null)
		{
			ModEntry.Instance.Monitor.Log("FTM called SpawnTimedSpawns for time: null", LogLevel.Debug);
			return;
		}

		var modEntryType = AccessTools.TypeByName("FarmTypeManager.ModEntry, FarmTypeManager")!;
		var stardewTimeType = AccessTools.Inner(modEntryType, "StardewTime");
		var timeGetter = AccessTools.PropertyGetter(stardewTimeType, "Time");
		ModEntry.Instance.Monitor.Log($"FTM called SpawnTimedSpawns for time: {(int)timeGetter.Invoke(time, null)!}", LogLevel.Debug);
	}
#endif
}