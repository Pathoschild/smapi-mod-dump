/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using BenchmarkDotNet.Attributes;
using SpriteMaster.Extensions;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
// ReSharper disable AccessToStaticMemberViaDerivedType

namespace Benchmarks.Arrays.Benchmarks;

public class CloneTextures : Textures {
	[Benchmark(Description = "Array.Clone", Baseline = true)]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ArrayClone(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			data.TempReference = data.Reference.Clone();
		}
	}

	[Benchmark(Description = "Array.CloneAs")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ArrayCloneAs(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			data.TempReference = data.Reference.Clone() as byte[];
		}
	}

	[Benchmark(Description = "Array.Copy (new)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ArrayCopyNew(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = GC.AllocateUninitializedArray<byte>(data.Reference.Length);
			Array.Copy(data.Reference, tempBuffer, data.Reference.Length);
			data.TempReference = tempBuffer;
		}
	}

	[Benchmark(Description = "Array.Copy (Uninitialized)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ArrayCopy(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			Array.Copy(data.Reference, tempBuffer, data.Reference.Length);
			data.TempReference = tempBuffer;
		}
	}

	[Benchmark(Description = "Array.CopyTo")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ArrayCopyTo(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			data.Reference.CopyTo(tempBuffer, 0);
			data.TempReference = tempBuffer;
		}
	}

	[Benchmark(Description = "Buffer.BlockCopy")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void BufferBlockCopy(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			Buffer.BlockCopy(data.Reference, 0, tempBuffer, 0, data.Reference.Length * sizeof(byte));
			data.TempReference = tempBuffer;
		}
	}

	[Benchmark(Description = "Buffer.MemoryCopy")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public unsafe void BufferMemoryCopy(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			fixed (byte* source = data.Reference) {
				fixed (byte* dest = tempBuffer) {
					Buffer.MemoryCopy(source, dest, tempBuffer.LongLength, data.Reference.LongLength);
				}
			}
			data.TempReference = tempBuffer;
		}
	}

	[Benchmark(Description = "Marshal.Copy (source pointer)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public unsafe void MarshalCopySrcPtr(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			fixed (byte* source = data.Reference) {
				Marshal.Copy((IntPtr)source, tempBuffer, 0, data.Reference.Length);
			}
			data.TempReference = tempBuffer;
		}
	}

	[Benchmark(Description = "Marshal.Copy (destination pointer)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public unsafe void MarshalCopyDstPtr(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			fixed (byte* dest = tempBuffer) {
				Marshal.Copy(data.Reference, 0, (IntPtr)dest, data.Reference.Length);
			}
			data.TempReference = tempBuffer;
		}
	}

	[Benchmark(Description = "Unsafe.CopyBlock")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void UnsafeCopyBlock(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];

			Unsafe.CopyBlock(ref MemoryMarshal.GetArrayDataReference(tempBuffer), ref MemoryMarshal.GetArrayDataReference(data.Reference), (uint)data.Reference.LongLength);

			data.TempReference = tempBuffer;
		}
	}

	[Benchmark(Description = "Unsafe.CopyBlock (Pointer)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public unsafe void UnsafeCopyBlockPtr(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			fixed (byte* source = data.Reference) {
				fixed (byte* dest = tempBuffer) {
					Unsafe.CopyBlock(dest, source, (uint)data.Reference.LongLength);
				}
			}

			data.TempReference = tempBuffer;
		}
	}

	[Benchmark(Description = "Span.CopyTo")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void SpanCopyTo(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			data.Reference.AsReadOnlySpan().CopyTo(tempBuffer.AsSpan());
			data.TempReference = tempBuffer;
		}
	}

	[Benchmark(Description = "ForLoop")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ForLoop(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			for (int i = 0; i < data.Reference.Length; ++i) {
				tempBuffer[i] = data.Reference[i];
			}
			data.TempReference = tempBuffer;
		}
	}

	[Benchmark(Description = "FixedForLoop")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public unsafe void FixedForLoop(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			fixed (byte* source = data.Reference) {
				fixed (byte* dest = tempBuffer) {
					for (int i = 0; i < data.Reference.Length; ++i) {
						dest[i] = source[i];
					}
				}
			}
			data.TempReference = tempBuffer;
		}
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
	public void FixedForLoopUnrolled(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			FixedUnrolledCopy(data.Reference, tempBuffer);
			data.TempReference = tempBuffer;
		}
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
	public void NumericVectorLoop(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			NumericVectorCopy(data.Reference, tempBuffer);
			data.TempReference = tempBuffer;
		}
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
	public void NumericVectorLoopUnrolled(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			NumericVectorUnrolledCopy(data.Reference, tempBuffer);
			data.TempReference = tempBuffer;
		}
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
	public void Avx2Loop(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			Avx2Copy(data.Reference, tempBuffer);
			data.TempReference = tempBuffer;
		}
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
	public void Avx2LoopUnrolled(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			Avx2UnrolledCopy(data.Reference, tempBuffer);
			data.TempReference = tempBuffer;
		}
	}

	[DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
	private static extern IntPtr NativeMemcpy(IntPtr dest, IntPtr src, nuint count);


	[Benchmark(Description = "Memcpy (P/Invoke)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public unsafe void Memcpy(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var tempBuffer = new byte[data.Reference.Length];
			fixed (byte* source = data.Reference) {
				fixed (byte* dest = tempBuffer) {
					NativeMemcpy((IntPtr)dest, (IntPtr)source, (nuint)data.Reference.Length);
				}
			}

			data.TempReference = tempBuffer;
		}
	}
}
