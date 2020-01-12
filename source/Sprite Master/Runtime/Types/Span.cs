using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types {
	[ImmutableObject(true)]
	public ref struct Span<T> where T : unmanaged {
		private sealed class CollectionHandle {
			private GCHandle Handle;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public CollectionHandle (GCHandle handle) {
				Handle = handle;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			~CollectionHandle () {
				Handle.Free();
			}
		}

		private readonly CollectionHandle Handle;
		private readonly object PinnedObject;
		private readonly IntPtr Pointer;
		public readonly int Length;
		private readonly int Size;

		private readonly static int TypeSize = Marshal.SizeOf(typeof(T));

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		private readonly int GetOffset(int index) {
			if (index < 0 || index >= Length) {
				throw new IndexOutOfRangeException(nameof(index));
			}

			return index * TypeSize;
		}

		public unsafe T this[int index] {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get {
				T* ptr = (T*)(Pointer + GetOffset(index));
				return *ptr;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set {
				T* ptr = (T*)(Pointer + GetOffset(index));
				*ptr = value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T[] ToArray() {
			var result = new T[Length];
			for (int i = 0; i < Length; ++i) {
				result[i] = this[i];
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe ref T GetPinnableReference () {
			return ref *(T*)Pointer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span(T[] data) : this(data, data.Length, data.Length * TypeSize) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span (T[] data, int length) : this(data, length, length * TypeSize) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span (object pinnedObject, int size) : this(pinnedObject, size / TypeSize, size) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span(object pinnedObject, int length, int size) {
			var handle = GCHandle.Alloc(pinnedObject, GCHandleType.Pinned);
			try {
				PinnedObject = pinnedObject;
				Pointer = handle.AddrOfPinnedObject();
				Length = length;
				Size = size;
			}
			catch {
				handle.Free();
				throw;
			}

			Handle = new CollectionHandle(handle);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe Span (T* data, int length, int size) {
			PinnedObject = null;
			Pointer = (IntPtr)data;
			Length = length;
			Size = size;
			Handle = null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe Span(T* data, int length) : this(data, length, length * TypeSize) {}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<U> As<U>() where U : unmanaged {
			// TODO add check for U == T
			if (PinnedObject == null) {
				return new Span<U>(Pointer, Size / Marshal.SizeOf(typeof(U)), Size);
			}
			else {
				return new Span<U>(PinnedObject, Size);
			}
		}

		public unsafe sealed class Enumerator : IEnumerator<T>, IEnumerator {
			private readonly T* Span;
			private readonly int Length;
			private int Index = 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Enumerator (in Span<T> span) {
				Span = (T *)span.Pointer;
				Length = span.Length;
			}

			public T Current {
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get { return Span[Index]; }
			}

			object IEnumerator.Current {
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get { return Span[Index]; }
			}

			[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose () {}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext () {
				++Index;
				if (Index >= Length) {
					return false;
				}
				return true;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Reset () {
				Index = 0;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly IEnumerator<T> GetEnumerator () {
			return new Enumerator(this);
		}

		/*
		IEnumerator IEnumerable.GetEnumerator () {
			return new Enumerator(this);
		}
		*/
	}
}
