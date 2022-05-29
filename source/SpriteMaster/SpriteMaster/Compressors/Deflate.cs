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
internal static class Deflate {
	private static readonly Action<ZlibStream, CompressionStrategy>? SetStrategy = typeof(ZlibStream).GetFieldSetter<ZlibStream, CompressionStrategy>("Strategy");

	private static bool? IsSupportedInternal = null;
	internal static bool IsSupported {
		[MethodImpl(Runtime.MethodImpl.RunOnce)]
		get {
			if (IsSupportedInternal.HasValue) {
				return IsSupportedInternal.Value;
			}

			try {
				var dummyData = new byte[16];
				var compressedData = CompressTest(dummyData);
				var uncompressedData = Decompress(compressedData, dummyData.Length);
				if (!dummyData.SequenceEqualF(uncompressedData)) {
					throw new Exception("Original and Uncompressed Data Mismatch");
				}
				Debug.Info("Deflate Compression is supported");
				IsSupportedInternal = true;
			}
			catch (DllNotFoundException) {
				Debug.Info("Deflate Compression not supported");
				IsSupportedInternal = false;
			}
			catch (Exception ex) {
				Debug.Info($"Deflate Compression not supported: '{ex.GetType().Name} {ex.Message}'");
				IsSupportedInternal = false;
			}

			return IsSupportedInternal.Value;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int CompressedLengthEstimate(byte[] data) => data.Length >> 1;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int CompressedLengthEstimate(ReadOnlySpan<byte> data) => data.Length >> 1;

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	internal static byte[] CompressTest(byte[] data) {
		ZlibStream? compressor = null;
		try {
			using var val = new MemoryStream(CompressedLengthEstimate(data));
			using (compressor = new ZlibStream(val, CompressionMode.Compress, CompressionLevel.BestCompression)) {
				SetStrategy?.Invoke(compressor, CompressionStrategy.Filtered);
				compressor.Write(data, 0, data.Length);
			}
			return val.GetArray();
		}
		catch (DllNotFoundException) when (compressor is not null) {
			GC.SuppressFinalize(compressor);
			throw;
		}
	}

	internal static byte[] Compress(byte[] data) {
		using var val = new MemoryStream(CompressedLengthEstimate(data));
		using (var compressor = new ZlibStream(val, CompressionMode.Compress, CompressionLevel.BestCompression)) {
			SetStrategy?.Invoke(compressor, CompressionStrategy.Filtered);
			compressor.Write(data, 0, data.Length);
		}
		return val.GetArray();
	}

	internal static byte[] Compress(ReadOnlySpan<byte> data) {
		using var val = new MemoryStream(CompressedLengthEstimate(data));
		using (var compressor = new ZlibStream(val, CompressionMode.Compress, CompressionLevel.BestCompression)) {
			SetStrategy?.Invoke(compressor, CompressionStrategy.Filtered);
			compressor.Write(data.ToArray(), 0, data.Length);
		}
		return val.GetArray();
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static byte[] Decompress(byte[] data) => ZlibStream.UncompressBuffer(data);

	internal static byte[] Decompress(byte[] data, int size) {
		using var dataStream = new MemoryStream(data);
		var output = new byte[size];
		using var val = new MemoryStream(output);
		using var compressor = new ZlibStream(dataStream, CompressionMode.Decompress);
		compressor.CopyTo(val);
		return output;
	}
}
