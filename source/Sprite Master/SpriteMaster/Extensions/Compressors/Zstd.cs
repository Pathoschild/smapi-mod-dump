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
using System.Runtime.CompilerServices;

using static SpriteMaster.Runtime;

namespace SpriteMaster.Extensions.Compressors;
// TODO : Implement a continual training dictionary so each stream doesn't require its own dictionary for in-memory compression.
static class Zstd {
	private sealed class Compressor : IDisposable {
		private readonly ZstdNet.Compressor Delegate;

		[MethodImpl(MethodImpl.Hot)]
		internal Compressor() : this(Zstd.Options.CompressionDefault) { }
		[MethodImpl(MethodImpl.Hot)]
		internal Compressor(ZstdNet.CompressionOptions options) {
			Delegate = new ZstdNet.Compressor(options);
		}

		[MethodImpl(MethodImpl.Hot)]
		public void Dispose() {
			try {
				Delegate.Dispose();
			}
			catch (DllNotFoundException) {
				// This eliminates an invalid call to a DLL that isn't present.
				GC.SuppressFinalize(Delegate);
			}
		}

		[MethodImpl(MethodImpl.Hot)]
		internal byte[] Wrap(byte[] data) {
			return Delegate.Wrap(data);
		}
	}

	private sealed class Decompressor : IDisposable {
		private readonly ZstdNet.Decompressor Delegate;

		[MethodImpl(MethodImpl.Hot)]
		internal Decompressor() : this(Zstd.Options.DecompressionDefault) { }
		[MethodImpl(MethodImpl.Hot)]
		internal Decompressor(ZstdNet.DecompressionOptions options) {
			Delegate = new ZstdNet.Decompressor(options);
		}

		[MethodImpl(MethodImpl.Hot)]
		public void Dispose() {
			try {
				Delegate.Dispose();
			}
			catch (DllNotFoundException) {
				// This eliminates an invalid call to a DLL that isn't present.
				GC.SuppressFinalize(Delegate);
			}
		}

		[MethodImpl(MethodImpl.Hot)]
		internal byte[] Unwrap(byte[] data) {
			return Delegate.Unwrap(data);
		}

		[MethodImpl(MethodImpl.Hot)]
		internal byte[] Unwrap(byte[] data, int size) {
			return Delegate.Unwrap(data, size);
		}
	}

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
				if (Config.Debug.MacOSTestMode) {
					throw new Exception("Mac OS Test Mode Enabled, Zstd not supported");
				}
				Debug.InfoLn("Zstd Compression is supported");
				return true;
			}
			catch (DllNotFoundException) {
				Debug.InfoLn($"Zstd Compression not supported");
				return false;
			}
			catch (Exception ex) {
				Debug.InfoLn($"Zstd Compression not supported: '{ex}'");
				return false;
			}
		}
	}

	private static class Options {
		internal static readonly ZstdNet.CompressionOptions CompressionDefault = new(ZstdNet.CompressionOptions.DefaultCompressionLevel);
		internal static readonly ZstdNet.DecompressionOptions DecompressionDefault = new(null);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static Compressor GetEncoder() {
		return new Compressor(Options.CompressionDefault);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static Decompressor GetDecoder() {
		return new Decompressor(Options.DecompressionDefault);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte[] Compress(byte[] data) {
		using var encoder = GetEncoder();
		return encoder.Wrap(data);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe byte[] Decompress(byte[] data) {
		using var decoder = GetDecoder();
		return decoder.Unwrap(data);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static byte[] Decompress(byte[] data, int size) {
		using var decoder = GetDecoder();
		return decoder.Unwrap(data, size);
	}
}
