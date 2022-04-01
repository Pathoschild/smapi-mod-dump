/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Caching;
using System.Threading;

namespace SpriteMaster;

[SMAPIConsole.Stats("suspended-sprite-cache")]
static class SuspendedSpriteCache {
	private static long TotalCachedSize = 0L;
	private static readonly TypedMemoryCache<ManagedSpriteInstance> Cache = new("SuspendedSpriteCache", OnEntryRemoved);
	private static readonly Condition TrimEvent = new();
	private static readonly Thread CacheTrimThread = ThreadExt.Run(CacheTrimLoop, background: true, name: "Cache Trim Thread");

	private static void TrimSize() {
		if (Config.SuspendedCache.MaxCacheSize <= 0 || Config.SuspendedCache.MaxCacheSize == long.MaxValue) {
			return;
		}

		int percentageOffset = 0;
		var totalCachedSize = Interlocked.Read(ref TotalCachedSize);
		while (totalCachedSize > Config.SuspendedCache.MaxCacheSize) {

			// How much is needed to be reduced, with an additional safety of 25% or so?
			var goal = (Config.SuspendedCache.MaxCacheSize * 0.75);
			var multiplier = goal / totalCachedSize;
			var percentageF = 1.0 - multiplier;
			var percentageI = Math.Clamp((int)Math.Round(percentageF * 100.0), 1, 100);

			Debug.Trace($"Trimming (Size) SuspendedSpriteCache: {percentageI}%, from {totalCachedSize.AsDataSize()} to {((long)Math.Round(goal)).AsDataSize()}");
			Cache.Trim(percentageI);

			var currentTotalCachedSize = Interlocked.Read(ref TotalCachedSize);
			if (currentTotalCachedSize == totalCachedSize) {
				// We didn't trim anything...
				++percentageOffset;
				if (percentageI == 100 || percentageOffset == 100) {
					Debug.Info($"Trimming (Size) SuspendedSprite Cache: Failed to trim, nothing trimmed at 100% (report to {"Ameisen".Colorized(DrawingColor.Red)})");
					return;
				}
			}
			totalCachedSize = currentTotalCachedSize;
		}
	}

	private static void TrimCount() {
		if (Config.SuspendedCache.MaxCacheCount <= 0 || Config.SuspendedCache.MaxCacheCount == long.MaxValue) {
			return;
		}

		int percentageOffset = 0;
		long totalCachedCount = Cache.Count;
		while (totalCachedCount > Config.SuspendedCache.MaxCacheCount) {
			// How much is needed to be reduced, with an additional safety of 25% or so?
			var goal = (Config.SuspendedCache.MaxCacheCount * 0.75);
			var multiplier = goal / totalCachedCount;
			var percentageF = 1.0 - multiplier;
			var percentageI = Math.Clamp((int)Math.Round(percentageF * 100.0) + percentageOffset, 1, 100);

			Debug.Trace($"Trimming (Count) SuspendedSpriteCache: {percentageI}%, from {totalCachedCount} to {((long)Math.Round(goal))}");
			Cache.Trim(percentageI);
			var currentTotalCachedCount = Cache.Count;
			if (currentTotalCachedCount == totalCachedCount) {
				// We didn't trim anything...
				++percentageOffset;
				if (percentageI == 100 || percentageOffset == 100) {
					Debug.Info($"Trimming (Count) SuspendedSprite Cache: Failed to trim, nothing trimmed at 100% (report to {"Ameisen".Colorized(DrawingColor.Red)})");
					return;
				}
			}
			totalCachedCount = currentTotalCachedCount;
		}
	}

	private static void CacheTrimLoop() {
		while (true) {
			TrimEvent.Wait();

			TrimSize();
			TrimCount();
		}
	}

	private static void OnEntryRemoved(CacheEntryRemovedReason reason, ManagedSpriteInstance element) {
		Interlocked.Add(ref TotalCachedSize, -element.MemorySize);
		element.Dispose();
	}

	internal static void Add(ManagedSpriteInstance instance) {
		if (!Config.SuspendedCache.Enabled) {
			instance.Dispose();
			return;
		}

		var key = instance.Hash.ToString64();
		Cache.Set(key, instance);
		Interlocked.Add(ref TotalCachedSize, instance.MemorySize);
		Debug.Trace($"SuspendedSpriteCache Size: {Cache.Count.ToString(System.Drawing.Color.LightCoral)}");

		if (Interlocked.Read(ref TotalCachedSize) > Config.SuspendedCache.MaxCacheSize) {
			TrimEvent.Set();
		}
	}

	internal static ManagedSpriteInstance? Fetch(ulong hash) {
		if (!Config.SuspendedCache.Enabled) {
			return null;
		}

		var key = hash.ToString64();
		return Cache.Get(key);
	}

	internal static bool TryFetch(ulong hash, [NotNullWhen(true)] out ManagedSpriteInstance? instance) {
		instance = Fetch(hash);
		return instance is not null;
	}

	internal static bool Remove(ulong hash) {
		var element = Cache.Remove(hash.ToString64());
		if (element is not null) {
			return true;
		}
		return false;
	}

	internal static bool Remove(ManagedSpriteInstance instance) => Remove(instance.Hash);

	internal static void Purge() {
		Cache.Clear();
	}

	[SMAPIConsole.StatsMethod]
	internal static string[] DumpStats() {
		var statsLines = new List<string>();
		statsLines.Add($"\tTotal Suspended Elements: {Cache.Count}");
		statsLines.Add($"\tTotal Memory Size       : {Interlocked.Read(ref TotalCachedSize).AsDataSize()}");
		return statsLines.ToArray();
	}
}
