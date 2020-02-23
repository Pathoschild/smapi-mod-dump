using SpriteMaster.Extensions;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SpriteMaster.Types {
	public sealed class DoubleBuffer<T> {
		public const uint BufferCount = 2;
		public const int StartingIndex = 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetSafeIndex(int index) {
			return index & 1;
		}

		public static readonly Type<T> BufferType = Type<T>.This;

		// Regarding bounds checking on x86 and x64
		// https://stackoverflow.com/questions/16713076/array-bounds-check-efficiency-in-net-4-and-above
		private readonly T[] BufferedObjects;

		private volatile int _CurrentBuffer = StartingIndex;
		public int CurrentBuffer {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => GetSafeIndex(_CurrentBuffer);
		}

		public int NextBuffer {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => GetSafeIndex(unchecked(_CurrentBuffer + 1));
		}

		public T Current {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => BufferedObjects[CurrentBuffer];
		}
		public T Next {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => BufferedObjects[NextBuffer];
		}

		public Tuple<T, T> Both {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				var currentIndex = _CurrentBuffer;
				return Tuple.Create(
					BufferedObjects[GetSafeIndex(currentIndex)],
					BufferedObjects[GetSafeIndex(unchecked(currentIndex + 1))]
				);
			}
		}

		public Tuple<T, T> All => Both;
		public DoubleBuffer(in T element0, in T element1) {
			BufferedObjects = new [] {
				element0,
				element1
			};
			Thread.MemoryBarrier();
		}

		public DoubleBuffer (T[] elements) {
			Contract.Requires(elements.Length >= 2);
			BufferedObjects = (elements.Length == 2) ?
				elements :
				new[] { elements[0], elements[1] };
			Thread.MemoryBarrier();
		}

		public DoubleBuffer (params object[] parameters) : this(
			// We do, indeed, want to create two seperate instances.
			Reflection.CreateInstance<T>(parameters),
			Reflection.CreateInstance<T>(parameters)
		) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Swap() {
			unchecked {
				++_CurrentBuffer;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SwapAtomic () {
			unchecked {
				Interlocked.Increment(ref _CurrentBuffer);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator T (in DoubleBuffer<T> buffer) {
			return buffer.Current;
		}
	}
}
