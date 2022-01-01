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

namespace SpriteMaster.Types;

static class Extensions {
	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	internal static FixedSpan<T> AsFixedSpan<T>(this T[] data) where T : unmanaged => new(data);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	internal static FixedSpan<U> AsFixedSpan<T, U>(this T[] data) where T : unmanaged where U : unmanaged {
		using var intermediateSpan = new FixedSpan<T>(data);
		return intermediateSpan.As<U>();
	}

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	internal static FixedSpan<T> AsFixedSpan<T>(this T[] data, int length) where T : unmanaged => new(data, length);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	internal static FixedSpan<U> AsFixedSpan<T, U>(this T[] data, int length) where T : unmanaged where U : unmanaged {
		using var intermediateSpan = new FixedSpan<T>(data, length);
		return intermediateSpan.As<U>();
	}

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] data) where T : unmanaged => new(data);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] data, int length) where T : unmanaged => new(data, 0, length);
}

[ImmutableObject(true)]
struct FixedSpan<T> : IDisposable where T : unmanaged {
	private sealed class CollectionHandle : IDisposable {
		private GCHandle? Handle;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal CollectionHandle(in GCHandle handle) => Handle = handle;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		~CollectionHandle() => Dispose();

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public void Dispose() {
			if (Handle.HasValue) {
				Handle.Value.Free();
				Handle = null;
			}
		}
	}

	private CollectionHandle Handle;
	private WeakReference PinnedObject;
	internal readonly IntPtr Pointer;
	internal readonly int Length;
	private readonly int Size;

	internal readonly unsafe T* TypedPointer => (T*)Pointer;

	internal readonly static int TypeSize = Marshal.SizeOf(typeof(T));

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	private readonly int GetOffset(int index) {
#if DEBUG
		if (index < 0 || index >= Length) {
			throw new IndexOutOfRangeException($"{nameof(index)}: {index} outside [0, {Length}]");
		}
#endif

		return index * TypeSize;
	}

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	private readonly uint GetOffset(uint index) {
#if DEBUG
		if (index >= (uint)Length) {
			throw new IndexOutOfRangeException($"{nameof(index)}: {index} outside [0, {Length}]");
		}
#endif

		return index * (uint)TypeSize;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly unsafe T At(int index) => *(T*)(Pointer + GetOffset(index));

	internal readonly unsafe T this[int index] {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get => *(T*)(Pointer + GetOffset(index));
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => *(T*)(Pointer + GetOffset(index)) = value;
	}

	internal readonly unsafe T this[uint index] {
		[MethodImpl(Runtime.MethodImpl.Hot)]
		get => *(T*)(Pointer + (int)GetOffset(index));
		[MethodImpl(Runtime.MethodImpl.Hot)]
		set => *(T*)(Pointer + (int)GetOffset(index)) = value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly unsafe T[] ToArray(bool copy = false) {
		if (!copy && PinnedObject.Target is T[] array) {
			return array;
		}
		var result = new T[Length];
		fixed (T* outPtr = result) {
			System.Buffer.MemoryCopy(TypedPointer, outPtr, Length * TypeSize, Length * TypeSize);
		}
		return result;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly U[] ToArray<U>(bool copy = false) where U : unmanaged {
		using var span = As<U>();
		return span.ToArray(copy: copy);
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly unsafe ref T GetPinnableReference() => ref *(T*)Pointer;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static implicit operator FixedSpan<T>(T[] array) => array.AsFixedSpan();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal FixedSpan(T[] data) : this(data, data.Length, data.Length * TypeSize) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal FixedSpan(T[] data, int length) : this(data, length, length * TypeSize) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private FixedSpan(object pinnedObject, int size) : this(pinnedObject, size / TypeSize, size) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private FixedSpan(object pinnedObject, int length, int size) {
		var handle = GCHandle.Alloc(pinnedObject, GCHandleType.Pinned);
		try {
			PinnedObject = new WeakReference(pinnedObject);
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

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[Obsolete("Very Unsafe")]
	internal unsafe FixedSpan(T* data, int length, int size) {
		PinnedObject = null;
		Pointer = (IntPtr)data;
		Length = length;
		Size = size;
		Handle = null;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[Obsolete("Very Unsafe")]
	internal unsafe FixedSpan(T* data, int length) : this(data, length, length * TypeSize) { }

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal readonly FixedSpan<U> As<U>() where U : unmanaged {
		// TODO add check for U == T
		if (PinnedObject is null) {
			return new(Pointer, Size / Marshal.SizeOf(typeof(U)), Size);
		}
		else {
			return new(PinnedObject.Target, Size);
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public void Dispose() {
		if (Handle is not null) {
			Handle.Dispose();
			Handle = null;
		}
		if (PinnedObject is not null) {
			PinnedObject.Target = null;
			PinnedObject = null;
		}
	}

	internal unsafe sealed class Enumerator : IEnumerator<T>, IEnumerator {
		private readonly T* Span;
		private readonly int Length;
		private int Index = 0;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		internal Enumerator(in FixedSpan<T> span) {
			Span = (T*)span.Pointer;
			Length = span.Length;
		}

		public T Current => Span[Index];

		object IEnumerator.Current => Span[Index];

		[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
		public void Dispose() { }

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public bool MoveNext() => Index++ < Length;

		[MethodImpl(Runtime.MethodImpl.Hot)]
		public void Reset() => Index = 0;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	public readonly IEnumerator<T> GetEnumerator() => new Enumerator(this);

	/*
	IEnumerator IEnumerable.GetEnumerator () {
		return new Enumerator(this);
	}
	*/
}
