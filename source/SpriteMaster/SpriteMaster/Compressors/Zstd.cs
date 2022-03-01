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
using Pastel;
using System;
using System.Runtime.CompilerServices;
using static SpriteMaster.Runtime;

namespace SpriteMaster.Compressors;
// TODO : Implement a continual training dictionary so each stream doesn't require its own dictionary for in-memory compression.
//[HarmonizeFinalizeCatcher<ZstdNet.Compressor, DllNotFoundException>(critical: false)]
static class Zstd {
	private sealed class Compressor : IDisposable {
		private readonly ZstdNet.Compressor Delegator;

		[MethodImpl(MethodImpl.Hot)]
		internal Compressor() : this(Options.CompressionDefault) { }
		[MethodImpl(MethodImpl.Hot)]
		internal Compressor(ZstdNet.CompressionOptions options) => Delegator = new(options);


		[MethodImpl(MethodImpl.Hot)]
		public void Dispose() {
			try {
				Delegator.Dispose();
			}
			catch (DllNotFoundException) {
				// This eliminates an invalid call to a DLL that isn't present.
				GC.SuppressFinalize(Delegator);
			}
		}

		[MethodImpl(MethodImpl.Hot)]
		internal byte[] Wrap(byte[] data) => Delegator.Wrap(data);

		[MethodImpl(MethodImpl.Hot)]
		internal byte[] Wrap(ReadOnlySpan<byte> data) => Delegator.Wrap(data);
	}

	private sealed class Decompressor : IDisposable {
		private readonly ZstdNet.Decompressor Delegate;

		[MethodImpl(MethodImpl.Hot)]
		internal Decompressor() : this(Options.DecompressionDefault) { }
		[MethodImpl(MethodImpl.Hot)]
		internal Decompressor(ZstdNet.DecompressionOptions options) => Delegate = new(options);

		[MethodImpl(MethodImpl.Hot)]
		public void Dispose() => Delegate.Dispose();

		[MethodImpl(MethodImpl.Hot)]
		internal byte[] Unwrap(byte[] data) => Delegate.Unwrap(data);

		[MethodImpl(MethodImpl.Hot)]
		internal byte[] Unwrap(byte[] data, int size) => Delegate.Unwrap(data, size);
	}

	private static bool? IsSupported_ = null;
	internal static bool IsSupported {
		[MethodImpl(MethodImpl.RunOnce)]
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
				Debug.Info("Zstd Compression is supported".Pastel(DrawingColor.LightGreen));
				IsSupported_ = true;
			}
			catch (DllNotFoundException) {
				Debug.Info($"Zstd Compression not supported".Pastel(DrawingColor.LightGreen));
				IsSupported_ = false;
			}
			catch (Exception ex) {
				Debug.Info($"Zstd Compression not supported: '{ex.GetType().Name} {ex.Message}'".Pastel(DrawingColor.Red));
				IsSupported_ = false;
			}

			return IsSupported_.Value;
		}
	}

	private static class Options {
		internal static readonly ZstdNet.CompressionOptions CompressionDefault = new(ZstdNet.CompressionOptions.DefaultCompressionLevel);
		internal static readonly ZstdNet.DecompressionOptions DecompressionDefault = new(null);
	}

	[MethodImpl(MethodImpl.Hot)]
	private static Compressor GetEncoder() => new(Options.CompressionDefault);

	[MethodImpl(MethodImpl.Hot)]
	private static Decompressor GetDecoder() => new(Options.DecompressionDefault);

	[MethodImpl(MethodImpl.RunOnce)]
	private static byte[] CompressTest(byte[] data) {
		Compressor? encoder = null;
		try {
			using (encoder = GetEncoder()) {
				return encoder.Wrap(data);
			}
		}
		catch (DllNotFoundException) when (encoder is not null) {
			GC.SuppressFinalize(encoder);
			throw;
		}
	}

	[MethodImpl(MethodImpl.Hot)]
	internal static byte[] Compress(byte[] data) {
		using var encoder = GetEncoder();
		return encoder.Wrap(data);
	}

	[MethodImpl(MethodImpl.Hot)]
	internal static byte[] Compress(ReadOnlySpan<byte> data) {
		using var encoder = GetEncoder();
		return encoder.Wrap(data);
	}

	[MethodImpl(MethodImpl.Hot)]
	internal static byte[] Decompress(byte[] data) {
		using var decoder = GetDecoder();
		return decoder.Unwrap(data);
	}

	[MethodImpl(MethodImpl.Hot)]
	internal static byte[] Decompress(byte[] data, int size) {
		using var decoder = GetDecoder();
		return decoder.Unwrap(data, size);
	}
}
