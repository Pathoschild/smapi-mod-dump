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
		private sealed class Compressor : IDisposable {
			private readonly ZstdNet.Compressor Delegate;

			internal Compressor() : this(Zstd.Options.CompressionDefault) {}
			internal Compressor(ZstdNet.CompressionOptions options) {
				Delegate = new ZstdNet.Compressor(options);
			}

			public void Dispose() {
				try {
					Delegate.Dispose();
				}
				catch (DllNotFoundException) {
					// This eliminates an invalid call to a DLL that isn't present.
					GC.SuppressFinalize(Delegate);
				}
			}

			internal byte[] Wrap(byte[] data) {
				return Delegate.Wrap(data);
			}
		}

		private sealed class Decompressor : IDisposable {
			private readonly ZstdNet.Decompressor Delegate;

			internal Decompressor() : this(Zstd.Options.DecompressionDefault) { }
			internal Decompressor(ZstdNet.DecompressionOptions options) {
				Delegate = new ZstdNet.Decompressor(options);
			}

			public void Dispose() {
				try {
					Delegate.Dispose();
				}
				catch (DllNotFoundException) {
					// This eliminates an invalid call to a DLL that isn't present.
					GC.SuppressFinalize(Delegate);
				}
			}

			internal byte[] Unwrap(byte[] data) {
				return Delegate.Unwrap(data);
			}

			internal byte[] Unwrap(byte[] data, int size) {
				return Delegate.Unwrap(data, size);
			}
		}

		internal static bool IsSupported {
			get {
				try {
					var dummyData = new byte[16];
					var compressedData = Compress(dummyData);
					var uncompressedData = Decompress(compressedData, dummyData.Length);
					if (!Enumerable.SequenceEqual(dummyData, uncompressedData)) {
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
					Debug.InfoLn($"Zstd Compression not supported: '{ex.ToString()}'");
					return false;
				}
			}
		}

		private static class Options {
			internal static readonly ZstdNet.CompressionOptions CompressionDefault = new(ZstdNet.CompressionOptions.DefaultCompressionLevel);
			internal static readonly ZstdNet.DecompressionOptions DecompressionDefault = new(null);
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		private static Compressor GetEncoder() {
			return new Compressor(Options.CompressionDefault);
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		private static Decompressor GetDecoder() {
			return new Decompressor(Options.DecompressionDefault);
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static byte[] Compress(byte[] data) {
			using var encoder = GetEncoder();
			return encoder.Wrap(data);
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static unsafe byte[] Decompress(byte[] data) {
			using var decoder = GetDecoder();
			return decoder.Unwrap(data);
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		internal static byte[] Decompress(byte[] data, int size) {
			using var decoder = GetDecoder();
			return decoder.Unwrap(data, size);
		}
	}
}
