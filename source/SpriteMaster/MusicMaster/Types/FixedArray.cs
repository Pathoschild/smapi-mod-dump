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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MusicMaster.Types;

[StructLayout(LayoutKind.Auto)]
internal readonly unsafe struct FixedArray<T> : IReadOnlyList<T> where T : unmanaged {
	internal readonly T[] Array;
	internal readonly T* Pointer;

	internal readonly int Count => Array.Length;
	internal readonly int Length => Array.Length;
	internal readonly long LongLength => Array.LongLength;

	internal readonly ref T this[int index] {
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get => ref Pointer[index];
	}

	internal FixedArray(int length) {
		Array = GC.AllocateUninitializedArray<T>(length, pinned: true);
		Pointer = (T*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(Array));
	}

	public readonly struct Enumerator : IEnumerator<T>, IEnumerator {
		private readonly T* Pointer;
		private readonly int Length;
		private readonly int Index = 0;

		public readonly T Current => Pointer[Index - 1];

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal Enumerator(in FixedArray<T> array) {
			Pointer = array.Pointer;
			Length = array.Length;
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		public readonly void Dispose() {}

		[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
		public readonly bool MoveNext() {
			if ((uint)Index < (uint)Length) {
				++Unsafe.AsRef(Index);
				return true;
			}

			return MoveNextRare();
		}

		[MustUseReturnValue, MethodImpl(MethodImplOptions.NoInlining)]
		private readonly bool MoveNextRare() {
			Unsafe.AsRef(Index) = Length + 1;
			return false;
		}

		readonly object? IEnumerator.Current => Current;

		[MethodImpl(Runtime.MethodImpl.Inline)]
		readonly void IEnumerator.Reset() {
			Unsafe.AsRef(Index) = 0;
		}
	}

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly T[] GetEnumerable() => Array;

	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	public readonly Enumerator GetEnumerator() => new Enumerator(this);
	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);
	[MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	readonly IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);
	readonly int IReadOnlyCollection<T>.Count => Count;
	readonly T IReadOnlyList<T>.this[int index] => Pointer[index];
}
