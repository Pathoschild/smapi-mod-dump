/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

using StardewValley.Network;

using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;

namespace Leclair.Stardew.Common;

public class AdvancedMultipleMutexRequest {

	private IModHelper? Helper;
	private int Timeout;

	private double Started;
	private int ReportedCount;
	private List<NetMutex> AcquiredLocks;
	private NetMutex[] Mutexes;

	private bool Live;
	private bool Evented;

	private Action? OnSuccess;
	private Action? OnFailure;

	private FarmerCollection FC;

	public AdvancedMultipleMutexRequest(IEnumerable<NetMutex> mutexes, Action? onSuccess = null, Action? onFailure = null, IModHelper? helper = null, int timeout = 1000) {
		Helper = helper;
		Timeout = timeout;
		OnSuccess = onSuccess;
		OnFailure = onFailure;
		AcquiredLocks = new();
		Mutexes = mutexes is NetMutex[] nms ? nms : mutexes.ToArray();
		FC = new(null);
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
		foreach(var mutex in Mutexes) {
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

		foreach(var mutex in Mutexes) {
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
		if (!Live || ReportedCount >= Mutexes.Length)
			return;

		// Check to see if we've timed out.
		double delta = Game1.currentGameTime.TotalGameTime.TotalMilliseconds - Started;
		if (delta >= Timeout) {
			LockFinished();
			return;
		}

		// Manually update the mutexes we care about but that we haven't
		// yet received a success/fail for. This is required for mutexes that
		// aren't being updated normally, which normally only happens for
		// mutexes contained by things in the currentLocation.
		foreach(var mutex in Mutexes) {
			if (!AcquiredLocks.Contains(mutex))
				mutex.Update(FC);
		}
	}

	private void LockAcquired(NetMutex mutex) {
		if (!Live) {
			mutex.ReleaseLock();
			return;
		}

		if (AcquiredLocks.Contains(mutex))
			return;

		ReportedCount++;
		AcquiredLocks.Add(mutex);
		if (ReportedCount >= Mutexes.Length)
			LockFinished();
	}

	private void LockFailed(NetMutex mutex) {
		if (!Live)
			return;

		ReportedCount++;
		if (ReportedCount >= Mutexes.Length)
			LockFinished();
	}

	private void LockFinished() {
		if (Evented) {
			Helper!.Events.GameLoop.UpdateTicked -= OnUpdate;
			Evented = false;
		}

		if (IsLocked()) {
			OnSuccess?.Invoke();

		} else { 
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
		ReportedCount = 0;
		AcquiredLocks.Clear();

		if (Evented) {
			Helper!.Events.GameLoop.UpdateTicked -= OnUpdate;
			Evented = false;
		}
	}

}
