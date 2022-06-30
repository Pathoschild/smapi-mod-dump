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
using SpriteMaster.Extensions;
using SpriteMaster.Types;
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
	private static readonly Condition TrimEvent = new();
	private static readonly Thread CacheTrimThread = ThreadExt.Run(CacheTrimLoop, background: true, name: "SuspendedSpriteCache Trim Thread");

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static int Percent75(int value) {
		return (value >> 1) + (value >> 2);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static long Percent75(long value) {
		return (value >> 1) + (value >> 2);
	}

	private static void TrimCount() {
		if (Config.SuspendedCache.MaxCacheCount is <= 0 or int.MaxValue) {
			return;
		}

		int currentCacheCount = Cache.Count;

		if (currentCacheCount <= Config.SuspendedCache.MaxCacheCount) {
			return;
		}

		var goal = Percent75(Config.SuspendedCache.MaxCacheCount);

		Debug.Trace($"Trimming (Count) SuspendedSpriteCache: {currentCacheCount} -> {goal}");
		Cache.TrimTo(goal);
	}

	[DoesNotReturn]
	private static void CacheTrimLoop() {
		while (true) {
			TrimEvent.Wait();

			TrimCount();
		}
		// ReSharper disable once FunctionNeverReturns
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static void OnEntryRemoved(EvictionReason reason, ulong key, ManagedSpriteInstance element) {
		element.Dispose();
	}

	internal static void Add(ManagedSpriteInstance instance) {
		if (!Config.SuspendedCache.Enabled) {
			instance.Dispose();
			return;
		}

		Cache.Set(instance.Hash, instance);
		Debug.Trace($"SuspendedSpriteCache Size: {Cache.Count.ToString(DrawingColor.LightCoral)}");

		if (Cache.Count > Config.SuspendedCache.MaxCacheCount) {
			TrimEvent.Set();
		}
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

	internal static bool Remove(ManagedSpriteInstance instance) => Remove(instance.Hash);

	internal static void RemoveFast(ManagedSpriteInstance instance) => RemoveFast(instance.Hash);

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
