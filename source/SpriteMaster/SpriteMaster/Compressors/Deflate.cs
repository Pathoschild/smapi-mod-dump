/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Ionic.Zlib;
using LinqFasterer;
using SpriteMaster.Extensions;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Compressors;

//[HarmonizeFinalizeCatcher<ZlibStream, DllNotFoundException>(critical: false)]
static class Deflate {
	private static readonly Action<ZlibStream, CompressionStrategy>? SetStrategy = typeof(ZlibStream).GetFieldSetter<ZlibStream, CompressionStrategy>("Strategy");

	internal static bool IsSupported {
		[MethodImpl(Runtime.MethodImpl.RunOnce)]
		get {
			try {
				var dummyData = new byte[16];
				var compressedData = CompressTest(dummyData);
				var uncompressedData = Decompress(compressedData, dummyData.Length);
				if (!dummyData.SequenceEqualF(uncompressedData)) {
					throw new Exception("Original and Uncompressed Data Mismatch");
				}
				Debug.InfoLn("Deflate Compression is supported");
				return true;
			}
			catch (DllNotFoundException) {
				Debug.InfoLn($"Deflate Compression not supported");
				return false;
			}
			catch (Exception ex) {
				Debug.InfoLn($"Deflate Compression not supported: '{ex}'");
				return false;
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int CompressedLengthEstimate(byte[] data) => data.Length >> 1;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int CompressedLengthEstimate(ReadOnlySpan<byte> data) => data.Length >> 1;

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	internal static byte[] CompressTest(byte[] data) {
		ZlibStream? compressor = null;
		try {
			using var val = new MemoryStream(CompressedLengthEstimate(data));
			using (compressor = new ZlibStream(val, CompressionMode.Compress, CompressionLevel.BestCompression)) {
				if (SetStrategy is not null) {
					SetStrategy(compressor, CompressionStrategy.Filtered);
				}
				compressor.Write(data, 0, data.Length);
			}
			return val.ToArray();
		}
		catch (DllNotFoundException) when (compressor is not null) {
			GC.SuppressFinalize(compressor);
			throw;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte[] Compress(byte[] data) {
		using var val = new MemoryStream(CompressedLengthEstimate(data));
		using (var compressor = new ZlibStream(val, CompressionMode.Compress, CompressionLevel.BestCompression)) {
			if (SetStrategy is not null) {
				SetStrategy(compressor, CompressionStrategy.Filtered);
			}
			compressor.Write(data, 0, data.Length);
		}
		return val.ToArray();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte[] Compress(ReadOnlySpan<byte> data) {
		using var val = new MemoryStream(CompressedLengthEstimate(data));
		using (var compressor = new ZlibStream(val, CompressionMode.Compress, CompressionLevel.BestCompression)) {
			if (SetStrategy is not null) {
				SetStrategy(compressor, CompressionStrategy.Filtered);
			}
			compressor.Write(data.ToArray(), 0, data.Length);
		}
		return val.ToArray();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte[] Decompress(byte[] data) => ZlibStream.UncompressBuffer(data);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte[] Decompress(byte[] data, int size) {
		using var dataStream = new MemoryStream(data);
		var output = new byte[size];
		using (var val = new MemoryStream(output)) {
			using var compressor = new ZlibStream(dataStream, CompressionMode.Decompress);
			compressor.CopyTo(val);
		}
		return output;
	}
}
