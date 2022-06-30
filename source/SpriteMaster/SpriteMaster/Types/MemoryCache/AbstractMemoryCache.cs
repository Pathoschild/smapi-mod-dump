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
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types.MemoryCache;

internal abstract class AbstractMemoryCache<TKey, TValue> : AbstractObjectCache<TKey, TValue[]> where TKey : notnull where TValue : unmanaged {
	internal AbstractMemoryCache(string name, long maxSize, RemovalCallbackDelegate? removalAction = null) :
		base(name, maxSize, removalAction) {
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal virtual ReadOnlySpan<TValue> GetSpan(TKey key) =>
		Get(key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal virtual bool TryGetSpan(TKey key, out ReadOnlySpan<TValue> value) {
		if (TryGet(key, out var val)) {
			value = val;
			return true;
		}
		value = default;
		return false;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal virtual ReadOnlySpan<TValue> UpdateSpan(TKey key, TValue[] value) =>
		Update(key, value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal virtual ReadOnlySpan<TValue> RemoveSpan(TKey key) =>
		Remove(key);

	internal static AbstractMemoryCache<TKey, TValue> Create(string name, RemovalCallbackDelegate? removalAction = null, long? maxSize = null, long? maxCount = null, bool compressed = false) {
		if (compressed) {
			return new CompressedMemoryCache<TKey, TValue>(name, maxSize, removalAction);
		}
		else {
			return new MemoryCache<TKey, TValue>(name, maxSize, removalAction);
		}
	}
}
