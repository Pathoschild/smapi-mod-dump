/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using SpriteMaster.Types.Interlocking;
using System;
using System.Collections.Generic;

namespace SpriteMaster.MemoryMonitor;
internal sealed class Manager {
	private static readonly InterlockedBool Purging = false;
	private static IPurgeable[] PurgeablesArrayInternal = Array.Empty<IPurgeable>();
	private static readonly HashSet<IPurgeable> Purgeables = new();

	private static ReadOnlySpan<IPurgeable> LockedPurgeables {
		get {
			lock (Purgeables) {
				Purgeables.CopyTo(PurgeablesArrayInternal);
				return PurgeablesArrayInternal.AsSpan(0, Purgeables.Count);
			}
		}
	}

	internal static bool Register(IPurgeable purgeable) {
		lock (Purgeables) {
			if (Purgeables.Add(purgeable)) {
				Array.Resize(ref PurgeablesArrayInternal, Purgeables.Count);
				return true;
			}

			return false;
		}
	}

	internal static bool Unregister(IPurgeable purgeable) {
		lock (Purgeables) {
			return Purgeables.Remove(purgeable);
		}
	}

	private delegate ulong? PurgeDelegate(IPurgeable @this, IPurgeable.Target target);

	private static ulong Purge(ulong currentUsage, ulong targetUsage, PurgeDelegate purgeMethod) {
		if (Purging.CompareExchange(true, false)) {
			return 0UL;
		}

		ulong purged = 0UL;

		try {
			foreach (var purgeable in LockedPurgeables) {
				if (purgeMethod(purgeable, new() {CurrentMemoryUsage = currentUsage, TargetMemoryUsage = targetUsage}) is { } purgedBytes) {
					purged += purgedBytes;
				};
			}
		}
		finally {
			Purging.Value = false;
		}

		return purged;
	}

	internal static ulong HardPurge(ulong currentUsage, ulong targetUsage) {
		var result = Purge(currentUsage, targetUsage, (obj, target) => obj.OnPurgeHard(target));
		Metadata.Metadata.Purge();
		return result;
	}

	internal static ulong SoftPurge(ulong currentUsage, ulong targetUsage) {
		return Purge(currentUsage, targetUsage, (obj, target) => obj.OnPurgeSoft(target));
	}
}
