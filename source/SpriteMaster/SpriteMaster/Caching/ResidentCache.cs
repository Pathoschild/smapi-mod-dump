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
using System.Runtime.CompilerServices;
using System.Security;

namespace SpriteMaster.Caching;

/// <summary>
/// Used to cache original texture data so it doesn't need to perform blocking fetches as often
/// </summary>
[SuppressUnmanagedCodeSecurity]
static class ResidentCache {
	internal static bool Enabled => Config.MemoryCache.Enabled;

	private static readonly SharedLock CacheLock = new();
	private static TypedMemoryCache<byte[]> Cache = CreateCache();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static TypedMemoryCache<byte[]> CreateCache() => Enabled ? new(name: "ResidentCache") : null!;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte[]? Get(string key) => Cache.Get(key);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool TryGet(string key, out byte[]? value) {
		var result = Get(key);
		value = result;
		return result is not null;
	}

	internal static byte[]? Set(string key, byte[] value) => Cache.Set(key, value);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte[]? Remove(string key) => Cache.Remove(key);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Purge() {
		if (Cache is null) {
			return;
		}

		using (CacheLock.Write) {
			Cache?.Clear();
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void OnSettingsChanged() {
		using (CacheLock.Write) {
			if (Enabled) {
				if (Cache is null) {
					Cache = CreateCache();
				}
			}
			else {
				Purge();
			}
		}
	}
}
