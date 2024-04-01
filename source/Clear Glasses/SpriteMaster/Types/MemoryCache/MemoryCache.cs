/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SpriteMaster.Types.MemoryCache;

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

internal class MemoryCache<TKey, TValue> :
	AbstractMemoryCache<TKey, TValue>
	where TKey : notnull where TValue : unmanaged {
	private readonly ObjectCache<TKey, TValue[]> UnderlyingCache;

	public override long TotalSize => UnderlyingCache.TotalSize;
	public override int Count => UnderlyingCache.Count;

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override TValue[]? Get(TKey key) {
		return UnderlyingCache.Get(key);
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override ReadOnlySpan<TValue> GetSpan(TKey key) {
		return UnderlyingCache.Get(key);
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override bool Contains(TKey key) {
		return UnderlyingCache.Contains(key);
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override bool TryGet(TKey key, [NotNullWhen(true)] out TValue[]? value) {
		return UnderlyingCache.TryGet(key, out value);
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override bool TryGetSpan(TKey key, out ReadOnlySpan<TValue> value) {
		if (UnderlyingCache.TryGet(key, out var entry)) {
			value = entry;
			return true;
		}

		value = default;
		return false;
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override bool TrySetDelegated<TValueGetter>(TKey key, TValueGetter valueGetter) where TValueGetter : struct {
		return UnderlyingCache.TrySetDelegated(key, valueGetter);
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override bool TrySet(TKey key, TValue[] value) {
		return UnderlyingCache.TrySet(key, value);
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override TValue[] Set(TKey key, TValue[] value) {
		Contract.Assert(value is not null);
		return UnderlyingCache.Set(key, value);
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override TValue[] SetOrTouch(TKey key, TValue[] value) {
		Contract.Assert(value is not null);
		return UnderlyingCache.SetOrTouch(key, value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void SetFast(TKey key, TValue[] value) {
		Contract.Assert(value is not null);
		UnderlyingCache.SetFast(key, value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void SetOrTouchFast(TKey key, TValue[] value) {
		Contract.Assert(value is not null);
		UnderlyingCache.SetOrTouchFast(key, value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override TValue[]? Update(TKey key, TValue[] value) {
		Contract.Assert(value is not null);
		return UnderlyingCache.Update(key, value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override ReadOnlySpan<TValue> UpdateSpan(TKey key, TValue[] value) {
		return UnderlyingCache.Update(key, value);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override TValue[]? Remove(TKey key) {
		return UnderlyingCache.Remove(key);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void RemoveFast(TKey key) {
		UnderlyingCache.RemoveFast(key);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override ReadOnlySpan<TValue> RemoveSpan(TKey key) {
		return UnderlyingCache.Remove(key);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void Touch(TKey key) {
		UnderlyingCache.Touch(key);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void Trim(int count) {
		UnderlyingCache.Trim(count);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void TrimTo(int count) {
		UnderlyingCache.TrimTo(count);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void Clear() {
		UnderlyingCache.Clear();
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override (ulong Count, ulong Size) ClearWithCount() =>
		UnderlyingCache.ClearWithCount();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void Dispose() {
		UnderlyingCache.Dispose();
	}

	public override ValueTask DisposeAsync() => UnderlyingCache.DisposeAsync();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private void OnRemove(EvictionReason reason, TKey key, TValue[] element) {
		Contract.Assert(element is not null);
		RemovalCallback?.Invoke(reason, key, element);
	}

	internal MemoryCache(string name, long? maxSize, RemovalCallbackDelegate<TKey, TValue[]>? removalAction = null) :
		base(name, removalAction) {
		UnderlyingCache = new($"{name} (Underlying)", maxSize, OnRemove);
	}

	public override ulong? OnPurgeHard(IPurgeable.Target target, CancellationToken cancellationToken = default) {
		return UnderlyingCache.OnPurgeHard(target, cancellationToken);
	}

	public override ulong? OnPurgeSoft(IPurgeable.Target target, CancellationToken cancellationToken = default) {
		return UnderlyingCache.OnPurgeSoft(target, cancellationToken);
	}

	public override long SizeBytes => UnderlyingCache.SizeBytes;
}
