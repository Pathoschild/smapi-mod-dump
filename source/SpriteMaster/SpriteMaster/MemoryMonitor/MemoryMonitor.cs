/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Caching;
using SpriteMaster.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpriteMaster;

class MemoryMonitor {
	private readonly Thread MemoryPressureThread;
	private readonly Thread GarbageCollectThread;
	private readonly object CollectLock = new();

	internal MemoryMonitor() {
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

	internal void TriggerGC() {
		lock (CollectLock) {
			Garbage.Collect(compact: true, blocking: true, background: false);
			DrawState.TriggerGC.Set(true);
		}
	}

	internal void TriggerPurge() {
		lock (CollectLock) {
			Garbage.Collect(compact: true, blocking: true, background: false);
			ResidentCache.Purge();
			Garbage.Collect(compact: true, blocking: true, background: false);
			DrawState.TriggerGC.Set(true);
		}
	}

	private void MemoryPressureLoop() {
		for (; ; ) {
			if (DrawState.TriggerGC && DrawState.TriggerGC.Wait()) {
				continue;
			}

			lock (CollectLock!) {
				try {
					using var _ = new MemoryFailPoint(Config.Garbage.RequiredFreeMemory);
				}
				catch (InsufficientMemoryException) {
					Debug.Warning($"Less than {(Config.Garbage.RequiredFreeMemory * 1024 * 1024).AsDataSize(decimals: 0)} available for block allocation, forcing full garbage collection");
					ResidentCache.Purge();
					SuspendedSpriteCache.Purge();
					DrawState.TriggerGC.Set(true);
					Thread.Sleep(10000);
				}
			}
			Thread.Sleep(512);
		}
	}

	private void GarbageCheckLoop() {
		try {
			for (; ; ) {
				GC.RegisterForFullGCNotification(10, 10);
				GC.WaitForFullGCApproach();
				if (Garbage.ManualCollection) {
					Thread.Sleep(128);
					continue;
				}
				lock (CollectLock!) {
					if (DrawState.TriggerGC && DrawState.TriggerGC.Wait()) {
						continue;
					}

					ResidentCache.Purge();
					DrawState.TriggerGC.Set(true);
					// TODO : Do other cleanup attempts here.
				}
			}
		}
		catch {

		}
	}
}
