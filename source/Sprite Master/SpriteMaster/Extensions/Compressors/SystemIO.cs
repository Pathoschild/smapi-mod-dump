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
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions.Compressors;

static class SystemIO {
	internal static bool IsSupported {
		[MethodImpl(Runtime.MethodImpl.RunOnce)]
		get {
			try {
				var dummyData = new byte[16];
				var compressedData = Compress(dummyData);
				var uncompressedData = Decompress(compressedData, dummyData.Length);
				if (!EnumerableF.SequenceEqualF(dummyData, uncompressedData)) {
					throw new Exception("Original and Uncompressed Data Mismatch");
				}
				Debug.InfoLn("System.IO Compression is supported");
				return true;
			}
			catch (DllNotFoundException) {
				Debug.InfoLn($"System.IO Compression not supported");
				return false;
			}
			catch (Exception ex) {
				Debug.InfoLn($"System.IO Compression not supported: '{ex}'");
				return false;
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int CompressedLengthEstimate(byte[] data) {
		return data.Length >> 1;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int DecompressedLengthEstimate(byte[] data) {
		return data.Length << 1;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte[] Compress(byte[] data) {
		using var val = new MemoryStream(CompressedLengthEstimate(data));
		using (var compressor = new System.IO.Compression.DeflateStream(val, System.IO.Compression.CompressionLevel.Optimal)) {
			compressor.Write(data, 0, data.Length);
		}
		return val.ToArray();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte[] Decompress(byte[] data) {
		using var dataStream = new MemoryStream(data);
		using var val = new MemoryStream(DecompressedLengthEstimate(data));
		using (var compressor = new System.IO.Compression.DeflateStream(dataStream, System.IO.Compression.CompressionMode.Decompress)) {
			compressor.CopyTo(val);
		}
		return val.ToArray();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte[] Decompress(byte[] data, int size) {
		using var dataStream = new MemoryStream(data);
		var output = new byte[size];
		using (var val = new MemoryStream(output)) {
			using var compressor = new System.IO.Compression.DeflateStream(dataStream, System.IO.Compression.CompressionMode.Decompress);
			compressor.CopyTo(val);
		}
		return output;
	}
}
