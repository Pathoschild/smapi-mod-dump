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
using SpriteMaster.Extensions;
using SpriteMaster.Extensions.Reflection;
using SpriteMaster.Types.Interlocking;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SpriteMaster.Types.MemoryCache;

internal abstract class AbstractObjectCache<TKey, TValue> :
	SpriteMasterObject, IObjectCache<TKey, TValue>
	where TKey : notnull where TValue : notnull {
	protected readonly RemovalCallbackDelegate<TKey, TValue>? RemovalCallback = null;

	public string Name { get; }
	protected readonly long MaxSize;
	protected readonly long MaxSizeHysteresis;
	protected long CurrentSize = 0L;

	protected readonly object PurgingLock = new();
	protected readonly InterlockedBool Disposed = false;

	protected TryLock TryPurgingLock => new(PurgingLock);

	public long TotalSize => Interlocked.Read(ref CurrentSize);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	protected void OnEntryRemoved(TKey key, in TValue value, EvictionReason reason) {
		Contract.Assert(value is not null);
		RemovalCallback?.Invoke(reason, key, value);
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	protected static long GetSizeBytes(TValue value) {
		var size = value switch {
			byte[] bytes => bytes.LongLength,
			IList<byte> bytes => bytes.Count,
			Array array => array.LongLength * typeof(TValue).GetElementType()!.Size(),
			IByteSize byteSizeInterface => byteSizeInterface.SizeBytes,
			_ => ThrowHelper.ThrowInvalidTypeParameterException<long>(
				$"Type cannot have its size fetched by '{typeof(AbstractObjectCache<TKey, TValue>).GetTypeName()}'",
				typeof(TValue)
			)
		};

		size.AssertPositiveOrZero();

		return size;
	}

	internal AbstractObjectCache(string name, long maxSize, RemovalCallbackDelegate<TKey, TValue>? removalAction = null) {
		Name = name.Intern();
		RemovalCallback = removalAction;
		MaxSize = maxSize;
		MaxSizeHysteresis = (maxSize >> 2) + (maxSize >> 1);
	}

	[Pure]
	public abstract int Count { get; }

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public abstract bool Contains(TKey key);

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public abstract TValue? Get(TKey key);

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public abstract bool TryGet(TKey key, [NotNullWhen(true)] out TValue? value);

	[MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public abstract bool TrySetDelegated<TValueGetter>(TKey key, TValueGetter valueGetter) where TValueGetter : struct, IObjectCache<TKey, TValue>.IValueGetter;

	[MustUseReturnValue]
	public abstract bool TrySet(TKey key, TValue value);

	[MustUseReturnValue]
	public abstract TValue Set(TKey key, TValue value);

	[MustUseReturnValue]
	public abstract TValue SetOrTouch(TKey key, TValue value);

	public abstract void SetFast(TKey key, TValue value);
	public abstract void SetOrTouchFast(TKey key, TValue value);

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public abstract TValue? Update(TKey key, TValue value);

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public abstract TValue? Remove(TKey key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public abstract void RemoveFast(TKey key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public abstract void Touch(TKey key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public abstract void Trim(int count);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public abstract void TrimTo(int count);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public abstract void Clear();

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public abstract (ulong Count, ulong Size) ClearWithCount();

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	protected bool OnDispose() {
		return !Disposed.CompareExchange(true, false);
	}

	public ValueTask DisposeAsync() {
		return Disposed ? ValueTask.CompletedTask : new(Task.Run(Dispose));
	}

	[Pure]
	public long SizeBytes => TotalSize;

	public abstract ulong? OnPurgeHard(IPurgeable.Target target, CancellationToken cancellationToken = default);
	public abstract ulong? OnPurgeSoft(IPurgeable.Target target, CancellationToken cancellationToken = default);
}
