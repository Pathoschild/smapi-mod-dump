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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using SObject = StardewValley.Object;

namespace Shockah.Kokoro.Stardew;

public delegate void MachineTrackerChangedEvent(GameLocation location, SObject machine, MachineProcessingState? oldValue, MachineProcessingState? newValue);

public static class MachineTracker
{
	private static bool IsSetup = false;

	private static readonly PerScreen<List<WeakReference<SObject>>> TrackedMachines = new(() => new());
	private static readonly PerScreen<HashSet<(GameLocation location, SObject machine)>> QueuedMachineUpdates = new(() => new());
	private static readonly PerScreen<List<SObject>> IgnoredMachinesForUpdates = new(() => new());
	private static readonly PerScreen<ConditionalWeakTable<SObject, StructRef<MachineProcessingState>>> MachineProcessingStateCache = new(() => new());

	public static event MachineTrackerChangedEvent? MachineChangedEvent;

	internal static void Setup(IMonitor monitor, IModHelper helper, Harmony harmony)
	{
		if (IsSetup)
			return;
		IsSetup = true;

		helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
		helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
		helper.Events.World.ObjectListChanged += OnObjectListChanged;

		harmony.TryPatchVirtual(
			monitor: monitor,
			original: () => AccessTools.Method(typeof(SObject), nameof(SObject.performObjectDropInAction)),
			prefix: new HarmonyMethod(typeof(MachineTracker), nameof(SObject_performObjectDropInAction_Prefix)),
			postfix: new HarmonyMethod(typeof(MachineTracker), nameof(SObject_performObjectDropInAction_Postfix))
		);
		harmony.TryPatchVirtual(
			monitor: monitor,
			original: () => AccessTools.Method(typeof(SObject), nameof(SObject.checkForAction)),
			prefix: new HarmonyMethod(typeof(MachineTracker), nameof(SObject_checkForAction_Prefix)),
			postfix: new HarmonyMethod(typeof(MachineTracker), nameof(SObject_checkForAction_Postfix))
		);
	}

	private static void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
	{
		QueuedMachineUpdates.Value.Clear();
		IgnoredMachinesForUpdates.Value.Clear();
	}

	private static void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
	{
		if (!Context.IsWorldReady)
			return;

		foreach (var (location, machine) in QueuedMachineUpdates.Value)
			UpsertMachine(location, machine);
		QueuedMachineUpdates.Value.Clear();
	}

	private static void OnObjectListChanged(object? sender, ObjectListChangedEventArgs e)
	{
		foreach (var @object in e.Removed)
		{
			if (MachineProcessingStateCache.Value.TryGetValue(@object.Value, out var oldState))
			{
				MachineChangedEvent?.Invoke(e.Location, @object.Value, oldState, null);
				MachineProcessingStateCache.Value.Remove(@object.Value);
			}
		}
		foreach (var @object in e.Added)
			StartTrackingMachine(e.Location, @object.Value);
	}

	[SuppressMessage("SMAPI.CommonErrors", "AvoidNetField:Avoid Netcode types when possible", Justification = "Registering for events")]
	private static void StartTrackingMachine(GameLocation location, SObject machine)
	{
		if (!IsMachine(machine))
			return;

		UpsertMachine(location, machine);
		foreach (var refToRemove in TrackedMachines.Value.Where(r => !r.TryGetTarget(out _)).ToList())
			TrackedMachines.Value.Remove(refToRemove);
		if (TrackedMachines.Value.Any(r => r.TryGetTarget(out var trackedMachine) && machine == trackedMachine))
			return;

		machine.readyForHarvest.fieldChangeVisibleEvent += (_, oldValue, newValue) =>
		{
			if (IgnoredMachinesForUpdates.Value.Contains(machine))
				return;
			QueuedMachineUpdates.Value.Add((location, machine));
		};
		machine.minutesUntilReady.fieldChangeVisibleEvent += (_, oldValue, newValue) =>
		{
			if (IgnoredMachinesForUpdates.Value.Contains(machine))
				return;
			QueuedMachineUpdates.Value.Add((location, machine));
		};
		machine.heldObject.fieldChangeVisibleEvent += (_, oldValue, newValue) =>
		{
			if (IgnoredMachinesForUpdates.Value.Contains(machine))
				return;
			QueuedMachineUpdates.Value.Add((location, machine));
		};
		if (machine is CrabPot crabPot)
		{
			crabPot.bait.fieldChangeVisibleEvent += (_, oldValue, newValue) =>
			{
				if (IgnoredMachinesForUpdates.Value.Contains(machine))
					return;
				QueuedMachineUpdates.Value.Add((location, machine));
			};
		}

		TrackedMachines.Value.Add(new(machine));
	}

	private static void UpsertMachine(GameLocation location, SObject machine)
	{
		var newState = new MachineProcessingState(machine);
		if (MachineProcessingStateCache.Value.TryGetValue(machine, out var oldState))
		{
			if (newState != oldState.Value)
				MachineChangedEvent?.Invoke(location, machine, oldState.Value, newState);
		}
		else
		{
			MachineChangedEvent?.Invoke(location, machine, null, newState);
		}
		MachineProcessingStateCache.Value.AddOrUpdate(machine, newState);
	}

	private static bool IsMachine(SObject @object)
	{
		if (@object is CrabPot || @object is WoodChipper)
			return true;
		if (@object.IsSprinkler())
			return false;
		if (!@object.bigCraftable.Value && @object.Category != SObject.BigCraftableCategory)
			return false;
		if (@object.heldObject.Value is Chest || @object.heldObject.Value?.Name == "Chest")
			return false;
		return true;
	}

	private static void SObject_performObjectDropInAction_Prefix(SObject __instance, bool __1 /* probe */)
	{
		if (__1)
			IgnoredMachinesForUpdates.Value.Add(__instance);
	}

	private static void SObject_performObjectDropInAction_Postfix(SObject __instance, bool __1 /* probe */)
	{
		if (__1)
			IgnoredMachinesForUpdates.Value.Remove(__instance);
	}

	private static void SObject_checkForAction_Prefix(SObject __instance, bool __1 /* justCheckingForActivity */)
	{
		if (__1)
			IgnoredMachinesForUpdates.Value.Add(__instance);
	}

	private static void SObject_checkForAction_Postfix(SObject __instance, bool __1 /* justCheckingForActivity */)
	{
		if (__1)
			IgnoredMachinesForUpdates.Value.Remove(__instance);
	}
}