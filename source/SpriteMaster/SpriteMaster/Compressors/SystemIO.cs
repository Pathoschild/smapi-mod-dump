/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using LinqFasterer;
using SpriteMaster.Extensions;
using System;
using System.IO;
using System.Runtime.CompilerServices;

using IOC = System.IO.Compression;

namespace SpriteMaster.Compressors;

//[HarmonizeFinalizeCatcher<IOC.DeflateStream, DllNotFoundException>(critical: false)]
internal static class SystemIO {

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
				Debug.Info("System.IO Compression is supported");
				IsSupportedInternal = true;
			}
			catch (DllNotFoundException) {
				Debug.Info("System.IO Compression not supported");
				IsSupportedInternal = false;
			}
			catch (Exception ex) {
				Debug.Info($"System.IO Compression not supported: '{ex.GetType().Name} {ex.Message}'");
				IsSupportedInternal = false;
			}

			return IsSupportedInternal.Value;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int CompressedLengthEstimate(byte[] data) => data.Length >> 1;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int CompressedLengthEstimate(ReadOnlySpan<byte> data) => data.Length >> 1;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int DecompressedLengthEstimate(byte[] data) => data.Length << 1;

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	internal static byte[] CompressTest(byte[] data) {
		IOC.DeflateStream? compressor = null;
		try {
			using var val = new MemoryStream(CompressedLengthEstimate(data));
			using (compressor = new IOC.DeflateStream(val, IOC.CompressionLevel.Optimal)) {
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
		using (var compressor = new IOC.DeflateStream(val, IOC.CompressionLevel.Optimal)) {
			compressor.Write(data, 0, data.Length);
		}
		return val.GetArray();
	}

	internal static byte[] Compress(ReadOnlySpan<byte> data) {
		using var val = new MemoryStream(CompressedLengthEstimate(data));
		using (var compressor = new IOC.DeflateStream(val, IOC.CompressionLevel.Optimal)) {
			compressor.Write(data);
		}
		return val.GetArray();
	}

	internal static byte[] Decompress(byte[] data) {
		using var dataStream = new MemoryStream(data);
		using var val = new MemoryStream(DecompressedLengthEstimate(data));
		using (var compressor = new IOC.DeflateStream(dataStream, IOC.CompressionMode.Decompress)) {
			compressor.CopyTo(val);
		}
		return val.GetArray();
	}

	internal static byte[] Decompress(byte[] data, int size) {
		using var dataStream = new MemoryStream(data);
		var output = new byte[size];
		using var val = new MemoryStream(output);
		using var compressor = new IOC.DeflateStream(dataStream, IOC.CompressionMode.Decompress);
		compressor.CopyTo(val);
		return output;
	}
}
