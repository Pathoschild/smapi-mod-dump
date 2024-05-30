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

using Netcode;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Network;

namespace Leclair.Stardew.Common;

public class AdvancedMultipleMutexRequest {

	public static ModSubscriber? Mod { get; internal set; }

	private readonly IModHelper? Helper;
	private readonly int Timeout;

	private double Started;
	private readonly List<NetMutex> AcquiredLocks;
	private readonly List<NetMutex> ReportedLocks;
	private readonly NetMutex[] Mutexes;

	private bool Live;
	private bool Evented;

	private readonly Action? OnSuccess;
	private readonly Action? OnFailure;

	private readonly int ScreenId;

	//private FarmerCollection FC;

	public AdvancedMultipleMutexRequest(IEnumerable<NetMutex> mutexes, Action? onSuccess = null, Action? onFailure = null, IModHelper? helper = null, int timeout = 1000) {
		Helper = helper;
		Timeout = timeout;
		OnSuccess = onSuccess;
		OnFailure = onFailure;
		ScreenId = Context.ScreenId;

		AcquiredLocks = new();
		ReportedLocks = new();
		Mutexes = mutexes is NetMutex[] nms ? nms : mutexes.ToArray();

		// Update each mutex before we get started. This will ensure
		// their previous owner fields are updated correctly.
		var farmers = Game1.getOnlineFarmers();
		foreach (var mutex in Mutexes)
			mutex.Update(farmers);

		RequestLock();
	}

	/// <summary>
	/// Check to see if all our mutexes are locked.
	/// </summary>
	public bool IsLocked() {
		// If we're not in a requested state, or have not acquired all our
		// locks, return false.
		if (!Live || AcquiredLocks.Count < Mutexes.Length)
			return false;

		// Double check with every mutex to ensure we hold the lock.
		foreach (var mutex in Mutexes) {
			if (!mutex.IsLockHeld())
				return false;
		}

		// Finally, maybe, if the master player hasn't gotten uppity, we're locked.
		return true;
	}

	/// <summary>
	/// Request that we attain a lock.
	/// </summary>
	public void RequestLock() {
		if (Live)
			return;

		if (Mutexes.Length == 0) {
			OnSuccess?.Invoke();
			return;
		}

		foreach (var mutex in Mutexes) {
			if (mutex.IsLocked()) {
				OnFailure?.Invoke();
				return;
			}
		}

		Live = true;
		Started = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
		if (Helper is not null && Timeout > 0) {
			Helper.Events.GameLoop.UpdateTicked += OnUpdate;
			Evented = true;
		}

		for (int i = 0; i < Mutexes.Length; i++) {
			NetMutex mutex = Mutexes[i];
			mutex.RequestLock(delegate {
				LockAcquired(mutex);
			}, delegate {
				LockFailed(mutex);
			});
		}
	}

	private void OnUpdate(object? sender, UpdateTickedEventArgs e) {
		if (!Live || ScreenId != Context.ScreenId || ReportedLocks.Count >= Mutexes.Length)
			return;

		// Check to see if we've timed out.
		double delta = Game1.currentGameTime.TotalGameTime.TotalMilliseconds - Started;
		if (delta >= Timeout) {
			LockFinished();
			return;
		}

		// Manually update the mutexes we care about but that we haven't
		// yet received a success/fail for. This may be required for mutexes that
		// aren't being updated normally, which normally only happens for
		// mutexes contained by things in the currentLocation.
		var farmers = Game1.getOnlineFarmers();

		foreach (var mutex in Mutexes)
			if (!AcquiredLocks.Contains(mutex)) {
				mutex.Update(farmers);

				// See if we're held now, but didn't get an update.
				if (mutex.IsLockHeld() && !AcquiredLocks.Contains(mutex)) {
#if DEBUG
					LogLevel level = LogLevel.Debug;
#else
					LogLevel level = LogLevel.Trace;
#endif
					Mod?.Log($"Acquired lock for mutex {mutex.NetFields.GetHashCode()} without receiving delegate call.", level);
					LockAcquired(mutex);
				}
			}
	}

	private void LockAcquired(NetMutex mutex) {
		if (!Live) {
			mutex.ReleaseLock();
			return;
		}

		if (ReportedLocks.Contains(mutex)) {
#if DEBUG
			LogLevel level = LogLevel.Debug;
#else
			LogLevel level = LogLevel.Trace;
#endif
			Mod?.Log($"Acquired lock for mutex {mutex.NetFields.GetHashCode()} that we already received report from.", level);
			return;
		}

		ReportedLocks.Add(mutex);

		if (!AcquiredLocks.Contains(mutex))
			AcquiredLocks.Add(mutex);

		if (ReportedLocks.Count >= Mutexes.Length)
			LockFinished();
	}

	private void LockFailed(NetMutex mutex) {
		if (!Live)
			return;

		if (ReportedLocks.Contains(mutex)) {
#if DEBUG
			LogLevel level = LogLevel.Debug;
#else
			LogLevel level = LogLevel.Trace;
#endif
			Mod?.Log($"Received lock failed for mutex {mutex.NetFields.GetHashCode()} that we already received report from.", level);
			return;
		}

		ReportedLocks.Add(mutex);

		if (ReportedLocks.Count >= Mutexes.Length)
			LockFinished();
	}

	private void LogMutexes() {
		if (Mod is null)
			return;

#if DEBUG
		LogLevel level = LogLevel.Debug;
#else
		LogLevel level = LogLevel.Trace;
#endif

		try {
			List<string[]> states = new();

			foreach (var mutex in Mutexes) {
				var owner = Helper?.Reflection?.GetField<NetLong>(mutex, "owner", false)?.GetValue();
				var request = Helper?.Reflection?.GetField<NetEvent1Field<long, NetLong>>(mutex, "lockRequest", false)?.GetValue();
				object? _inEvents = request is null ? null : Helper!.Reflection!.GetField<object>(request, "incomingEvents", false)?.GetValue();
				object? _outEvents = request is null ? null : Helper!.Reflection!.GetField<object>(request, "outgoingEvents", false)?.GetValue();

				bool acquired = AcquiredLocks.Contains(mutex);
				bool reported = ReportedLocks.Contains(mutex);

				int inEvents = _inEvents is null ? -1 : Helper!.Reflection!.GetProperty<int>(_inEvents, "Count", false)?.GetValue() ?? -1;
				int outEvents = _outEvents is null ? -1 : Helper!.Reflection!.GetProperty<int>(_outEvents, "Count", false)?.GetValue() ?? -1;

				states.Add(new string[] {
							$"{mutex.GetHashCode()}",
							$"{mutex.IsLocked()}",
							$"{mutex.IsLockHeld()}",
							$"{acquired}",
							$"{reported}",
							owner is not null ? $"{owner.Value}" : "---",
							owner is not null ? $"{owner.TargetValue}" : "---",
							$"{inEvents}",
							$"{outEvents}"
						});
			}

			string[] headers = [
				"ID",
						"Locked",
						"LockHeld",
						"Acquired",
						"Reported",
						"Owner",
						"OTarget",
						"inEvents",
						"outEvents"
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
					Mod.Log($"Unable to acquire all mutexes within {Timeout} ms. IsHost: {Game1.IsMasterGame}; Multiplayer: {Context.IsMultiplayer}; Mutex state:", level);
					LogMutexes();

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
		foreach (var mutex in Mutexes)
			mutex.ReleaseLock();

		Live = false;
		ReportedLocks.Clear();
		AcquiredLocks.Clear();

		if (Evented) {
			Helper!.Events.GameLoop.UpdateTicked -= OnUpdate;
			Evented = false;
		}
	}

}

#endif
