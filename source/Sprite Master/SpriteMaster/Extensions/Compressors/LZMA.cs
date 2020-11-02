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
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions.Compressors {
	// https://stackoverflow.com/a/8605828
	// TODO : Implement a continual training dictionary so each stream doesn't require its own dictionary for in-memory compression.
	internal static class LZMA {
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
					Debug.InfoLn($"LZMA Compression not supported: '{ex.ToString()}'");
					return false;
				}
			}
		}

		private static class Data {
			internal static readonly byte[] Properties;

			static Data() {
				var encoder = new SevenZip.Compression.LZMA.Encoder();
				using var propertiesStream = new MemoryStream(5);
				encoder.WriteCoderProperties(propertiesStream);
				propertiesStream.Flush();
				Properties = propertiesStream.ToArray();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static SevenZip.Compression.LZMA.Encoder GetEncoder() {
			return new SevenZip.Compression.LZMA.Encoder();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static SevenZip.Compression.LZMA.Decoder GetDecoder() {
			var decoder = new SevenZip.Compression.LZMA.Decoder();
			decoder.SetDecoderProperties(Data.Properties);
			return decoder;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int CompressedLengthEstimate(byte[] data) {
			return data.Length >> 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int DecompressedLengthEstimate(byte[] data) {
			return data.Length << 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static byte[] Compress(byte[] data) {
			using var output = new MemoryStream(CompressedLengthEstimate(data));

			var encoder = GetEncoder();

			using (var input = new MemoryStream(data)) {
				encoder.Code(input, output, data.Length, -1, null);
			}

			output.Flush();
			return output.ToArray();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static unsafe byte[] Decompress(byte[] data) {
			using var output = new MemoryStream(DecompressedLengthEstimate(data));

			using (var input = new MemoryStream(data)) {
				var decoder = GetDecoder();
				decoder.Code(input, output, input.Length, -1, null);
			}

			output.Flush();
			return output.ToArray();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
}
