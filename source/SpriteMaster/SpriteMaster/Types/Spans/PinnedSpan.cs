/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable UnusedMember.Global

namespace SpriteMaster.Types.Spans;

[DebuggerTypeProxy(typeof(PinnedSpanDebugView<>))]
[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Auto)]
internal readonly ref struct PinnedSpan<T> where T : unmanaged {

	internal static PinnedSpan<T> Empty => new(null);

	internal readonly object ReferenceObject;
	internal readonly Span<T> InnerSpan;

	/// <summary>
	/// Creates a new span over the entirety of the target array.
	/// </summary>
	/// <param name="array">The target array.</param>
	/// <remarks>Returns default when <paramref name="array"/> is null.</remarks>
	[MethodImpl(Runtime.MethodImpl.Inline)]
	private PinnedSpan(T[]? array) :
		this(array!, new(array)) {
	}

	/// <summary>
	/// Creates a new span with the given length.
	/// </summary>
	/// <param name="length">The number of items in the span.</param>
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal PinnedSpan(int length) :
		this(GC.AllocateUninitializedArray<T>(length, pinned: true)) {
	}

	/// <summary>
	/// Creates a new span over the target unmanaged buffer.  Clearly this
	/// is quite dangerous, because we are creating arbitrarily typed T's
	/// out of a void*-typed block of memory.  And the length is not checked.
	/// But if this creation is correct, then all subsequent uses are correct.
	/// </summary>
	/// <param name="refObject">Reference object for garbage collection purposes.</param>
	/// <param name="pointer">An unmanaged pointer to memory.</param>
	/// <param name="length">The number of <typeparamref name="T"/> elements the memory contains.</param>
	/// <exception cref="System.ArgumentException">
	/// Thrown when <typeparamref name="T"/> is reference type or contains pointers and hence cannot be stored in unmanaged memory.
	/// </exception>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// Thrown when the specified <paramref name="length"/> is negative.
	/// </exception>
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal unsafe PinnedSpan(object refObject, void* pointer, int length) :
		this(refObject, new(pointer, length)) {
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal unsafe PinnedSpan(object refObject, ref T pointer, int length) : 
		this(refObject, new(Unsafe.AsPointer(ref pointer), length)) {
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private PinnedSpan(object refObject, Span<T> span) {
		PinnedSpanCommon.CheckPinnedWeak(refObject);
		ReferenceObject = refObject;
		InnerSpan = span;
	}

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static PinnedSpan<T> FromInternal(object refObject, Span<T> span) => new(refObject, span);

	// Most ripped from Span.cs

	/// <summary>
	/// Returns a reference to specified element of the Span.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	/// <exception cref="System.IndexOutOfRangeException">
	/// Thrown when index less than 0 or index greater than or equal to Length
	/// </exception>
	internal ref T this[int index] {
		[Pure]
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get => ref InnerSpan[index];
	}

	/// <summary>
	/// The number of items in the span.
	/// </summary>
	internal int Length {
		[Pure]
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get => InnerSpan.Length;
	}

	/// <summary>
	/// Returns true if Length is 0.
	/// </summary>
	internal bool IsEmpty {
		[Pure]
		[MethodImpl(Runtime.MethodImpl.Inline)]
		get => InnerSpan.IsEmpty;
	}

	/// <summary>
	/// Returns true if left and right point at the same memory and have the same length.  Note that
	/// this does *not* check to see if the *contents* are equal.
	/// </summary>
	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(PinnedSpan<T> left, Span<T> right) => left.InnerSpan == right;

	/// <summary>
	/// Returns true if left and right point at the same memory and have the same length.  Note that
	/// this does *not* check to see if the *contents* are equal.
	/// </summary>
	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator ==(Span<T> left, PinnedSpan<T> right) => left == right.InnerSpan;

	/// <summary>
	/// Returns false if left and right point at the same memory and have the same length.  Note that
	/// this does *not* check to see if the *contents* are equal.
	/// </summary>
	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(PinnedSpan<T> left, Span<T> right) => !(left == right);

	/// <summary>
	/// Returns false if left and right point at the same memory and have the same length.  Note that
	/// this does *not* check to see if the *contents* are equal.
	/// </summary>
	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static bool operator !=(Span<T> left, PinnedSpan<T> right) => !(left == right);

	/// <summary>
	/// This method is not supported as spans cannot be boxed. To compare two spans, use operator==.
	/// <exception cref="System.NotSupportedException">
	/// Always thrown by this method.
	/// </exception>
	/// </summary>
	[Pure]
	[Obsolete("Equals() on Span will always throw an exception. Use == instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MethodImpl(Runtime.MethodImpl.Inline)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
	public override bool Equals(object? obj) => InnerSpan.Equals(obj);
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

	/// <summary>
	/// This method is not supported as spans cannot be boxed.
	/// <exception cref="System.NotSupportedException">
	/// Always thrown by this method.
	/// </exception>
	/// </summary>
	[Pure]
	[Obsolete("GetHashCode() on Span will always throw an exception.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MethodImpl(Runtime.MethodImpl.Inline)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
	public override int GetHashCode() => InnerSpan.GetHashCode();
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

	/// <summary>Gets an enumerator for this span.</summary>
	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	public Span<T>.Enumerator GetEnumerator() => InnerSpan.GetEnumerator();

	/// <summary>
	/// Returns a reference to the 0th element of the Span. If the Span is empty, returns null reference.
	/// It can be used for pinning and is required to support the use of span within a fixed statement.
	/// </summary>
	[Pure]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal ref T GetPinnableReference() => ref InnerSpan.GetPinnableReference();

	[Pure]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal ref T GetPinnableReferenceUnsafe() => ref InnerSpan.GetPinnableReference();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal unsafe T* GetPointer() => (T*)Unsafe.AsPointer(ref GetPinnableReferenceUnsafe());

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal unsafe IntPtr GetIntPointer() => (IntPtr)Unsafe.AsPointer(ref GetPinnableReferenceUnsafe());

	/// <summary>
	/// Clears the contents of this span.
	/// </summary>
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal void Clear() => InnerSpan.Clear();

	/// <summary>
	/// Fills the contents of this span with the given value.
	/// </summary>
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal void Fill(T value) => InnerSpan.Fill(value);

	/// <summary>
	/// Copies the contents of this span into destination span. If the source
	/// and destinations overlap, this method behaves as if the original values in
	/// a temporary location before the destination is overwritten.
	/// </summary>
	/// <param name="destination">The span to copy items into.</param>
	/// <exception cref="System.ArgumentException">
	/// Thrown when the destination Span is shorter than the source Span.
	/// </exception>
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal void CopyTo(Span<T> destination) => InnerSpan.CopyTo(destination);

	/// <summary>
	/// Copies the contents of this span into destination span. If the source
	/// and destinations overlap, this method behaves as if the original values in
	/// a temporary location before the destination is overwritten.
	/// </summary>
	/// <param name="destination">The span to copy items into.</param>
	/// <returns>If the destination span is shorter than the source span, this method
	/// return false and no data is written to the destination.</returns>
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal bool TryCopyTo(Span<T> destination) => InnerSpan.TryCopyTo(destination);

	/// <summary>
	/// Defines an implicit conversion of a <see cref="PinnedSpan{T}"/> to a <see cref="Span{T}"/>
	/// </summary>
	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator Span<T>(PinnedSpan<T> span) => span.InnerSpan;

	/// <summary>
	/// Defines an implicit conversion of a <see cref="PinnedSpan{T}"/> to a <see cref="ReadOnlySpan{T}"/>
	/// </summary>
	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator ReadOnlySpan<T>(PinnedSpan<T> span) => span.InnerSpan;

	/// <summary>
	/// Defines an implicit conversion of a <see cref="PinnedSpan{T}"/> to a <see cref="ReadOnlyPinnedSpan{T}"/>
	/// </summary>
	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	public static implicit operator ReadOnlyPinnedSpan<T>(PinnedSpan<T> span) => ReadOnlyPinnedSpan<T>.FromInternal(span.ReferenceObject, span.InnerSpan);

	/// <summary>
	/// For <see cref="Span{Char}"/>, returns a new instance of string that represents the characters pointed to by the span.
	/// Otherwise, returns a <see cref="string"/> with the name of the type and the number of elements.
	/// </summary>
	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	public override string ToString() => InnerSpan.ToString();

	/// <summary>
	/// Forms a slice out of the given span, beginning at 'start'.
	/// </summary>
	/// <param name="start">The index at which to begin this slice.</param>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// Thrown when the specified <paramref name="start"/> index is not in range (&lt;0 or &gt;Length).
	/// </exception>
	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal PinnedSpan<T> Slice(int start) => new(ReferenceObject, InnerSpan.Slice(start));

	/// <summary>
	/// Forms a slice out of the given span, beginning at 'start', of given length
	/// </summary>
	/// <param name="start">The index at which to begin this slice.</param>
	/// <param name="length">The desired length for the slice (exclusive).</param>
	/// <exception cref="System.ArgumentOutOfRangeException">
	/// Thrown when the specified <paramref name="start"/> or end index is not in range (&lt;0 or &gt;Length).
	/// </exception>
	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal PinnedSpan<T> Slice(int start, int length) => new(ReferenceObject, InnerSpan.Slice(start, length));

	/// <summary>
	/// Copies the contents of this span into a new array.  This heap
	/// allocates, so should generally be avoided, however it is sometimes
	/// necessary to bridge the gap with APIs written in terms of arrays.
	/// </summary>
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal T[] ToArray() => InnerSpan.ToArray();

	[DebuggerTypeProxy(typeof(PinnedSpanDebugView<>))]
	[DebuggerDisplay("{AsSpan.ToString(),raw}")]
	[StructLayout(LayoutKind.Auto)]
	internal unsafe readonly struct FixedSpan {
		private readonly object ReferenceObject;
		internal readonly T* Pointer;
		internal readonly int Length;

		internal FixedSpan(PinnedSpan<T> span) {
			ReferenceObject = span.ReferenceObject;
			Pointer = (T*)Unsafe.AsPointer(ref span.GetPinnableReferenceUnsafe());
			Length = span.Length;
		}

		internal PinnedSpan<T> AsSpan => new(ReferenceObject, Pointer, Length);
	}

	internal FixedSpan Fixed => new(this);
}
