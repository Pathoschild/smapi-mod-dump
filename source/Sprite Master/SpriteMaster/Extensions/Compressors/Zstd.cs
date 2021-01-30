/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions.Compressors {
	// TODO : Implement a continual training dictionary so each stream doesn't require its own dictionary for in-memory compression.
	internal static class Zstd {
		internal static bool IsSupported {
			get {
				try {
					var dummyData = new byte[16];
					var compressedData = Compress(dummyData);
					var uncompressedData = Decompress(compressedData, dummyData.Length);
					if (!Enumerable.SequenceEqual(dummyData, uncompressedData)) {
						throw new Exception("Original and Uncompressed Data Mismatch");
					}
					return true;
				}
				catch (Exception ex) {
					Debug.InfoLn($"Zstd Compression not supported: '{ex.ToString()}'");
					return false;
				}
			}
		}

		private static class Options {
			internal static readonly ZstdNet.CompressionOptions CompressionDefault = new(ZstdNet.CompressionOptions.DefaultCompressionLevel);
			internal static readonly ZstdNet.DecompressionOptions DecompressionDefault = new(null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ZstdNet.Compressor GetEncoder() {
			return new ZstdNet.Compressor(Options.CompressionDefault);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ZstdNet.Decompressor GetDecoder() {
			return new ZstdNet.Decompressor(Options.DecompressionDefault);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static byte[] Compress(byte[] data) {
			using var encoder = GetEncoder();
			return encoder.Wrap(data);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static unsafe byte[] Decompress(byte[] data) {
			using var decoder = GetDecoder();
			return decoder.Unwrap(data);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static byte[] Decompress(byte[] data, int size) {
			using var decoder = GetDecoder();
			return decoder.Unwrap(data, size);
		}
	}
}
