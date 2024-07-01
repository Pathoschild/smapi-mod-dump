/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if COMMON_MUTEX

using System;
using System.Collections.Generic;
using System.Linq;

using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Inventory;

using Netcode;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Network;

namespace Leclair.Stardew.Common;

public class MultipleEventedInventoryExclusiveRequest {

	public static ModSubscriber? Mod { get; internal set; }

	private readonly IModHelper? Helper;
	private readonly int Timeout;

	private double Started;
	private readonly List<IBCInventory> Acquired;
	private readonly List<IBCInventory> Reported;
	private readonly IBCInventory[] Inventories;

	private bool Live;
	private bool Evented;

	private readonly Action? OnSuccess;
	private readonly Action? OnFailure;

	private readonly int ScreenId;

	public MultipleEventedInventoryExclusiveRequest(
		IEnumerable<IBCInventory> inventories,
		Action? onSuccess = null,
		Action? onFailure = null,
		IModHelper? helper = null,
		int timeout = 5000
	) {
		Helper = helper;
		Timeout = timeout;
		OnSuccess = onSuccess;
		OnFailure = onFailure;
		ScreenId = Context.ScreenId;

		Acquired = new();
		Reported = new();
		Inventories = inventories is IBCInventory[] ivs ? ivs : inventories.ToArray();
	}

	/// <summary>
	/// Check to see if all our inventories are exclusive.
	/// </summary>
	public bool IsLocked() {
		// If we're not in a requested state, or have not acquired all our
		// locks, return false.
		if (!Live || Acquired.Count < Inventories.Length)
			return false;

		// Finally, maybe, if the master player hasn't gotten uppity, we're locked.
		return true;
	}

	/// <summary>
	/// Request that we attain a lock.
	/// </summary>
	public void RequestLock() {
		if (Live)
			return;

		if (Inventories.Length == 0) {
			OnSuccess?.Invoke();
			return;
		}

		Live = true;
		Started = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
		bool waiting = false;

		for (int i = 0; i < Inventories.Length; i++) {
			var inv = Inventories[i];
			if (inv is WorkingInventory win && win.Provider is IEventedInventoryProvider eip) {
				bool? result;
				try {
					result = eip.StartExclusive(inv.Object, inv.Location, inv.Player, delegate (bool success) {
						if (success)
							LockAcquired(inv);
						else
							LockFailed(inv);
					});
				} catch(Exception ex) {
					Mod?.Log($"Error calling StartExclusive for inventory {inv.Object.GetHashCode()}: {ex}", LogLevel.Error);
					result = false;
				}

				if (result.HasValue) {
					if (result.Value)
						LockAcquired(inv);
					else
						LockFailed(inv);

				} else
					waiting = true;

			} else
				// We should never have an inventory that isn't
				// a WorkingInventory.
				LockFailed(inv);
		}

		if (waiting && Helper is not null && Timeout > 0) {
			Helper.Events.GameLoop.UpdateTicked += OnUpdate;
			Evented = true;
		}
	}

	private void OnUpdate(object? sender, UpdateTickedEventArgs e) {
		if (!Live || ScreenId != Context.ScreenId || Reported.Count >= Inventories.Length)
			return;

		// Check to see if we've timed out.
		double delta = Game1.currentGameTime.TotalGameTime.TotalMilliseconds - Started;
		if (delta >= Timeout) {
			LockFinished();
			return;
		}
	}

	private void LockAcquired(IBCInventory inv) {
		if (!Live) {
			if (inv is WorkingInventory win && win.Provider is IEventedInventoryProvider eip)
				eip.EndExclusive(inv.Object, inv.Location, inv.Player);
			return;
		}

		if (Reported.Contains(inv)) {
#if DEBUG
			LogLevel level = LogLevel.Debug;
#else
			LogLevel level = LogLevel.Trace;
#endif
			Mod?.Log($"Acquired lock for inventory {inv.Object.GetHashCode()} that we already received report from.", level);
			return;
		}

		Reported.Add(inv);

		if (!Acquired.Contains(inv))
			Acquired.Add(inv);

		if (Reported.Count >= Inventories.Length)
			LockFinished();
	}

	private void LockFailed(IBCInventory inv) {
		if (!Live)
			return;

		if (Reported.Contains(inv)) {
#if DEBUG
			LogLevel level = LogLevel.Debug;
#else
			LogLevel level = LogLevel.Trace;
#endif
			Mod?.Log($"Received lock failed for inventory {inv.Object.GetHashCode()} that we already received report from.", level);
			return;
		}

		Reported.Add(inv);

		if (Reported.Count >= Inventories.Length)
			LockFinished();
	}

	private void LogInventories() {
		if (Mod is null)
			return;

#if DEBUG
		LogLevel level = LogLevel.Debug;
#else
		LogLevel level = LogLevel.Trace;
#endif

		try {
			List<string[]> states = new();

			foreach (var inv in Inventories) {
				bool acquired = Acquired.Contains(inv);
				bool reported = Reported.Contains(inv);

				states.Add([
					$"{inv.Object.GetHashCode()}",
					$"{inv.Object.GetType().FullName}",
					$"{acquired}",
					$"{reported}"
				]);
			}

			string[] headers = [
				"ID",
				"Type",
				"Acquired",
				"Reported",
			];

			Mod.LogTable(headers, states, level);

		} catch (Exception) {
			/* do nothing */
		}
	}

	private void LockFinished() {
		if (Evented) {
			Helper!.Events.GameLoop.UpdateTicked -= OnUpdate;
			Evented = false;
		}

		if (IsLocked()) {
			OnSuccess?.Invoke();

		} else {
			if (Mod != null) {
				try {
#if DEBUG
					LogLevel level = LogLevel.Debug;
#else
					LogLevel level = LogLevel.Trace;
#endif
					Mod.Log($"Unable to acquire all inventories within {Timeout} ms. IsHost: {Game1.IsMasterGame}; Multiplayer: {Context.IsMultiplayer}; State:", level);
					LogInventories();

				} catch (Exception) {
					/* do nothing */
				}
			}

			ReleaseLock();
			OnFailure?.Invoke();
		}
	}

	/// <summary>
	/// Release our lock.
	/// </summary>
	public void ReleaseLock() {
		foreach (var inv in Inventories)
			if (inv is WorkingInventory win && win.Provider is IEventedInventoryProvider eip)
				eip.EndExclusive(inv.Object, inv.Location, inv.Player);

		Live = false;
		Reported.Clear();
		Acquired.Clear();

		if (Evented) {
			Helper!.Events.GameLoop.UpdateTicked -= OnUpdate;
			Evented = false;
		}
	}

}

#endif
