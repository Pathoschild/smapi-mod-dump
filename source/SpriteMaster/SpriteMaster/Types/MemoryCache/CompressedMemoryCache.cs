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
using Microsoft.Toolkit.HighPerformance;
using SpriteMaster.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SpriteMaster.Types.MemoryCache;

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

internal class CompressedMemoryCache<TKey, TValue> : AbstractMemoryCache<TKey, TValue>, ICompressedMemoryCache where TKey : notnull where TValue : unmanaged {
	private readonly ObjectCache<TKey, ValueEntry> UnderlyingCache;
	private readonly Compression.Algorithm CurrentAlgorithm = Compression.GetPreferredAlgorithm(SMConfig.ResidentCache.Compress);

	private sealed class ValueEntry : IByteSize, IDisposable {
		private readonly WeakReference<TValue[]> UncompressedInternal;
		private readonly WeakReference<TValue[]> UncompressedInternal2 = new(null!);
		internal ReadOnlySpan<TValue> Uncompressed => GetUncompressed();
		private byte[]? Compressed = null;
		private readonly Task CompressionTask;
		private readonly long Size;
		private readonly Compression.Algorithm Algorithm;

		internal TValue[] UncompressedArray => GetUncompressedArray();

		public long SizeBytes => Size;

		internal long RealSizeBytes {
			get {
				if (Compressed is { } compressedData) {
					return compressedData.Length;
				}

				return 0L;
			}
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal ValueEntry(TValue[] value, Compression.Algorithm algorithm) {
			Size = value.LongLength * Unsafe.SizeOf<TValue>();
			UncompressedInternal = value.MakeWeak();

			Algorithm = algorithm;
			CompressionTask = ICompressedMemoryCache.TaskFactory.StartNew(() => Compress(value));
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		private void Compress(TValue[] uncompressed) {
			Compressed = uncompressed.AsReadOnlySpan().AsBytes().Compress<byte>(Algorithm);
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		private ReadOnlySpan<TValue> GetUncompressed() {
			return GetUncompressedArray();
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		private TValue[] GetUncompressedArray() {
			if (UncompressedInternal.TryGetTarget(out var uncompressed)) {
				return uncompressed;
			}

			if (UncompressedInternal2.TryGetTarget(out uncompressed)) {
				return uncompressed;
			}

			CompressionTask.Wait();
			Compressed.AssertNotNull();

			var uncompressedData = Compressed!.Decompress<TValue>((int)Size, Algorithm);
			UncompressedInternal2.SetTarget(uncompressedData);
			return uncompressedData;
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		public void Dispose() {
			CompressionTask.Dispose();
			Compressed = null;
			UncompressedInternal.SetTarget(null!);
			UncompressedInternal2.SetTarget(null!);
		}
	}

	internal override int Count => UnderlyingCache.Count;

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal override TValue[]? Get(TKey key) {
		var entry = UnderlyingCache.Get(key);

		return entry?.UncompressedArray;
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal override ReadOnlySpan<TValue> GetSpan(TKey key) {
		var entry = UnderlyingCache.Get(key);

		if (entry is not null) {
			return entry.Uncompressed;
		}

		return default;
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal override bool TryGet(TKey key, [NotNullWhen(true)] out TValue[]? value) {
		if (UnderlyingCache.TryGet(key, out var entry)) {
			value = entry.UncompressedArray;
			return true;
		}

		value = null;
		return false;
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal override bool TryGetSpan(TKey key, out ReadOnlySpan<TValue> value) {
		if (UnderlyingCache.TryGet(key, out var entry)) {
			value = entry.Uncompressed;
			return true;
		}

		value = default;
		return false;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal override TValue[] Set(TKey key, TValue[] value) {
		UnderlyingCache.Set(key, new(value, CurrentAlgorithm));
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal override TValue[]? Update(TKey key, TValue[] value) {
		var entry = UnderlyingCache.Update(key, new(value, CurrentAlgorithm));
		return entry?.UncompressedArray;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal override ReadOnlySpan<TValue> UpdateSpan(TKey key, TValue[] value) {
		var entry = UnderlyingCache.Update(key, new(value, CurrentAlgorithm));
		return entry is not null ? entry.Uncompressed : default;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal override TValue[]? Remove(TKey key) {
		return UnderlyingCache.Remove(key)?.UncompressedArray;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal override void RemoveFast(TKey key) {
		UnderlyingCache.RemoveFast(key);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal override ReadOnlySpan<TValue> RemoveSpan(TKey key) {
		var entry = UnderlyingCache.Remove(key);
		if (entry is not null) {
			return entry.Uncompressed;
		}

		return default;
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
	private void OnRemove(EvictionReason reason, TKey key, ValueEntry element) {
		RemovalCallback?.Invoke(reason, key, null!);
	}

	internal CompressedMemoryCache(string name, long? maxSize, RemovalCallbackDelegate? removalAction = null) :
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
