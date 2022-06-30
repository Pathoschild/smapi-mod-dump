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
using SpriteMaster.Types;
using SpriteMaster.Types.MemoryCache;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security;

namespace SpriteMaster.Caching;

/// <summary>
/// Used to cache original texture data so it doesn't need to perform blocking fetches as often
/// </summary>
[SuppressUnmanagedCodeSecurity]
internal static class ResidentCache {
	internal static bool Enabled => Config.ResidentCache.Enabled;

	private static readonly AbstractMemoryCache<ulong, byte> Cache = CreateCache();

	private static AbstractMemoryCache<ulong, byte> CreateCache() => AbstractMemoryCache<ulong, byte>.Create(
		name: "ResidentCache",
		maxSize: Config.ResidentCache.MaxSize,
		compressed: true
	);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[]? Get(ulong key) =>
		Cache.Get(key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool TryGet(ulong key, [NotNullWhen(true)] out byte[]? value) =>
		Cache.TryGet(key, out value);

	internal static byte[] Set(ulong key, byte[] value) =>
		Cache.Set(key, value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[]? Remove(ulong key) =>
		Cache.Remove(key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void RemoveFast(ulong key) =>
		Cache.RemoveFast(key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void Purge() {
		Cache.Clear();
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void OnSettingsChanged() {
		if (!Enabled) {
			Purge();
		}
	}
}
