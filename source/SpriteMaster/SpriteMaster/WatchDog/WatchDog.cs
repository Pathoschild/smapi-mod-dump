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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace SpriteMaster.WatchDog;

// WatchDog only interrupts in non-debug builds; in debug builds, if we encounter a deadlock, I want to know about it.

static class WatchDog {
	private static Thread? WatchdogThread = null;

	private static readonly object WatchedThreadsLock = new();
	private static readonly List<Thread> WatchedThreads = new();

	private static readonly Dictionary<Thread, bool> ThreadWorkingState = new();
	private static readonly Dictionary<Thread, long> LastTickMap = new();

	internal ref struct WorkingStateCookie {
		public WorkingStateCookie() => WatchDog.SetWorkingState(true);

		internal void Dispose() => WatchDog.SetWorkingState(false);
	}

	internal static WorkingStateCookie ScopedWorkingState => new();

	internal static void Initialize() {
		if (!Config.WatchDog.Enabled) {
			return;
		}

		if (WatchdogThread is not null) {
			Debug.Warning("Tried to initialize watchdog, but it is already running");
			return;
		}

		WatchdogThread = new Thread(WatchdogRun);
		WatchdogThread.Start();
	}

	internal static void Tick() {
		if (!Config.WatchDog.Enabled) {
			return;
		}

		var thread = Thread.CurrentThread;
		var timestamp = Stopwatch.GetTimestamp();

		lock (WatchedThreadsLock) {
			if (LastTickMap.TryAdd(thread, timestamp)) {
				WatchedThreads.Add(thread);
				ThreadWorkingState.Add(thread, true);
			}
			else {
				LastTickMap[thread] = timestamp;
			}
		}
	}

	internal static void SetWorkingState(bool state) {
		if (!Config.WatchDog.Enabled) {
			return;
		}

		Tick();
		ThreadWorkingState[Thread.CurrentThread] = state;
	}

	/*
#pragma warning disable CS0618 // Type or member is obsolete
	private readonly ref struct SuspendCookie {
		readonly Thread Thread;
		readonly bool WasSuspended;

		public SuspendCookie(Thread thread) {
			Thread = thread;
			WasSuspended = thread.ThreadState is (System.Threading.ThreadState.Suspended or System.Threading.ThreadState.SuspendRequested);
			thread.Suspend();
		}

		internal void Dispose() {
			if (!WasSuspended) {
				Thread.Resume();
			}
		}
	}
#pragma warning restore CS0618 // Type or member is obsolete
	*/

	private static void HandleSpinningThread(Thread thread) {
		Debug.Warning($"Spinning Thread detected: '{thread.Name}' :: {thread.ManagedThreadId}");
	}

	private static bool CheckThread(Thread thread, long currentTimestamp) {
		// If the thread is not alive, then there's no reason to monitor it.
		if (!thread.IsAlive || thread == Thread.CurrentThread) {
			WatchedThreads.Remove(thread);
			LastTickMap.Remove(thread);
			ThreadWorkingState.Remove(thread);
			return true;
		}

		// If the thread is not in a working state (and thus is likely pending operations)
		// then it should be skipped. Deadlocks happen in working states.
		if (!ThreadWorkingState[thread]) {
			return true;
		}

		// Otherwise, check if the last tick was longer than the interrupt interval ago
		var threadTimestamp = LastTickMap[thread];
		var difference = currentTimestamp - threadTimestamp;
		if (difference >= Config.WatchDog.InterruptInterval) {
			// Check if the thread is actually waiting on a lock.
			if (thread.ThreadState != System.Threading.ThreadState.WaitSleepJoin) {
				// This means that a thread is spinning, most likely. We cannot interrupt it, but we can warn about it.
				HandleSpinningThread(thread);
				return true;
			}

#if DEBUG
			Debug.Warning($"Frozen thread detected: watchdog is breaking...");
			Debugger.Break();
#else
			// We need to interrupt this thread. Not _safe_, but prevents freezing.
			Debug.Warning($"Frozen thread detected: watchdog is interrupting: {thread.ManagedThreadId} {thread.Name}");
			//thread.Interrupt();
			LastTickMap[thread] = currentTimestamp;
#endif
			return false;
		}
		return true;
	}

	private static void WatchdogRun() {
		bool wasInterrupted = false;
		while (true) {
			Thread.Sleep(wasInterrupted ? Config.WatchDog.ShortSleepInterval : Config.WatchDog.DefaultSleepInterval);
			wasInterrupted = false;
			var timestamp = Stopwatch.GetTimestamp();
			lock (WatchedThreadsLock) {
				// We only interrupt a single thread at a time, as releasing it might allow other threads to move past a deadlock on their own.
				// We shorten the interval time to allow them to catch up.
				foreach (var thread in WatchedThreads) {
					try {
						wasInterrupted = !CheckThread(thread, timestamp);
						if (wasInterrupted) {
							break;
						}
					}
					catch (Exception ex) {
						Debug.Warning(ex);
					}
				}
			}
		}
	}
}
