/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using BenchmarkDotNet.Attributes;
using Benchmarks.BenchmarkBase.Benchmarks;
using SpriteMaster.Extensions;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
// ReSharper disable AccessToStaticMemberViaDerivedType

namespace Benchmarks.Arrays.Benchmarks;

public partial class CloneBuffers : BenchmarkBase<DataSetArrayUnfixed<byte>, byte[]> {
	[Benchmark(Description = "Array.Clone", Baseline = true)]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public byte[] ArrayClone(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		return (byte[])dataSet.Data.Clone();
	}

	[Benchmark(Description = "Array.CloneAs")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public byte[] ArrayCloneAs(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		return (dataSet.Data.Clone() as byte[])!;
	}

	[Benchmark(Description = "Array.Copy (new)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public byte[] ArrayCopyNew(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = GC.AllocateUninitializedArray<byte>(dataSet.Data.Length);
		Array.Copy(dataSet.Data, tempBuffer, dataSet.Data.Length);
		return tempBuffer;
	}

	[Benchmark(Description = "Array.Copy (Uninitialized)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public byte[] ArrayCopy(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		Array.Copy(dataSet.Data, tempBuffer, dataSet.Data.Length);
		return tempBuffer;
	}

	[Benchmark(Description = "Array.CopyTo")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public byte[] ArrayCopyTo(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		dataSet.Data.CopyTo(tempBuffer, 0);
		return tempBuffer;
	}

	[Benchmark(Description = "Buffer.BlockCopy")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public byte[] BufferBlockCopy(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		Buffer.BlockCopy(dataSet.Data, 0, tempBuffer, 0, dataSet.Data.Length * sizeof(byte));
		return tempBuffer;
	}

	[Benchmark(Description = "Buffer.MemoryCopy")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public unsafe byte[] BufferMemoryCopy(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		fixed (byte* source = dataSet.Data) {
			fixed (byte* dest = tempBuffer) {
				Buffer.MemoryCopy(source, dest, tempBuffer.LongLength, dataSet.Data.LongLength);
			}
		}
		return tempBuffer;
	}

	[Benchmark(Description = "Marshal.Copy (source pointer)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public unsafe byte[] MarshalCopySrcPtr(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		fixed (byte* source = dataSet.Data) {
			Marshal.Copy((IntPtr)source, tempBuffer, 0, dataSet.Data.Length);
		}
		return tempBuffer;
	}

	[Benchmark(Description = "Marshal.Copy (destination pointer)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public unsafe byte[] MarshalCopyDstPtr(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		fixed (byte* dest = tempBuffer) {
			Marshal.Copy(dataSet.Data, 0, (IntPtr)dest, dataSet.Data.Length);
		}
		return tempBuffer;
	}

	[Benchmark(Description = "Unsafe.CopyBlock")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public byte[] UnsafeCopyBlock(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];

		Unsafe.CopyBlock(ref MemoryMarshal.GetArrayDataReference(tempBuffer), ref MemoryMarshal.GetArrayDataReference(dataSet.Data), (uint)dataSet.Data.LongLength);

		return tempBuffer;
	}

	[Benchmark(Description = "Unsafe.CopyBlock (Pointer)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public unsafe byte[] UnsafeCopyBlockPtr(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		fixed (byte* source = dataSet.Data) {
			fixed (byte* dest = tempBuffer) {
				Unsafe.CopyBlock(dest, source, (uint)dataSet.Data.LongLength);
			}
		}

		return tempBuffer;
	}

	[Benchmark(Description = "Span.CopyTo")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public byte[] SpanCopyTo(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		dataSet.Data.AsReadOnlySpan().CopyTo(tempBuffer.AsSpan());
		return tempBuffer;
	}

	[Benchmark(Description = "ForLoop")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public byte[] ForLoop(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		for (int i = 0; i < dataSet.Data.Length; ++i) {
			tempBuffer[i] = dataSet.Data[i];
		}
		return tempBuffer;
	}

	[Benchmark(Description = "FixedForLoop")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public unsafe byte[] FixedForLoop(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		fixed (byte* source = dataSet.Data) {
			fixed (byte* dest = tempBuffer) {
				for (int i = 0; i < dataSet.Data.Length; ++i) {
					dest[i] = source[i];
				}
			}
		}
		return tempBuffer;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void FixedUnrolledCopy(byte[] sourceArray, byte[] destArray) {
		fixed (byte* source = sourceArray) {
			fixed (byte* dest = destArray) {
				int remaining = sourceArray.Length;
				int i = 0;

				int alignedRemaining = remaining - (sizeof(ulong) - 1);
				for (; i < alignedRemaining; i += sizeof(ulong)) {
					*(ulong*)(dest + i) = *(ulong*)(source + i);
				}

				for (; i < remaining; ++i) {
					dest[i] = source[i];
				}
			}
		}
	}

	[Benchmark(Description = "FixedForLoopUnrolled")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public byte[] FixedForLoopUnrolled(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		FixedUnrolledCopy(dataSet.Data, tempBuffer);
		return tempBuffer;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void NumericVectorCopy(byte[] sourceArray, byte[] destArray) {
		int remaining = sourceArray.Length;
		int i = 0;

		int alignedRemaining = remaining - (Vector<byte>.Count - 1);
		for (; i < alignedRemaining; i += Vector<byte>.Count) {
			var sourceVec = new Vector<byte>(sourceArray, i);
			sourceVec.CopyTo(destArray, i);
		}

		for (; i < remaining; ++i) {
			destArray[i] = sourceArray[i];
		}
	}

	[Benchmark(Description = "NumericVectorLoop")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public byte[] NumericVectorLoop(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		NumericVectorCopy(dataSet.Data, tempBuffer);
		return tempBuffer;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void NumericVectorUnrolledCopy(byte[] sourceArray, byte[] destArray) {
		int remaining = sourceArray.Length;
		int i = 0;

		{
			int alignedRemaining = remaining - ((Vector<byte>.Count * 8) - 1);
			for (; i < alignedRemaining; i += (Vector<byte>.Count * 8)) {
				var sourceVec0 = new Vector<byte>(sourceArray, i + (Vector<byte>.Count * 0));
				var sourceVec1 = new Vector<byte>(sourceArray, i + (Vector<byte>.Count * 1));
				var sourceVec2 = new Vector<byte>(sourceArray, i + (Vector<byte>.Count * 2));
				var sourceVec3 = new Vector<byte>(sourceArray, i + (Vector<byte>.Count * 3));
				var sourceVec4 = new Vector<byte>(sourceArray, i + (Vector<byte>.Count * 4));
				var sourceVec5 = new Vector<byte>(sourceArray, i + (Vector<byte>.Count * 5));
				var sourceVec6 = new Vector<byte>(sourceArray, i + (Vector<byte>.Count * 6));
				var sourceVec7 = new Vector<byte>(sourceArray, i + (Vector<byte>.Count * 7));
				sourceVec0.CopyTo(destArray, i + (Vector<byte>.Count * 0));
				sourceVec1.CopyTo(destArray, i + (Vector<byte>.Count * 1));
				sourceVec2.CopyTo(destArray, i + (Vector<byte>.Count * 2));
				sourceVec3.CopyTo(destArray, i + (Vector<byte>.Count * 3));
				sourceVec4.CopyTo(destArray, i + (Vector<byte>.Count * 4));
				sourceVec5.CopyTo(destArray, i + (Vector<byte>.Count * 5));
				sourceVec6.CopyTo(destArray, i + (Vector<byte>.Count * 6));
				sourceVec7.CopyTo(destArray, i + (Vector<byte>.Count * 7));
			}
		}

		{
			int alignedRemaining = remaining - (Vector<byte>.Count - 1);
			for (; i < alignedRemaining; i += Vector<byte>.Count) {
				var sourceVec = new Vector<byte>(sourceArray, i);
				sourceVec.CopyTo(destArray, i);
			}
		}

		for (; i < remaining; ++i) {
			destArray[i] = sourceArray[i];
		}
	}

	[Benchmark(Description = "NumericVectorLoop (Unrolled)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public byte[] NumericVectorLoopUnrolled(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		NumericVectorUnrolledCopy(dataSet.Data, tempBuffer);
		return tempBuffer;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void Avx2Copy(byte[] sourceArray, byte[] destArray) {
		const int NumElements = 32;

		int remaining = sourceArray.Length;
		int i = 0;

		fixed (byte* source = sourceArray) {
			fixed (byte* dest = destArray) {
				int alignedRemaining = remaining - (NumElements - 1);
				for (; i < alignedRemaining; i += NumElements) {
					var sourceVec = Avx2.LoadVector256(source + i);
					Avx2.Store(dest + i, sourceVec);
				}
			}
		}

		for (; i < remaining; ++i) {
			destArray[i] = sourceArray[i];
		}
	}

	[Benchmark(Description = "Avx2Loop")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public byte[] Avx2Loop(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		Avx2Copy(dataSet.Data, tempBuffer);
		return tempBuffer;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void Avx2UnrolledCopy(byte[] sourceArray, byte[] destArray) {
		const int NumElementsPerVector = 32;
		const int NumElements = NumElementsPerVector * 8;

		int remaining = sourceArray.Length;
		int i = 0;

		fixed (byte* source = sourceArray) {
			fixed (byte* dest = destArray) {
				int alignedRemaining = remaining - (NumElements - 1);
				for (; i < alignedRemaining; i += NumElements) {
					var sourceVec0 = Avx2.LoadVector256(source + i + (NumElementsPerVector * 0));
					var sourceVec1 = Avx2.LoadVector256(source + i + (NumElementsPerVector * 1));
					var sourceVec2 = Avx2.LoadVector256(source + i + (NumElementsPerVector * 2));
					var sourceVec3 = Avx2.LoadVector256(source + i + (NumElementsPerVector * 3));
					var sourceVec4 = Avx2.LoadVector256(source + i + (NumElementsPerVector * 4));
					var sourceVec5 = Avx2.LoadVector256(source + i + (NumElementsPerVector * 5));
					var sourceVec6 = Avx2.LoadVector256(source + i + (NumElementsPerVector * 6));
					var sourceVec7 = Avx2.LoadVector256(source + i + (NumElementsPerVector * 7));
					Avx2.Store(dest + i + (NumElementsPerVector * 0), sourceVec0);
					Avx2.Store(dest + i + (NumElementsPerVector * 1), sourceVec1);
					Avx2.Store(dest + i + (NumElementsPerVector * 2), sourceVec2);
					Avx2.Store(dest + i + (NumElementsPerVector * 3), sourceVec3);
					Avx2.Store(dest + i + (NumElementsPerVector * 4), sourceVec4);
					Avx2.Store(dest + i + (NumElementsPerVector * 5), sourceVec5);
					Avx2.Store(dest + i + (NumElementsPerVector * 6), sourceVec6);
					Avx2.Store(dest + i + (NumElementsPerVector * 7), sourceVec7);
				}
			}
		}

		for (; i < remaining; ++i) {
			destArray[i] = sourceArray[i];
		}
	}

	[Benchmark(Description = "Avx2Loop (Unrolled)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public byte[] Avx2LoopUnrolled(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		Avx2UnrolledCopy(dataSet.Data, tempBuffer);
		return tempBuffer;
	}

	[DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
	private static extern IntPtr NativeMemcpy(IntPtr dest, IntPtr src, nuint count);


	[Benchmark(Description = "Memcpy (P/Invoke)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public unsafe byte[] Memcpy(in DataSetArrayUnfixed<byte> dataSet) {
		if (dataSet.Data.Length == 0) {
			return Array.Empty<byte>();
		}

		var tempBuffer = new byte[dataSet.Data.Length];
		fixed (byte* source = dataSet.Data) {
			fixed (byte* dest = tempBuffer) {
				NativeMemcpy((IntPtr)dest, (IntPtr)source, (nuint)dataSet.Data.Length);
			}
		}

		return tempBuffer;
	}
}
