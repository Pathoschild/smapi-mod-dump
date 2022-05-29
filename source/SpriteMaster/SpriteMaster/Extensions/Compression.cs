/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types.Spans;
using System;
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

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	internal static Algorithm GetPreferredAlgorithm(Algorithm fromAlgorithm = Algorithm.Best) {
		for (int i = (int)fromAlgorithm; i > 0; --i) {
			var algorithm = (Algorithm)i;

			bool supported = algorithm switch {
				Algorithm.Compress => Compressors.SystemIO.IsSupported,
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

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Compress(this byte[] data, Algorithm algorithm) => algorithm switch {
		Algorithm.None => data,
		Algorithm.Compress => Compressors.SystemIO.Compress(data),
		Algorithm.Deflate => Compressors.Deflate.Compress(data),
		Algorithm.Zstd => Compressors.Zstd.Compress(data),
		_ => throw new Exception($"Unknown Compression Algorithm: '{algorithm}'"),
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Compress(this ReadOnlySpan<byte> data, Algorithm algorithm) => algorithm switch {
		Algorithm.None => data.ToArray(),
		Algorithm.Compress => Compressors.SystemIO.Compress(data),
		Algorithm.Deflate => Compressors.Deflate.Compress(data),
		Algorithm.Zstd => Compressors.Zstd.Compress(data),
		_ => throw new Exception($"Unknown Compression Algorithm: '{algorithm}'"),
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Compress(this Span<byte> data, Algorithm algorithm) => Compress((ReadOnlySpan<byte>)data, algorithm);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Compress(this PinnedSpan<byte> data, Algorithm algorithm) => Compress((Span<byte>)data, algorithm);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Compress(this ReadOnlyPinnedSpan<byte> data, Algorithm algorithm) => Compress((ReadOnlySpan<byte>)data, algorithm);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Compress(this byte[] data) => Compress(data, BestAlgorithm);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Compress(this ReadOnlySpan<byte> data) => Compress(data, BestAlgorithm);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Compress(this Span<byte> data) => Compress((ReadOnlySpan<byte>)data);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Compress(this ReadOnlyPinnedSpan<byte> data) => Compress(data, BestAlgorithm);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Compress(this PinnedSpan<byte> data) => Compress((ReadOnlySpan<byte>)data);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Decompress(this byte[] data, int size, Algorithm algorithm) {
		if (size == -1) {
			return Decompress(data, algorithm);
		}

		return algorithm switch {
			Algorithm.None => data,
			Algorithm.Compress => Compressors.SystemIO.Decompress(data, size),
			Algorithm.Deflate => Compressors.Deflate.Decompress(data, size),
			Algorithm.Zstd => Compressors.Zstd.Decompress(data, size),
			_ => throw new Exception($"Unknown Compression Algorithm: '{algorithm}'"),
		};
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Decompress(this byte[] data, int size) {
		return Decompress(data, size, BestAlgorithm);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Decompress(this byte[] data, Algorithm algorithm) => algorithm switch {
		Algorithm.None => data,
		Algorithm.Compress => Compressors.SystemIO.Decompress(data),
		Algorithm.Deflate => Compressors.Deflate.Decompress(data),
		Algorithm.Zstd => Compressors.Zstd.Decompress(data),
		_ => throw new Exception($"Unknown Compression Algorithm: '{algorithm}'"),
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Decompress(this byte[] data) => Decompress(data, BestAlgorithm);
}
