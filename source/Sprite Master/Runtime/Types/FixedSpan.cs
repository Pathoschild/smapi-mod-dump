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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types {
	public static class Extensions {
		public static FixedSpan<T> AsFixedSpan<T>(this T[] data) where T : unmanaged {
			return new FixedSpan<T>(data);
		}

		public static FixedSpan<T> AsFixedSpan<T>(this T[] data, int length) where T : unmanaged {
			return new FixedSpan<T>(data, length);
		}
	}

	[ImmutableObject(true)]
	public ref struct FixedSpan<T> where T : unmanaged {
		private sealed class CollectionHandle {
			private GCHandle Handle;

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			public CollectionHandle (GCHandle handle) {
				Handle = handle;
			}

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			~CollectionHandle () {
				Handle.Free();
			}
		}

		private readonly CollectionHandle Handle;
		private readonly object PinnedObject;
		public readonly IntPtr Pointer;
		public readonly int Length;
		private readonly int Size;

		private readonly static int TypeSize = Marshal.SizeOf(typeof(T));

		[Pure, MethodImpl(Runtime.MethodImpl.Optimize)]
		private readonly int GetOffset(int index) {
#if DEBUG
			if (index < 0 || index >= Length) {
				throw new IndexOutOfRangeException(nameof(index));
			}
#endif

			return index * TypeSize;
		}

		[Pure, MethodImpl(Runtime.MethodImpl.Optimize)]
		private readonly uint GetOffset (uint index) {
#if DEBUG
			if (index >= unchecked((uint)Length)) {
				throw new IndexOutOfRangeException(nameof(index));
			}
#endif

			return index * unchecked((uint)TypeSize);
		}

		public unsafe T this[int index] {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			readonly get {
				T* ptr = (T*)(Pointer + GetOffset(index));
				return *ptr;
			}
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			set {
				T* ptr = (T*)(Pointer + GetOffset(index));
				*ptr = value;
			}
		}

		public unsafe T this[uint index] {
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			readonly get {
				T* ptr = (T*)(Pointer + unchecked((int)GetOffset(index)));
				return *ptr;
			}
			[MethodImpl(Runtime.MethodImpl.Optimize)]
			set {
				T* ptr = (T*)(Pointer + unchecked((int)GetOffset(index)));
				*ptr = value;
			}
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public T[] ToArray() {
			var result = new T[Length];
			for (int i = 0; i < Length; ++i) {
				result[i] = this[i];
			}
			return result;
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly unsafe ref T GetPinnableReference () {
			return ref *(T*)Pointer;
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public FixedSpan(T[] data) : this(data, data.Length, data.Length * TypeSize) {}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public FixedSpan (T[] data, int length) : this(data, length, length * TypeSize) { }

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		private FixedSpan (object pinnedObject, int size) : this(pinnedObject, size / TypeSize, size) { }

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		private FixedSpan(object pinnedObject, int length, int size) {
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

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public unsafe FixedSpan (T* data, int length, int size) {
			PinnedObject = null;
			Pointer = (IntPtr)data;
			Length = length;
			Size = size;
			Handle = null;
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public unsafe FixedSpan(T* data, int length) : this(data, length, length * TypeSize) {}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public FixedSpan<U> As<U>() where U : unmanaged {
			// TODO add check for U == T
			if (PinnedObject == null) {
				return new FixedSpan<U>(Pointer, Size / Marshal.SizeOf(typeof(U)), Size);
			}
			else {
				return new FixedSpan<U>(PinnedObject, Size);
			}
		}

		public unsafe sealed class Enumerator : IEnumerator<T>, IEnumerator {
			private readonly T* Span;
			private readonly int Length;
			private int Index = 0;

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			public Enumerator (in FixedSpan<T> span) {
				Span = (T *)span.Pointer;
				Length = span.Length;
			}

			public T Current {
				[MethodImpl(Runtime.MethodImpl.Optimize)]
				get { return Span[Index]; }
			}

			object IEnumerator.Current {
				[MethodImpl(Runtime.MethodImpl.Optimize)]
				get { return Span[Index]; }
			}

			[Pure, MethodImpl(Runtime.MethodImpl.Optimize)]
			public void Dispose () {}

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			public bool MoveNext () {
				++Index;
				if (Index >= Length) {
					return false;
				}
				return true;
			}

			[MethodImpl(Runtime.MethodImpl.Optimize)]
			public void Reset () => Index = 0;
		}

		[MethodImpl(Runtime.MethodImpl.Optimize)]
		public readonly IEnumerator<T> GetEnumerator () => new Enumerator(this);

		/*
		IEnumerator IEnumerable.GetEnumerator () {
			return new Enumerator(this);
		}
		*/
	}
}
