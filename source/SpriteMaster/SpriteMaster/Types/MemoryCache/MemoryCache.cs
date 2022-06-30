/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SpriteMaster.Types.MemoryCache;

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

internal class MemoryCache<TKey, TValue> : AbstractMemoryCache<TKey, TValue> where TKey : notnull where TValue : unmanaged {
	private readonly ObjectCache<TKey, TValue[]> UnderlyingCache;

	internal override int Count => UnderlyingCache.Count;

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal override TValue[]? Get(TKey key) {
		return UnderlyingCache.Get(key);
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal override ReadOnlySpan<TValue> GetSpan(TKey key) {
		return UnderlyingCache.Get(key);
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal override bool TryGet(TKey key, [NotNullWhen(true)] out TValue[]? value) {
		return UnderlyingCache.TryGet(key, out value);
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal override bool TryGetSpan(TKey key, out ReadOnlySpan<TValue> value) {
		if (UnderlyingCache.TryGet(key, out var entry)) {
			value = entry;
			return true;
		}

		value = default;
		return false;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal override TValue[] Set(TKey key, TValue[] value) {
		return UnderlyingCache.Set(key, value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal override TValue[]? Update(TKey key, TValue[] value) {
		return UnderlyingCache.Update(key, value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal override ReadOnlySpan<TValue> UpdateSpan(TKey key, TValue[] value) {
		return UnderlyingCache.Update(key, value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal override TValue[]? Remove(TKey key) {
		return UnderlyingCache.Remove(key);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal override void RemoveFast(TKey key) {
		UnderlyingCache.RemoveFast(key);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal override ReadOnlySpan<TValue> RemoveSpan(TKey key) {
		return UnderlyingCache.Remove(key);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal override void Trim(int count) {
		UnderlyingCache.Trim(count);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal override void TrimTo(int count) {
		UnderlyingCache.TrimTo(count);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal override void Clear() {
		UnderlyingCache.Clear();
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal override (ulong Count, ulong Size) ClearWithCount() =>
		UnderlyingCache.ClearWithCount();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void Dispose() {
		if (!OnDispose()) {
			return;
		}

		base.Dispose();

		UnderlyingCache.Dispose();
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private void OnRemove(EvictionReason reason, TKey key, TValue[] element) {
		RemovalCallback?.Invoke(reason, key, element);
	}

	internal MemoryCache(string name, long? maxSize, RemovalCallbackDelegate? removalAction = null) :
		base(name, maxSize ?? long.MaxValue, removalAction) {
		UnderlyingCache = new($"{name} (Underlying)", maxSize, OnRemove);
	}

	public override ulong? OnPurgeHard(IPurgeable.Target target, CancellationToken cancellationToken = default) {
		if (Disposed) {
			return null;
		}

		if (Purging.CompareExchange(true, false)) {
			return null;
		}

		try {
			return UnderlyingCache.OnPurgeHard(target, cancellationToken);
		}
		finally {
			Purging.Value = false;
		}
	}

	public override ulong? OnPurgeSoft(IPurgeable.Target target, CancellationToken cancellationToken = default) {
		if (Disposed) {
			return null;
		}

		if (Purging.CompareExchange(true, false)) {
			return null;
		}

		try {
			return UnderlyingCache.OnPurgeSoft(target, cancellationToken);
		}
		finally {
			Purging.Value = false;
		}
	}
}
