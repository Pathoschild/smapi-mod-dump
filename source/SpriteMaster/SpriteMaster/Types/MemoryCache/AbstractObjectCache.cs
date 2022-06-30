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
using SpriteMaster.Types.Interlocking;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SpriteMaster.Types.MemoryCache;

internal enum EvictionReason {
	None,
	/// <summary>Manually</summary>
	Removed,
	/// <summary>Overwritten</summary>
	Replaced,
	/// <summary>Timed out</summary>
	Expired,
	/// <summary>Event</summary>
	TokenExpired,
	/// <summary>Overflow</summary>
	Capacity,
}

internal abstract class AbstractObjectCache<TKey, TValue> : SpriteMasterObject, IDisposable, IAsyncDisposable, ICache where TKey : notnull where TValue : notnull {
	internal delegate void RemovalCallbackDelegate(EvictionReason reason, TKey key, TValue element);

	protected readonly RemovalCallbackDelegate? RemovalCallback = null;

	protected readonly InterlockedBool Purging = false;
	protected readonly long MaxSize;
	protected readonly long MaxSizeHysteresis;
	protected long CurrentSize = 0L;
	protected readonly InterlockedBool Disposed = false;

	internal long TotalSize => Interlocked.Read(ref CurrentSize);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	protected void OnEntryRemoved(TKey key, in TValue value, EvictionReason reason) {
		if (RemovalCallback is not null) {
			RemovalCallback!(reason, key, value);
		}
	}

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ThrowValueTypeException() =>
		throw new Exceptions.InvalidTypeParameterException<TValue>(
			$"Type '{typeof(TValue).FullName}' cannot have its size fetched by '{typeof(AbstractObjectCache<TKey, TValue>).Name}'"
		);

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static TReturn ThrowValueTypeException<TReturn>() =>
		throw new Exceptions.InvalidTypeParameterException<TValue>(
			$"Type '{typeof(TValue).FullName}' cannot have its size fetched by '{typeof(AbstractObjectCache<TKey, TValue>).Name}'"
		);

	[System.Diagnostics.Contracts.Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	protected static long GetSizeBytes(TValue value) {
		var size = value switch {
			byte[] bytes => bytes.LongLength,
			IList<byte> bytes => bytes.Count,
			Array array => array.LongLength * typeof(TValue).GetElementType()!.Size(),
			IByteSize byteSizeInterface => byteSizeInterface.SizeBytes,
			_ => ThrowValueTypeException<long>()
		};

		size.AssertPositiveOrZero();

		return size;
	}

	internal AbstractObjectCache(string name, long maxSize, RemovalCallbackDelegate? removalAction = null) {
		RemovalCallback = removalAction;
		MaxSize = maxSize;
		MaxSizeHysteresis = (maxSize >> 2) + (maxSize >> 1);
	}

	internal abstract int Count { get; }

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal abstract TValue? Get(TKey key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal abstract bool TryGet(TKey key, [NotNullWhen(true)] out TValue? value);

	internal abstract TValue Set(TKey key, TValue value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal abstract TValue? Update(TKey key, TValue value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal abstract TValue? Remove(TKey key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal abstract void RemoveFast(TKey key);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal abstract void Trim(int count);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal abstract void TrimTo(int count);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal abstract void Clear();

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal abstract (ulong Count, ulong Size) ClearWithCount();

	[MustUseReturnValue]
	protected bool OnDispose() {
		if (Disposed.CompareExchange(true, false)) {
			return false;
		}

		return true;
	}

	public ValueTask DisposeAsync() {
		if (Disposed) {
			return ValueTask.CompletedTask;
		}

		return new(Task.Run(Dispose));
	}

	[System.Diagnostics.Contracts.Pure]
	public long SizeBytes => TotalSize;

	public abstract ulong? OnPurgeHard(IPurgeable.Target target, CancellationToken cancellationToken = default);
	public abstract ulong? OnPurgeSoft(IPurgeable.Target target, CancellationToken cancellationToken = default);
}
