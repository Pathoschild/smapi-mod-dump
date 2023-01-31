/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Toolkit.HighPerformance;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MusicMaster.Extensions;

internal static class SpanExt {
	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Span<T> Make<T>(int count) where T : struct =>
		GC.AllocateUninitializedArray<T>(count, pinned: false);

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe Span<T> ToSpan<T>(this UnmanagedMemoryStream stream) where T : unmanaged =>
		new(stream.PositionPointer, (int)((stream.Length - stream.Position) / sizeof(T)));

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe ReadOnlySpan<T> ToReadOnlySpan<T>(this UnmanagedMemoryStream stream) where T : unmanaged =>
		new(stream.PositionPointer, (int)((stream.Length - stream.Position) / sizeof(T)));

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Span<T> ToSpanUnsafe<T>(this ReadOnlySpan<T> span) =>
		MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(span), span.Length);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void CopyTo<T>(this Span<T> inSpan, Span<T> outSpan, int inOffset, int outOffset, int count) where T : unmanaged =>
		inSpan.Slice(inOffset, count).CopyTo(outSpan.Slice(outOffset, count));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void CopyTo<T>(this ReadOnlySpan<T> inSpan, Span<T> outSpan, int inOffset, int outOffset, int count) where T : unmanaged =>
		inSpan.Slice(inOffset, count).CopyTo(outSpan.Slice(outOffset, count));

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Span<T> Cast<T>(this Span<byte> span) where T : unmanaged =>
		span.Cast<byte, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Span<T> Cast<T>(this Span<int> span) where T : unmanaged =>
		span.Cast<int, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Span<T> Cast<T>(this Span<uint> span) where T : unmanaged =>
		span.Cast<uint, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlySpan<T> Cast<T>(this ReadOnlySpan<byte> span) where T : unmanaged =>
		span.Cast<byte, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlySpan<T> Cast<T>(this ReadOnlySpan<int> span) where T : unmanaged =>
		span.Cast<int, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlySpan<T> Cast<T>(this ReadOnlySpan<uint> span) where T : unmanaged =>
		span.Cast<uint, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Span<U> AsSpan<T, U>(this T[] array) where T : unmanaged where U : unmanaged =>
		array.AsSpan().Cast<T, U>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Span<U> AsSpan<U>(this byte[] array) where U : unmanaged =>
		array.AsSpan().Cast<byte, U>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Span<U> AsSpan<U>(this int[] array) where U : unmanaged =>
		array.AsSpan().Cast<int, U>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Span<U> AsSpan<U>(this uint[] array) where U : unmanaged =>
		array.AsSpan().Cast<uint, U>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array) =>
		array;

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array, int start) =>
		new(array, start, array.Length - start);

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array, int start, int length) =>
		new(array, start, length);

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlySpan<U> AsReadOnlySpan<T, U>(this T[] array) where T : unmanaged where U : unmanaged =>
		array.AsSpan().Cast<T, U>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlySpan<U> AsReadOnlySpan<U>(this byte[] array) where U : unmanaged =>
		array.AsSpan().Cast<byte, U>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlySpan<U> AsReadOnlySpan<U>(this int[] array) where U : unmanaged =>
		array.AsSpan().Cast<int, U>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlySpan<U> AsReadOnlySpan<U>(this uint[] array) where U : unmanaged =>
		array.AsSpan().Cast<uint, U>();
}
