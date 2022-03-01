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

using SevenLZMA = SevenZip.Compression.LZMA;

namespace SpriteMaster.Compressors;

// https://stackoverflow.com/a/8605828
// TODO : Implement a continual training dictionary so each stream doesn't require its own dictionary for in-memory compression.
//[HarmonizeFinalizeCatcher<SevenLZMA.Encoder, DllNotFoundException>(critical: false)]
static class LZMA {

	private static bool? IsSupported_ = null;
	internal static bool IsSupported {
		[MethodImpl(Runtime.MethodImpl.RunOnce)]
		get {
			if (IsSupported_.HasValue) {
				return IsSupported_.Value;
			}

			try {
				var dummyData = new byte[16];
				var compressedData = CompressTest(dummyData);
				var uncompressedData = Decompress(compressedData, dummyData.Length);
				if (!dummyData.SequenceEqualF(uncompressedData)) {
					throw new Exception("Original and Uncompressed Data Mismatch");
				}
				Debug.Info("LZMA Compression is supported");
				IsSupported_ = true;
			}
			catch (DllNotFoundException) {
				Debug.Info($"LZMA Compression not supported");
				IsSupported_ = false;
			}
			catch (Exception ex) {
				Debug.Info($"LZMA Compression not supported: '{ex.GetType().Name} {ex.Message}'");
				IsSupported_ = false;
			}

			return IsSupported_.Value;
		}
	}

	private static class Data {
		internal static readonly byte[] Properties;

		[MethodImpl(Runtime.MethodImpl.RunOnce)]
		static Data() {
			var encoder = new SevenLZMA.Encoder();
			using var propertiesStream = new MemoryStream(5);
			encoder.WriteCoderProperties(propertiesStream);
			propertiesStream.Flush();
			Properties = propertiesStream.ToArray();
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static SevenLZMA.Encoder GetEncoder() => new();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static SevenLZMA.Decoder GetDecoder() {
		var decoder = new SevenLZMA.Decoder();
		decoder.SetDecoderProperties(Data.Properties);
		return decoder;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int CompressedLengthEstimate(byte[] data) => data.Length >> 1;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int CompressedLengthEstimate(ReadOnlySpan<byte> data) => data.Length >> 1;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int DecompressedLengthEstimate(byte[] data) => data.Length << 1;

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	internal static byte[] CompressTest(byte[] data) {
		using var output = new MemoryStream(CompressedLengthEstimate(data));

		using (var input = new MemoryStream(data)) {
			SevenLZMA.Encoder? encoder = null;
			try {
				encoder = GetEncoder();
				encoder.Code(input, output, data.Length, -1, null);
			}
			catch (DllNotFoundException) when (encoder is not null) {
				GC.SuppressFinalize(encoder);
				throw;
			}
		}

		output.Flush();
		return output.ToArray();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte[] Compress(byte[] data) {
		using var output = new MemoryStream(CompressedLengthEstimate(data));

		using (var input = new MemoryStream(data)) {
			var encoder = GetEncoder();
			encoder.Code(input, output, data.Length, -1, null);
		}

		output.Flush();
		return output.ToArray();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe byte[] Compress(ReadOnlySpan<byte> data) {
		using var output = new MemoryStream(CompressedLengthEstimate(data));

		fixed (byte* dataPtr = data) {
			using (var input = new UnmanagedMemoryStream(dataPtr, data.Length)) {
				var encoder = GetEncoder();
				encoder.Code(input, output, data.Length, -1, null);
			}
		}

		output.Flush();
		return output.ToArray();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte[] Decompress(byte[] data) {
		using var output = new MemoryStream(DecompressedLengthEstimate(data));

		using (var input = new MemoryStream(data)) {
			var decoder = GetDecoder();
			decoder.Code(input, output, input.Length, -1, null);
		}

		output.Flush();
		return output.ToArray();
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte[] Decompress(byte[] data, int size) {
		var output = new byte[size];
		using var outputStream = new MemoryStream(output);

		using (var input = new MemoryStream(data)) {
			var decoder = GetDecoder();
			decoder.Code(input, outputStream, input.Length, size, null);
		}

		outputStream.Flush();
		return output;
	}
}
