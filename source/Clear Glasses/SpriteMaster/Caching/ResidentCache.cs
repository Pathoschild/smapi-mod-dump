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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SpriteMaster.Caching;

/// <summary>
/// Used to cache original texture data so it doesn't need to perform blocking fetches as often
/// </summary>
internal static class ResidentCache {
	internal static bool Enabled => Config.ResidentCache.Enabled;

	private static IMemoryCache<ulong, byte> Cache = CreateCache();

	internal static long Size => Cache.SizeBytes;

	private static IMemoryCache<ulong, byte> CreateCache() => AbstractMemoryCache<ulong, byte>.Create(
		name: "ResidentCache",
		maxSize: Config.ResidentCache.MaxSize,
		compressed: Config.ResidentCache.Compress != Compression.Algorithm.None
	);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool Contains(ulong key) =>
		Cache.Contains(key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[]? Get(ulong key) =>
		Cache.Get(key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool TryGet(ulong key, [NotNullWhen(true)] out byte[]? value) =>
		Cache.TryGet(key, out value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool TrySet(ulong key, byte[] value) =>
		Cache.TrySet(key, value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void SetFast(ulong key, byte[] value) =>
		Cache.SetFast(key, value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void SetOrTouchFast(ulong key, byte[] value) =>
		Cache.SetOrTouchFast(key, value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[]? Remove(ulong key) =>
		Cache.Remove(key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void RemoveFast(ulong key) =>
		Cache.RemoveFast(key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void Touch(ulong key) =>
		Cache.Touch(key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void Purge() {
		Cache.Clear();
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void OnSettingsChanged() {
		if (!Enabled) {
			Purge();
		}

		bool isCompressed = Cache is ICompressedMemoryCache;
		bool shouldCompress = Config.ResidentCache.Compress != Compression.Algorithm.None;
		if (isCompressed != shouldCompress) {
			var newCache = CreateCache();
			var oldCache = Interlocked.Exchange(ref Cache, newCache);
			oldCache.Dispose();
		}
	}
}
