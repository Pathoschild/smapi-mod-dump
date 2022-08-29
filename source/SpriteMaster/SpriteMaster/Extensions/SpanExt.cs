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
using Microsoft.Toolkit.HighPerformance;
#if !SHIPPING
using SpriteMaster.Resample.Scalers.SuperXBR.Cg;
#endif
using SpriteMaster.Types;
using SpriteMaster.Types.Fixed;
using SpriteMaster.Types.Spans;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Extensions;

internal static class SpanExt {
	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Span<T> Make<T>(int count) where T : struct =>
		GC.AllocateUninitializedArray<T>(count, pinned: false);

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static PinnedSpan<T> MakePinned<T>(int count) where T : unmanaged =>
		new(count);

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

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void CopyTo<T>(this PinnedSpan<T> inSpan, Span<T> outSpan, int inOffset, int outOffset, int count) where T : unmanaged =>
		inSpan.Slice(inOffset, count).CopyTo(outSpan.Slice(outOffset, count));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static void CopyTo<T>(this ReadOnlyPinnedSpan<T> inSpan, Span<T> outSpan, int inOffset, int outOffset, int count) where T : unmanaged =>
		inSpan.Slice(inOffset, count).CopyTo(outSpan.Slice(outOffset, count));

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static PinnedSpan<TTo> Cast<TFrom, TTo>(this PinnedSpan<TFrom> span) where TFrom : unmanaged where TTo : unmanaged =>
		PinnedSpan<TTo>.FromInternal(span.ReferenceObject, span.InnerSpan.Cast<TFrom, TTo>());

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlyPinnedSpan<TTo> Cast<TFrom, TTo>(this ReadOnlyPinnedSpan<TFrom> span) where TFrom : unmanaged where TTo : unmanaged =>
		ReadOnlyPinnedSpan<TTo>.FromInternal(span.ReferenceObject, span.InnerSpan.Cast<TFrom, TTo>());

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
	internal static Span<T> Cast<T>(this Span<Color8> span) where T : unmanaged =>
		span.Cast<Color8, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Span<T> Cast<T>(this Span<Color16> span) where T : unmanaged =>
		span.Cast<Color16, T>();

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
	internal static ReadOnlySpan<T> Cast<T>(this ReadOnlySpan<Color8> span) where T : unmanaged =>
		span.Cast<Color8, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlySpan<T> Cast<T>(this ReadOnlySpan<Color16> span) where T : unmanaged =>
		span.Cast<Color16, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static PinnedSpan<T> Cast<T>(this PinnedSpan<byte> span) where T : unmanaged =>
		span.Cast<byte, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static PinnedSpan<T> Cast<T>(this PinnedSpan<int> span) where T : unmanaged =>
		span.Cast<int, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static PinnedSpan<T> Cast<T>(this PinnedSpan<uint> span) where T : unmanaged =>
		span.Cast<uint, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static PinnedSpan<T> Cast<T>(this PinnedSpan<Color8> span) where T : unmanaged =>
		span.Cast<Color8, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static PinnedSpan<T> Cast<T>(this PinnedSpan<Color16> span) where T : unmanaged =>
		span.Cast<Color16, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlyPinnedSpan<T> Cast<T>(this ReadOnlyPinnedSpan<byte> span) where T : unmanaged =>
		span.Cast<byte, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlyPinnedSpan<T> Cast<T>(this ReadOnlyPinnedSpan<int> span) where T : unmanaged =>
		span.Cast<int, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlyPinnedSpan<T> Cast<T>(this ReadOnlyPinnedSpan<uint> span) where T : unmanaged =>
		span.Cast<uint, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlyPinnedSpan<T> Cast<T>(this ReadOnlyPinnedSpan<Color8> span) where T : unmanaged =>
		span.Cast<Color8, T>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlyPinnedSpan<T> Cast<T>(this ReadOnlyPinnedSpan<Color16> span) where T : unmanaged =>
		span.Cast<Color16, T>();

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
	internal static Span<U> AsSpan<U>(this Color8[] array) where U : unmanaged =>
		array.AsSpan().Cast<Color8, U>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Span<U> AsSpan<U>(this Color16[] array) where U : unmanaged =>
		array.AsSpan().Cast<Color16, U>();

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

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlySpan<U> AsReadOnlySpan<U>(this Color8[] array) where U : unmanaged =>
		array.AsSpan().Cast<Color8, U>();

	[Pure]
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlySpan<U> AsReadOnlySpan<U>(this Color16[] array) where U : unmanaged =>
		array.AsSpan().Cast<Color16, U>();

	internal static Span<Fixed8> Elements(this Span<Color8> span) => span.Cast<Fixed8>();
	internal static ReadOnlySpan<Fixed8> Elements(this ReadOnlySpan<Color8> span) => span.Cast<Fixed8>();
	internal static PinnedSpan<Fixed8> Elements(this PinnedSpan<Color8> span) => span.Cast<Fixed8>();
	internal static ReadOnlyPinnedSpan<Fixed8> Elements(this ReadOnlyPinnedSpan<Color8> span) => span.Cast<Fixed8>();

	internal static Span<Fixed16> Elements(this Span<Color16> span) => span.Cast<Fixed16>();
	internal static ReadOnlySpan<Fixed16> Elements(this ReadOnlySpan<Color16> span) => span.Cast<Fixed16>();
	internal static PinnedSpan<Fixed16> Elements(this PinnedSpan<Color16> span) => span.Cast<Fixed16>();
	internal static ReadOnlyPinnedSpan<Fixed16> Elements(this ReadOnlyPinnedSpan<Color16> span) => span.Cast<Fixed16>();

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal static PinnedSpan<byte> AsBytes<T>(this PinnedSpan<T> span) where T : unmanaged =>
		span.Cast<T, byte>();

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ReadOnlyPinnedSpan<byte> AsBytes<T>(this ReadOnlyPinnedSpan<T> span) where T : unmanaged =>
		span.Cast<T, byte>();

#if !SHIPPING
	internal static Span<float> Elements(this Span<Float4> span) => span.Cast<Float4, float>();
	internal static ReadOnlySpan<float> Elements(this ReadOnlySpan<Float4> span) => span.Cast<Float4, float>();
	internal static PinnedSpan<float> Elements(this PinnedSpan<Float4> span) => span.Cast<Float4, float>();
	internal static ReadOnlyPinnedSpan<float> Elements(this ReadOnlyPinnedSpan<Float4> span) => span.Cast<Float4, float>();
#endif
}
