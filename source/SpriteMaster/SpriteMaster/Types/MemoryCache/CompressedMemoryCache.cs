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

internal class CompressedMemoryCache<TKey, TValue> :
	AbstractMemoryCache<TKey, TValue>, ICompressedMemoryCache
	where TKey : notnull where TValue : unmanaged {
	private readonly ObjectCache<TKey, ValueEntry> UnderlyingCache;
	private readonly Compression.Algorithm CurrentAlgorithm = Compression.GetPreferredAlgorithm(SMConfig.ResidentCache.Compress);

	private sealed class ValueEntry : IByteSize, IDisposable {
		private readonly WeakReference<TValue[]> UncompressedInternal;
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

			CompressionTask.Wait();
			Compressed.AssertNotNull();

			lock (this) {
				if (UncompressedInternal.TryGetTarget(out uncompressed)) {
					return uncompressed;
				}

				var uncompressedData = Compressed!.Decompress<TValue>((int)Size, Algorithm);
				UncompressedInternal.SetTarget(uncompressedData);
				return uncompressedData;
			}
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		public void Dispose() {
			lock (this) {
				CompressionTask.Dispose();
				Compressed = null;
				UncompressedInternal.SetTarget(null!);
			}
		}
	}

	public override long TotalSize => UnderlyingCache.TotalSize;
	public override int Count => UnderlyingCache.Count;

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override TValue[]? Get(TKey key) {
		if (UnderlyingCache.TryGet(key, out var entry)) {
			return entry.UncompressedArray;
		}

		return null;
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override ReadOnlySpan<TValue> GetSpan(TKey key) {
		if (UnderlyingCache.TryGet(key, out var entry)) {
			return entry.Uncompressed;
		}

		return default;
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override bool TryGet(TKey key, [NotNullWhen(true)] out TValue[]? value) {
		if (UnderlyingCache.TryGet(key, out var entry)) {
			value = entry.UncompressedArray;
			return true;
		}

		value = null;
		return false;
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override bool TryGetSpan(TKey key, out ReadOnlySpan<TValue> value) {
		if (UnderlyingCache.TryGet(key, out var entry)) {
			value = entry.Uncompressed;
			return true;
		}

		value = default;
		return false;
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public override TValue[] Set(TKey key, TValue[] value) {
		UnderlyingCache.SetFast(key, new(value, CurrentAlgorithm));
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void SetFast(TKey key, TValue[] value) {
		UnderlyingCache.SetFast(key, new(value, CurrentAlgorithm));
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override TValue[]? Update(TKey key, TValue[] value) {
		var entry = UnderlyingCache.Update(key, new(value, CurrentAlgorithm));
		return entry?.UncompressedArray;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override ReadOnlySpan<TValue> UpdateSpan(TKey key, TValue[] value) {
		var entry = UnderlyingCache.Update(key, new(value, CurrentAlgorithm));
		return entry is not null ? entry.Uncompressed : default;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override TValue[]? Remove(TKey key) {
		return UnderlyingCache.Remove(key)?.UncompressedArray;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void RemoveFast(TKey key) {
		UnderlyingCache.RemoveFast(key);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override ReadOnlySpan<TValue> RemoveSpan(TKey key) {
		var entry = UnderlyingCache.Remove(key);
		if (entry is not null) {
			return entry.Uncompressed;
		}

		return default;
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
	public override  (ulong Count, ulong Size) ClearWithCount() =>
		UnderlyingCache.ClearWithCount();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override void Dispose() {
		UnderlyingCache.Dispose();
	}

	public override ValueTask DisposeAsync() {
		return UnderlyingCache.DisposeAsync();
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private void OnRemove(EvictionReason reason, TKey key, ValueEntry element) {
		RemovalCallback?.Invoke(reason, key, null!);
	}

	internal CompressedMemoryCache(string name, long? maxSize, RemovalCallbackDelegate<TKey, TValue[]>? removalAction = null) :
		base(name, removalAction) {
		UnderlyingCache = new(name, maxSize, OnRemove);
	}

	public override ulong? OnPurgeHard(IPurgeable.Target target, CancellationToken cancellationToken = default) {
		return UnderlyingCache.OnPurgeHard(target, cancellationToken);
	}

	public override ulong? OnPurgeSoft(IPurgeable.Target target, CancellationToken cancellationToken = default) {
		return UnderlyingCache.OnPurgeSoft(target, cancellationToken);
	}

	public override long SizeBytes => UnderlyingCache.SizeBytes;
}
