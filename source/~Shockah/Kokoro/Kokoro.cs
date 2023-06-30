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
using Shockah.Kokoro.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Shockah.Kokoro;

public class Kokoro : BaseMod
{
	public static Kokoro Instance { get; private set; } = null!;

	private PerScreen<LinkedList<string>> QueuedObjectDialogue { get; init; } = new(() => new());

	public override void Entry(IModHelper helper)
	{
		Instance = this;

		// force-referencing Shrike assemblies, otherwise none dependent mods will load
		_ = typeof(ISequenceMatcher<CodeInstruction>).Name;
		_ = typeof(ILMatches).Name;

		helper.Events.GameLoop.GameLaunched += OnGameLaunched;
		helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
		MachineTracker.Setup(Monitor, helper, new Harmony(ModManifest.UniqueID));
	}

	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
	{
		var harmony = new Harmony(ModManifest.UniqueID);

		if (Helper.ModRegistry.IsLoaded("Esca.FarmTypeManager"))
		{
			var modEntryType = AccessTools.TypeByName("FarmTypeManager.ModEntry, FarmTypeManager");
			if (modEntryType is null)
			{
				Monitor.Log("Tried to patch Farm Type Manager, but failed.", LogLevel.Error);
			}
			else
			{
				harmony.TryPatch(
					monitor: Monitor,
					original: () => AccessTools.Method(modEntryType, "DayStarted"),
					transpiler: new HarmonyMethod(AccessTools.Method(GetType(), nameof(FarmTypeManager_ModEntry_DayStarted_Transpiler)))
				);
				harmony.TryPatch(
					monitor: Monitor,
					original: () => AccessTools.Method(modEntryType, "TimeChanged"),
					transpiler: new HarmonyMethod(AccessTools.Method(GetType(), nameof(FarmTypeManager_ModEntry_TimeChanged_Transpiler)))
				);
			}
		}
	}

	private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
	{
		// dequeue object dialogue
		var message = QueuedObjectDialogue.Value.First;
		if (message is not null && Game1.activeClickableMenu is not DialogueBox)
		{
			QueuedObjectDialogue.Value.RemoveFirst();
			Game1.drawObjectDialogue(message.Value);
		}
	}

	public void QueueObjectDialogue(string message)
	{
		if (Game1.activeClickableMenu is DialogueBox)
			QueuedObjectDialogue.Value.AddLast(message);
		else
			Game1.drawObjectDialogue(message);
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
			Instance.Monitor.Log($"Could not patch method {originalMethod} - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
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
			Instance.Monitor.Log($"Could not patch method {originalMethod} - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
			return instructions;
		}
	}
}