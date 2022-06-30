/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using LinqFasterer;
using Microsoft.Toolkit.HighPerformance;
using SpriteMaster.Extensions;
using System;
using System.IO;
using System.Runtime.CompilerServices;

using IOC = System.IO.Compression;

namespace SpriteMaster.Compressors;

//[HarmonizeFinalizeCatcher<IOC.DeflateStream, DllNotFoundException>(critical: false)]
internal static class SystemIo {

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

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int CompressedLengthEstimate(ReadOnlySpan<byte> data) => data.Length >> 1;

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int DecompressedLengthEstimate(byte[] data) => data.Length << 1;

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal static long CompressedLengthMax(ReadOnlySpan<byte> data) => 11L + data.Length + (data.Length >> 3) + (data.Length >> 7);

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

	[Pure, MustUseReturnValue]
	internal static byte[] Compress(byte[] data) {
		using var val = new MemoryStream(CompressedLengthEstimate(data));
		using (var compressor = new IOC.DeflateStream(val, IOC.CompressionLevel.Optimal)) {
			compressor.Write(data, 0, data.Length);
		}
		return val.GetArray();
	}

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte[] CompressBytes(ReadOnlySpan<byte> data) {
		using var val = new MemoryStream(CompressedLengthEstimate(data));
		using (var compressor = new IOC.DeflateStream(val, IOC.CompressionLevel.Optimal)) {
			compressor.Write(data);
		}
		return val.GetArray();
	}

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe long AlignCount<T>(long size) where T : unmanaged =>
		(long)(((ulong)size + (ulong)(sizeof(T) - 1)) / (ulong)sizeof(T));

	[Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe int AlignCount<T>(int size) where T : unmanaged =>
		(int)(((long)size + (sizeof(T) - 1)) / sizeof(T));

	[Pure, MustUseReturnValue]
	internal static unsafe T[] Compress<T>(ReadOnlySpan<byte> data) where T : unmanaged {
		if (typeof(T) == typeof(byte)) {
			return (T[])(object)CompressBytes(data);
		}

		long requiredSize = CompressedLengthMax(data);
		long capacity = AlignCount<T>(requiredSize);
		if (capacity > int.MaxValue) {
			var resultArray = CompressBytes(data);
			T[] copiedResult = GC.AllocateUninitializedArray<T>(AlignCount<T>(resultArray.Length));
			resultArray.AsReadOnlySpan().CopyTo(copiedResult.AsSpan().AsBytes());
			return copiedResult;
		}

		T[] result = GC.AllocateUninitializedArray<T>((int)capacity);
		long resultLength;

		fixed (T* resultPtr = result) {
			using var resultStream = new UnmanagedMemoryStream((byte*)resultPtr, result.Length * sizeof(T));

			using var compressor = new IOC.DeflateStream(resultStream, IOC.CompressionLevel.Optimal);
			compressor.Write(data.ToArray(), 0, data.Length);
			compressor.Flush();
			resultLength = resultStream.Position;
		}

		if (result.Length != resultLength) {
			Array.Resize(ref result, (int)resultLength);
		}

		return result;
	}

	[Pure, MustUseReturnValue]
	internal static byte[] Decompress(byte[] data) {
		using var dataStream = new MemoryStream(data);
		using var val = new MemoryStream(DecompressedLengthEstimate(data));
		using (var compressor = new IOC.DeflateStream(dataStream, IOC.CompressionMode.Decompress)) {
			compressor.CopyTo(val);
		}
		return val.GetArray();
	}

	[Pure, MustUseReturnValue]
	internal static byte[] Decompress(byte[] data, int size) {
		using var dataStream = new MemoryStream(data);
		var output = new byte[size];
		using var val = new MemoryStream(output);
		using var compressor = new IOC.DeflateStream(dataStream, IOC.CompressionMode.Decompress);
		compressor.CopyTo(val);
		return output;
	}

	[Pure, MustUseReturnValue]
	internal static unsafe T[] Decompress<T>(byte[] data, int size) where T : unmanaged{
		if (typeof(T) == typeof(byte)) {
			return (T[])(object)Decompress(data, size);
		}

		using var dataStream = new MemoryStream(data);
		var output = GC.AllocateUninitializedArray<T>(AlignCount<T>(size));
		fixed (T* outputPtr = output) {
			using var val = new UnmanagedMemoryStream((byte*)outputPtr, output.Length * sizeof(T));
			using var compressor = new IOC.DeflateStream(dataStream, IOC.CompressionMode.Decompress);
			compressor.CopyTo(val);
		}

		return output;
	}
}
