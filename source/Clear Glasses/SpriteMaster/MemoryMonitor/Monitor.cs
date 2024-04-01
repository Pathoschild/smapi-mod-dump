/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using SpriteMaster.Caching;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;
using ThreadState = System.Threading.ThreadState;

namespace SpriteMaster.MemoryMonitor;

internal sealed class Monitor {
	private readonly Thread MemoryPressureThread;
	private readonly Thread GarbageCollectThread;
	private readonly object CollectLock = new();

	internal Monitor() {
		MemoryPressureThread = new Thread(MemoryPressureLoop) {
			Name = "Memory Pressure Thread",
			Priority = ThreadPriority.BelowNormal,
			IsBackground = true
		};

		GarbageCollectThread = new Thread(GarbageCheckLoop) {
			Name = "Garbage Collection Thread",
			Priority = ThreadPriority.BelowNormal,
			IsBackground = true
		};
	}

	internal void Start() {
		if (MemoryPressureThread.ThreadState == ThreadState.Unstarted) {
			MemoryPressureThread.Start();
		}
		if (GarbageCollectThread.ThreadState == ThreadState.Unstarted) {
			GarbageCollectThread.Start();
		}
	}

	internal void TriggerGarbageCollection() {
		lock (CollectLock) {
			Garbage.Collect(compact: true, blocking: true, background: false);
		}
	}

	internal void TriggerPurge(bool hard, bool soft, bool collect) {
		lock (CollectLock) {
			if (collect) {
				Garbage.Collect(compact: false, blocking: false, background: true);
			}

			long usedMemory = GC.GetTotalMemory(false);
			if (hard) {
				Manager.HardPurge((ulong)usedMemory, 0UL);
			}
			else if (soft) {
				Manager.SoftPurge((ulong)usedMemory, 0UL);
			}

			if (collect) {
				Garbage.Collect(compact: true, blocking: true, background: false);
			}
		}
	}

	[System.Diagnostics.Contracts.Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static ulong ClampedWiden(long value) =>
		(ulong)Math.Max(0L, value);

	private void MemoryPressureLoop() {
		for (; ; ) {
			if (DrawState.TriggerCollection && DrawState.TriggerCollection.Wait()) {
				continue;
			}

			ulong hardRequired = ClampedWiden(Interlocked.Read(ref Config.Garbage.RequiredFreeMemoryHard));
			ulong softRequired = ClampedWiden(Interlocked.Read(ref Config.Garbage.RequiredFreeMemorySoft));

			if (TryPurge<HardPurgeMethod>(hardRequired) || TryPurge<SoftPurgeMethod>(softRequired)) {
				DrawState.TriggerCollection.Set(true);
				Thread.Sleep(2000);
			}
			else {
				Thread.Sleep(512);
			}
		}
	}

	// This is a little wonky, but is done because .NET cannot really inline delegates but it _can_ if they're via a struct. Sorta.
	[MethodImpl(MethodImplOptions.NoInlining)]
	private bool TryPurge<TPurge>(ulong target, TPurge callback = default) where TPurge : struct, IPurgeMethod {
		lock (CollectLock) {
			ulong targetMegabytes = target / SizesExt<ulong>.MiB;

			targetMegabytes = Math.Clamp(targetMegabytes, 1, int.MaxValue);

			try {
				using var _ = new MemoryFailPoint((int)targetMegabytes);
			}
			catch (InsufficientMemoryException) {
				ulong usedMemory = Math.Max(target, ClampedWiden(GC.GetTotalMemory(false)));
				ulong newTarget = usedMemory - target;

				Debug.Warning($"Less than {target.AsDataSize(decimals: 0)} available for block allocation, purging");

				Garbage.Collect(compact: false, blocking: false, background: true);

				_ = callback.Invoke(usedMemory, newTarget);

				Garbage.Collect(compact: true, blocking: true, background: false);

				return true;
			}
		}

		return false;
	}

	private interface IPurgeMethod {
		ulong Invoke(ulong current, ulong target);
	}

	private readonly struct HardPurgeMethod : IPurgeMethod {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ulong Invoke(ulong current, ulong target) {
			return Manager.SoftPurge(current, target);
		}
	}

	private readonly struct SoftPurgeMethod : IPurgeMethod {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ulong Invoke(ulong current, ulong target) {
			return Manager.SoftPurge(current, target);
		}
	}

	private void GarbageCheckLoop() {
		try {
			while (true) {
				try {
					GC.RegisterForFullGCNotification(10, 10);
					GC.WaitForFullGCApproach();
					if (Garbage.ManualCollection) {
						Thread.Sleep(128);
						continue;
					}

					if (DrawState.TriggerCollection && DrawState.TriggerCollection.Wait()) {
						continue;
					}

					lock (CollectLock) {
						ResidentCache.Purge();
					}
				}
				catch (ThreadInterruptedException) {
					break;
				}
				catch {
					// Ignore Exceptions
				}
			}
		}
		catch {
			// Ignore Exceptions
		}
	}
}
