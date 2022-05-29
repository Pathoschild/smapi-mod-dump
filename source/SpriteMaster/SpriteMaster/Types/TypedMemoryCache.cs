/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Runtime.Caching;

namespace SpriteMaster.Types;

internal class TypedMemoryCache<T> where T : class {
	internal delegate void RemovalCallbackDelegate(CacheEntryRemovedReason reason, T element);

	private MemoryCache Cache;
	private readonly RemovalCallbackDelegate? RemovalCallback = null;

	private void OnEntryRemoved(CacheEntryRemovedArguments args) {
		var element = args.CacheItem?.Value as T;
		if (element is null) {
			return;
		}
		RemovalCallback!(args.RemovedReason, element);
	}

	internal TypedMemoryCache(string name, RemovalCallbackDelegate? removalAction = null) {
		Cache = new(name);
		RemovalCallback = removalAction;
	}

	internal long Count => Cache.GetCount();

	internal T? Get(string key) => Cache.Get(key) as T;

	internal T Set(string key, T value) {
		if (RemovalCallback is null) {
			Cache[key] = value;
		}
		else {
			Cache.Set(key, value, new CacheItemPolicy { RemovedCallback = OnEntryRemoved });
		}
		return value;
	}

	internal T? Update(string key, T value) {
		// TODO : threadsafety
		T? original = null;
		if (Cache.Get(key) is T current) {
			original = current;
		}
		Set(key, value);
		return original;
	}

	internal T? Remove(string key) => Cache.Remove(key) as T;

	internal long Trim(int percent) => Cache.Trim(percent);

	internal void Clear() {
		var name = Cache.Name;
		Cache.Dispose();
		Cache = new(name);
	}
}
