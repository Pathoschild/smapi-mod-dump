/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Types.MemoryCache;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SpriteMaster.Caching;

[SMAPIConsole.Stats("suspended-sprite-cache")]
internal static class SuspendedSpriteCache {
	private static readonly AbstractObjectCache<ulong, ManagedSpriteInstance> Cache = new ObjectCache<ulong, ManagedSpriteInstance>(
		name: "SuspendedSpriteCache",
		maxSize: Config.SuspendedCache.MaxCacheSize,
		removalAction: OnEntryRemoved
	);

	internal static long Size => Cache.SizeBytes;

	private static readonly ThreadLocal<bool> Resurrecting = new(false);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static void OnEntryRemoved(EvictionReason reason, ulong key, ManagedSpriteInstance element) {
		if (!Resurrecting.Value) {

			element?.DisposeSuspended();
		}
	}

	internal static void Add(ManagedSpriteInstance instance) {
		if (!Config.SuspendedCache.Enabled) {
			instance.Dispose();
			return;
		}

		Cache.Set(instance.Hash, instance);
		Debug.Trace($"SuspendedSpriteCache Size: {Cache.Count.ToString(DrawingColor.LightCoral)}");
	}

	internal static ManagedSpriteInstance? Fetch(ulong hash) {
		if (!Config.SuspendedCache.Enabled) {
			return null;
		}

		return Cache.Get(hash);
	}

	internal static bool TryFetch(ulong hash, [NotNullWhen(true)] out ManagedSpriteInstance? instance) {
		instance = Fetch(hash);
		return instance is not null;
	}

	internal static bool Remove(ulong hash) {
		var element = Cache.Remove(hash);
		return element is not null;
	}

	internal static void RemoveFast(ulong hash) {
		Cache.RemoveFast(hash);
	}

	internal static bool Resurrect(ulong hash) {
		Resurrecting.Value = true;
		try {
			var element = Cache.Remove(hash);
			return element is not null;
		}
		finally {
			Resurrecting.Value = false;
		}
	}

	internal static void ResurrectFast(ulong hash) {
		Resurrecting.Value = true;
		try {
			Cache.RemoveFast(hash);
		}
		finally {
			Resurrecting.Value = false;
		}
	}

	internal static bool Remove(ManagedSpriteInstance instance) => Remove(instance.Hash);

	internal static void RemoveFast(ManagedSpriteInstance instance) => RemoveFast(instance.Hash);

	internal static void Resurrect(ManagedSpriteInstance instance) => ResurrectFast(instance.Hash); 

	internal static void Purge() {
		Cache.Clear();
	}

	[SMAPIConsole.StatsMethod]
	internal static string[] DumpStats() {
		var statsLines = new List<string> {
			$"\tTotal Suspended Elements: {Cache.Count}",
			$"\tTotal Memory Size       : {Cache.TotalSize.AsDataSize()}"
		};
		return statsLines.ToArray();
	}
}
