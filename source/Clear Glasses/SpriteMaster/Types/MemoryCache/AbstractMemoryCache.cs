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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SpriteMaster.Types.MemoryCache;

internal abstract class AbstractMemoryCache<TKey, TValue> :
	IMemoryCache<TKey, TValue>
	where TKey : notnull where TValue : unmanaged {

	protected readonly RemovalCallbackDelegate<TKey, TValue[]>? RemovalCallback = null;
	public string Name { get; }

	internal AbstractMemoryCache(string name, RemovalCallbackDelegate<TKey, TValue[]>? removalAction = null) {
		RemovalCallback = removalAction;
		Name = name;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public virtual ReadOnlySpan<TValue> GetSpan(TKey key) =>
		Get(key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public virtual bool TryGetSpan(TKey key, out ReadOnlySpan<TValue> value) {
		if (TryGet(key, out var val)) {
			value = val;
			return true;
		}
		value = default;
		return false;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public virtual ReadOnlySpan<TValue> UpdateSpan(TKey key, TValue[] value) =>
		Update(key, value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public virtual ReadOnlySpan<TValue> RemoveSpan(TKey key) =>
		Remove(key);

	internal static IMemoryCache<TKey, TValue> Create(string name, RemovalCallbackDelegate<TKey, TValue[]>? removalAction = null, long? maxSize = null, long? maxCount = null, bool compressed = false) {
		if (compressed) {
			return new CompressedMemoryCache<TKey, TValue>(name, maxSize, removalAction);
		}
		else {
			return new MemoryCache<TKey, TValue>(name, maxSize, removalAction);
		}
	}

	public abstract void Dispose();
	public abstract ValueTask DisposeAsync();
	public abstract ulong? OnPurgeHard(IPurgeable.Target target, CancellationToken cancellationToken = default);
	public abstract ulong? OnPurgeSoft(IPurgeable.Target target, CancellationToken cancellationToken = default);
	public abstract long SizeBytes { get; }
	public abstract long TotalSize { get; }
	public abstract int Count { get; }
	[MustUseReturnValue]
	public abstract bool Contains(TKey key);
	public abstract TValue[]? Get(TKey key);
	[MustUseReturnValue]
	public abstract bool TryGet(TKey key, [NotNullWhen(true)] out TValue[]? value);
	[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public abstract bool TrySetDelegated<TValueGetter>(TKey key, TValueGetter valueGetter) where TValueGetter : struct, IMemoryCache<TKey, TValue>.IValueGetter;
	[MustUseReturnValue]
	public abstract bool TrySet(TKey key, TValue[] value);
	public abstract TValue[] Set(TKey key, TValue[] value);
	public abstract TValue[] SetOrTouch(TKey key, TValue[] value);
	public abstract void SetFast(TKey key, TValue[] value);
	public abstract void SetOrTouchFast(TKey key, TValue[] value);
	public abstract TValue[]? Update(TKey key, TValue[] value);
	public abstract TValue[]? Remove(TKey key);
	public abstract void Touch(TKey key);
	public abstract void RemoveFast(TKey key);
	public abstract void Trim(int count);
	public abstract void TrimTo(int count);
	public abstract void Clear();
	public abstract (ulong Count, ulong Size) ClearWithCount();
}
