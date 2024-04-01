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
using SpriteMaster.Types.Spans;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

internal static class Compression {
	internal enum Algorithm {
		None = 0,
		Compress,
		Deflate,
		Zstd,
		Best = Zstd,
	}

	internal static readonly Algorithm BestAlgorithm = GetPreferredAlgorithm();

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe long AlignCount<T>(long size) where T : unmanaged =>
		(long)(((ulong)size + (ulong)(sizeof(T) - 1)) / (ulong)sizeof(T));

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe int AlignCount<T>(int size) where T : unmanaged =>
		(int)(((long)size + (sizeof(T) - 1)) / sizeof(T));

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	private static T[] ConvertArray<T>(byte[] data) where T : unmanaged {
		if (typeof(T) == typeof(byte)) {
			return (T[])(object)data;
		}

		int elementCount = AlignCount<T>(data.Length);
		T[] result = GC.AllocateUninitializedArray<T>(elementCount);
		data.AsReadOnlySpan().CopyTo(result.AsSpan().AsBytes());
		return result;
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	private static T[] ConvertArray<T>(ReadOnlySpan<byte> data) where T : unmanaged {
		if (typeof(T) == typeof(byte)) {
			return (T[])(object)data.ToArray();
		}

		return data.Cast<byte, T>().ToArray();
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	internal static Algorithm GetPreferredAlgorithm(Algorithm fromAlgorithm = Algorithm.Best) {
		for (int i = (int)fromAlgorithm; i > 0; --i) {
			var algorithm = (Algorithm)i;

			bool supported = algorithm switch {
				Algorithm.Compress => Compressors.SystemIo.IsSupported,
				Algorithm.Deflate => Compressors.Deflate.IsSupported,
				Algorithm.Zstd => Compressors.Zstd.IsSupported,
				_ => false
			};

			if (supported) {
				return algorithm;
			}
		}

		return Algorithm.None;
	}

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static T ThrowUnknownCompressionAlgorithmException<T>(Algorithm algorithm) =>
		throw new NotImplementedException($"Unknown Compression Algorithm: '{algorithm}'");

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Compress(this byte[] data, Algorithm algorithm) => algorithm switch {
		Algorithm.None => data,
		Algorithm.Compress => Compressors.SystemIo.Compress(data),
		Algorithm.Deflate => Compressors.Deflate.Compress(data),
		Algorithm.Zstd => Compressors.Zstd.Compress(data),
		_ => ThrowUnknownCompressionAlgorithmException<byte[]>(algorithm)
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static T[] Compress<T>(this ReadOnlySpan<byte> data, Algorithm algorithm) where T : unmanaged => algorithm switch {
		Algorithm.None => ConvertArray<T>(data),
		Algorithm.Compress => Compressors.SystemIo.Compress<T>(data),
		Algorithm.Deflate => Compressors.Deflate.Compress<T>(data),
		Algorithm.Zstd => Compressors.Zstd.Compress<T>(data),
		_ => ThrowUnknownCompressionAlgorithmException<T[]>(algorithm)
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static T[] Compress<T>(this Span<byte> data, Algorithm algorithm) where T : unmanaged => Compress<T>((ReadOnlySpan<byte>)data, algorithm);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static T[] Compress<T>(this PinnedSpan<byte> data, Algorithm algorithm) where T : unmanaged => Compress<T>((Span<byte>)data, algorithm);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static T[] Compress<T>(this ReadOnlyPinnedSpan<byte> data, Algorithm algorithm) where T : unmanaged => Compress<T>((ReadOnlySpan<byte>)data, algorithm);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Compress(this byte[] data) => Compress(data, BestAlgorithm);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static T[] Compress<T>(this ReadOnlySpan<byte> data) where T : unmanaged => Compress<T>(data, BestAlgorithm);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static T[] Compress<T>(this Span<byte> data) where T : unmanaged => Compress<T>((ReadOnlySpan<byte>)data);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static T[] Compress<T>(this ReadOnlyPinnedSpan<byte> data) where T : unmanaged => Compress<T>(data, BestAlgorithm);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static T[] Compress<T>(this PinnedSpan<byte> data) where T : unmanaged => Compress<T>((ReadOnlySpan<byte>)data);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Decompress(this byte[] data, int size, Algorithm algorithm) {
		if (size == -1) {
			return Decompress(data, algorithm);
		}

		return algorithm switch {
			Algorithm.None => data,
			Algorithm.Compress => Compressors.SystemIo.Decompress(data, size),
			Algorithm.Deflate => Compressors.Deflate.Decompress(data, size),
			Algorithm.Zstd => Compressors.Zstd.Decompress(data, size),
			_ => ThrowUnknownCompressionAlgorithmException<byte[]>(algorithm)
		};
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static T[] Decompress<T>(this byte[] data, int size, Algorithm algorithm) where T : unmanaged {
		if (size == -1) {
			ThrowHelper.ThrowArgumentException("size must not be negative", nameof(size));
		}

		return algorithm switch {
			Algorithm.None => ConvertArray<T>(data),
			Algorithm.Compress => Compressors.SystemIo.Decompress<T>(data, size),
			Algorithm.Deflate => Compressors.Deflate.Decompress<T>(data, size),
			Algorithm.Zstd => Compressors.Zstd.Decompress<T>(data, size),
			_ => ThrowUnknownCompressionAlgorithmException<T[]>(algorithm)
		};
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Decompress(this byte[] data, int size) {
		return Decompress(data, size, BestAlgorithm);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static T[] Decompress<T>(this byte[] data, int size) where T : unmanaged {
		return Decompress<T>(data, size, BestAlgorithm);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Decompress(this byte[] data, Algorithm algorithm) => algorithm switch {
		Algorithm.None => data,
		Algorithm.Compress => Compressors.SystemIo.Decompress(data),
		Algorithm.Deflate => Compressors.Deflate.Decompress(data),
		Algorithm.Zstd => Compressors.Zstd.Decompress(data),
		_ => ThrowUnknownCompressionAlgorithmException<byte[]>(algorithm)
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Decompress(this byte[] data) => Decompress(data, BestAlgorithm);
}
