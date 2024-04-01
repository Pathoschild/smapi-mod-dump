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
using Microsoft.Toolkit.HighPerformance;
using SpriteMaster.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpriteMaster.Types.MemoryCache;

internal class ObjectCache<TKey, TValue> : AbstractObjectCache<TKey, TValue>
	where TKey : notnull where TValue : notnull {

	private static readonly bool IsTouchableValue = typeof(TValue).IsAssignableFrom(typeof(ITouchable));

	[StructLayout(LayoutKind.Auto)]
	private struct CacheEntry {
		internal readonly TValue Value;
		internal ConcurrentLinkedListSlim<TKey>.NodeRef Node;
		internal readonly long Size;

		internal readonly bool IsValid => Node.IsValid;

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal CacheEntry(in TValue value, ConcurrentLinkedListSlim<TKey>.NodeRef node, long size) {
			Contract.Assert(value is not null);
			Contract.Assert(node.IsValid);

			Value = value;
			Node = node;
			Size = size;
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal readonly void UpdateValue(in TValue value, long size) {
			Contract.Assert(value is not null);
			Contract.Assert(size >= 0);

			Unsafe.AsRef(in Value) = value;
			Unsafe.AsRef(in Size) = size;
		}
	}

	
	private readonly Dictionary<TKey, CacheEntry> Cache = new();
	private readonly ConcurrentLinkedListSlim<TKey> RecentAccessList = new();
	private readonly SharedLock CacheLock = new();

	private readonly CancellationTokenSource ThreadCancelSource = new();
	private readonly AutoResetEvent TrimEvent = new(false);
	private readonly Thread TrimThread;

	private void CacheTrimLoop(in CancellationToken cancellationToken) {
		while (!cancellationToken.IsCancellationRequested) {
			if (!TrimEvent.WaitOne()) {
				continue;
			}

			if (cancellationToken.IsCancellationRequested) {
				break;
			}

			using (var l = TryPurgingLock) if (l) {
				TrimToSize(MaxSizeHysteresis);
			}
		}
	}

	internal ObjectCache(string name, long? maxSize, RemovalCallbackDelegate<TKey, TValue>? removalAction = null) :
		base(name, maxSize ?? long.MaxValue, removalAction) {
		TrimThread = ThreadExt.Run(
			() => CacheTrimLoop(ThreadCancelSource.Token),
			background: true, 
			name: $"Cache '{name}' Trim Thread"
		);
	}

	public override int Count => Cache.Count;

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override bool Contains(TKey key) {
		using (CacheLock.Read) {
			return Cache.ContainsKey(key);
		}
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override TValue? Get(TKey key) {
		if (TryGet(key, out var value)) {
			return value;
		}

		return default;
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override bool TryGet(TKey key, [NotNullWhen(true)] out TValue? value) {
		using (CacheLock.Read) {
			if (Cache.TryGetValue(key, out var entry)) {
				RecentAccessList.MoveToFront(entry.Node);
				value = entry.Value;
				return true;
			}
		}

		value = default;
		return false;
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override bool TrySetDelegated<TValueGetter>(TKey key, TValueGetter valueGetter) where TValueGetter : struct {
		long sizeDelta;

		using (CacheLock.ReadWrite) {
			if (!Cache.ContainsKey(key)) {
				var value = valueGetter.Invoke();
				var size = GetSizeBytes(value);

				using (CacheLock.Write) {
					Cache.Add(key, new(value, RecentAccessList.AddFront(key), size));
					sizeDelta = size;
				}
			}
			else {
				return false;
			}

			Interlocked.Add(ref CurrentSize, sizeDelta);
		}

		if (sizeDelta != 0L && Interlocked.Read(ref CurrentSize) > MaxSize) {
			TrimEvent.Set();
		}

		return true;
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override bool TrySet(TKey key, TValue value) {
		Contract.Assert(value is not null);

		long sizeDelta;
		var size = GetSizeBytes(value);

		using (CacheLock.ReadWrite) {
			if (!Cache.ContainsKey(key)) {

				using (CacheLock.Write) {
					Cache.Add(key, new(value, RecentAccessList.AddFront(key), size));
					sizeDelta = size;
				}
			}
			else {
				return false;
			}

			Interlocked.Add(ref CurrentSize, sizeDelta);
		}

		if (sizeDelta != 0L && Interlocked.Read(ref CurrentSize) > MaxSize) {
			TrimEvent.Set();
		}

		return true;
	}

	[MustUseReturnValue]
	public override TValue Set(TKey key, TValue value) {
		Contract.Assert(value is not null);

		Optional<TValue> original = default;
		var size = GetSizeBytes(value);
		long sizeDelta;

		using (CacheLock.Write) {
			if (Cache.TryGetValue(key, out var entry)) {
				RecentAccessList.MoveToFront(entry.Node);
				var originalValue = entry.Value;
				if (ReferenceEquals(originalValue, value)) {
					return originalValue;
				}

				original = originalValue;

				var originalSize = entry.Size;
				sizeDelta = size - originalSize;
				entry.UpdateValue(value, size);
				Cache[key] = entry;
			}
			else {
				Cache.Add(key, new(value, RecentAccessList.AddFront(key), size));
				sizeDelta = size;
			}

			Interlocked.Add(ref CurrentSize, sizeDelta);
		}

		if (sizeDelta != 0L) {
			if (Interlocked.Read(ref CurrentSize) > MaxSize) {
				TrimEvent.Set();
			}
		}


		if (original.HasValue) {
			OnEntryRemoved(key, original, EvictionReason.Replaced);
		}

		return value;
	}

	[MustUseReturnValue]
	public override TValue SetOrTouch(TKey key, TValue value) {
		Contract.Assert(value is not null);

		Optional<TValue> original = default;
		var size = GetSizeBytes(value);
		long sizeDelta;

		using (CacheLock.Write) {
			if (Cache.TryGetValue(key, out var entry)) {
				RecentAccessList.MoveToFront(entry.Node);
				var originalValue = entry.Value;

				original = originalValue;

				var originalSize = entry.Size;
				sizeDelta = size - originalSize;
				entry.UpdateValue(value, size);
				Cache[key] = entry;
			}
			else {
				Cache.Add(key, new(value, RecentAccessList.AddFront(key), size));
				sizeDelta = size;
			}

			Interlocked.Add(ref CurrentSize, sizeDelta);
		}

		if (sizeDelta != 0L) {
			if (Interlocked.Read(ref CurrentSize) > MaxSize) {
				TrimEvent.Set();
			}
		}


		if (original.HasValue) {
			OnEntryRemoved(key, original, EvictionReason.Replaced);
		}

		return value;
	}

	public override void SetFast(TKey key, TValue value) {
		_ = Set(key, value);
	}

	public override void SetOrTouchFast(TKey key, TValue value) {
		_ = SetOrTouch(key, value);
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override TValue? Update(TKey key, TValue value) {
		Contract.Assert(value is not null);

		Optional<TValue> original = default;
		var size = GetSizeBytes(value);
		long sizeDelta;

		using (CacheLock.ReadWrite) {
			bool found = Cache.TryGetValue(key, out var entry);
			if (found) {
				RecentAccessList.MoveToFront(entry.Node);
				var originalValue = entry.Value;
				if (ReferenceEquals(originalValue, value)) {
					return originalValue;
				}

				original = originalValue;

				var originalSize = entry.Size;
				sizeDelta = size - originalSize;

				using (CacheLock.Write) {
					entry.UpdateValue(value, size);
					Cache[key] = entry;
				}
			}
			else {
				using (CacheLock.Write) {
					Cache.Add(key, new(value, RecentAccessList.AddFront(key), size));
					sizeDelta = size;
				}
			}

			Interlocked.Add(ref CurrentSize, sizeDelta);
		}

		if (sizeDelta != 0L && Interlocked.Read(ref CurrentSize) > MaxSize) {
			TrimEvent.Set();
		}

		if (original.HasValue) {
			OnEntryRemoved(key, original, EvictionReason.Replaced);
		}

		return original;
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override TValue? Remove(TKey key) {
		Optional<TValue> result = default;

		using (CacheLock.Write) {
			if (Cache.Remove(key, out var entry)) {
				result = entry.Value;
				Interlocked.Add(ref CurrentSize, -entry.Size);
				RecentAccessList.Release(ref entry.Node);
				entry.Node = default;
			}
		}

		if (result.HasValue) {
			OnEntryRemoved(key, result, EvictionReason.Removed);
			return result;
		}

		return default;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void RemoveFast(TKey key) {
		Optional<TValue> result = default;

		using (CacheLock.Write) {
			if (Cache.Remove(key, out var entry)) {
				result = entry.Value;
				Interlocked.Add(ref CurrentSize, -entry.Size);
				RecentAccessList.Release(ref entry.Node);
				entry.Node = default;
			}
		}

		if (result.HasValue) {
			OnEntryRemoved(key, result, EvictionReason.Removed);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void Touch(TKey key) {
		if (!IsTouchableValue) {
			return;
		}

		Optional<TValue> removeResult = default;

		using (CacheLock.Write) {
			if (Cache.TryGetValue(key, out var entry)) {
				if (!((ITouchable)entry.Value).Touch()) {
					removeResult = entry.Value;
					Interlocked.Add(ref CurrentSize, -entry.Size);
					RecentAccessList.Release(ref entry.Node);
					entry.Node = default;
				}
				RecentAccessList.MoveToFront(entry.Node);
			}
		}

		if (removeResult.HasValue) {
			OnEntryRemoved(key, removeResult, EvictionReason.TokenExpired);
		}
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	private bool TryPopKey([NotNullWhen(true)] out TKey? key) {
#if SHIPPING && !CONTRACTS_FULL
		if (RecentAccessList.Count != 0) {
			key = RecentAccessList.RemoveLast();
			Contract.Assert(key is not null);
			return true;
		}
		if (Cache.Count != 0) {
			key = Cache.Last().Key;
			Contract.Assert(key is not null);
			return true;
		}

		key = default;
		return false;
#else
		Contract.Assert(RecentAccessList.Count > 0);
		key = RecentAccessList.RemoveLast();
		Contract.Assert(key is not null);
		return true;
#endif
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void Trim(int count) {
		count.AssertPositiveOrZero();

		if (count == 0) {
			return;
		}

		var trimArray = GC.AllocateUninitializedArray<KeyValuePair<TKey, TValue>>(count);

		using (CacheLock.Write) {
			if (count > Cache.Count) {
				count = Cache.Count;
			}

			if (count == 0) {
				return;
			}

			int i = 0;
			for (; i < count; i++) {
				if (!TryPopKey(out var removeKey)) {
					break;
				}
				var removed = Cache.Remove(removeKey, out var entry);
				removed.AssertTrue();
				Interlocked.Add(ref CurrentSize, -entry.Size);
				trimArray[i] = new(removeKey, entry.Value);
			}

			count = i;
		}

		ReportTrimmed(trimArray.AsReadOnlySpan(0, count), EvictionReason.Capacity);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void TrimTo(int count) {
		count.AssertPositiveOrZero();

		Trim(Cache.Count - count);
	}

	private void TrimToSize(long size) {
		CacheLock.IsWriteLock.AssertFalse();
		size.AssertPositiveOrZero();

		if (size == 0L) {
			Clear();
		}
		else {
			var trimmed = new List<KeyValuePair<TKey, TValue>>();

			using (CacheLock.Write) {
				var currentSize = Interlocked.Read(ref CurrentSize);

				while (currentSize >= size) {
					if (!TryPopKey(out var removeKey)) {
						break;
					}
					var removed = Cache.Remove(removeKey, out var entry);
					removed.AssertTrue();
					currentSize -= entry.Size;
					trimmed.Add(new(removeKey, entry.Value));
				}

				CurrentSize = currentSize;
			}

			ReportTrimmed(trimmed.AsSpan(), EvictionReason.Capacity);
		}
	}

	private void ReportTrimmed(ReadOnlySpan<KeyValuePair<TKey, TValue>> trimmed, EvictionReason reason) {
		CacheLock.IsWriteLock.AssertFalse();

		foreach (var pair in trimmed) {
			OnEntryRemoved(pair.Key, pair.Value, reason);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void Clear() =>
		_ = ClearWithCount();

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override (ulong Count, ulong Size) ClearWithCount() {
		int removedCount;
		var removedPairs = GC.AllocateUninitializedArray<KeyValuePair<TKey, TValue>>(Count);

		long removedSize;

		using (CacheLock.Write) {
			removedSize = CurrentSize;

			if (Count > removedPairs.Length) {
				removedPairs = GC.AllocateUninitializedArray<KeyValuePair<TKey, TValue>>(Count);
			}

			removedCount = Count;

			var removedIndex = 0;
			foreach (var (key, entry) in Cache) {
				removedPairs[removedIndex++] = new(key, entry.Value);
			}

			CurrentSize = 0;
			Cache.Clear();
			RecentAccessList.Clear();
		}

		ReportTrimmed(removedPairs.AsReadOnlySpan(0, removedCount), EvictionReason.Removed);

		return (checked((ulong)removedCount), checked((ulong)removedSize));
	}

	public override void Dispose() {
		if (!OnDispose()) {
			return;
		}

		base.Dispose();

		ThreadCancelSource.Cancel();
		TrimEvent.Set();

		Clear();

		TrimThread.Join();

		Clear();
		CacheLock.Dispose();

		ThreadCancelSource.Dispose();
		TrimEvent.Dispose();
	}

	public override ulong? OnPurgeHard(IPurgeable.Target target, CancellationToken cancellationToken = default) {
		if (Disposed) {
			return null;
		}

		lock (PurgingLock) {
			return ClearWithCount().Size;
		}
	}

	public override ulong? OnPurgeSoft(IPurgeable.Target target, CancellationToken cancellationToken = default) {
		if (Disposed) {
			return null;
		}

		if (target.Difference == 0UL) {
			return 0UL;
		}

		lock (PurgingLock) {
			long startSize = TotalSize;
			long purgeSize = Math.Min(startSize, (long)target.Difference);
			long purgeTo = Math.Max(0L, startSize - purgeSize);

			if (purgeTo == 0L) {
				return ClearWithCount().Size;
			}
			else if (purgeTo == startSize) {
				return 0UL;
			}
			else {
				TrimToSize(purgeTo);
				long purged = Math.Max(0L, TotalSize - startSize);
				return (ulong)purged;
			}
		}
	}
}
