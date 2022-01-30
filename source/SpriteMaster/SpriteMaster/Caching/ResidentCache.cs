/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SpriteMaster.Caching;

/// <summary>
/// Used to cache original texture data so it doesn't need to perform blocking fetches as often
/// </summary>
[SuppressUnmanagedCodeSecurity]
static class ResidentCache {
	internal static bool Enabled => Config.MemoryCache.Enabled;

	private static readonly SharedLock CacheLock = new();
	private static MemoryCache Cache = CreateCache();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static MemoryCache CreateCache() => Enabled ? new(name: "ResidentCache", config: null) : null!;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T? Get<T>(string key) where T : class => Cache.Get(key) as T;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool TryGet<T>(string key, out T? value) where T : class {
		var result = Get<T>(key);
		value = result;
		return result is not null;
	}

	internal static T? Set<T>(string key, T value) where T : class => (Cache[key] = value) as T;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T? Remove<T>(string key) where T : class => Cache.Remove(key) as T;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static void Purge() {
		if (Cache is null) {
			return;
		}

		using (CacheLock.Write) {
			Cache?.Dispose();
			Cache = CreateCache();
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
