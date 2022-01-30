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
using SpriteMaster.Resample.Scalers.SuperXBR.Cg;
using SpriteMaster.Types;
using SpriteMaster.Types.Fixed;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SpriteMaster.Extensions;

static class SpanExt {
	internal static Span<T> MakeUninitialized<T>(int count, bool pinned = false) where T : struct => GC.AllocateUninitializedArray<T>(count, pinned: pinned);

	internal static Span<T> MakePinned<T>(int count) where T : struct => MakeUninitialized<T>(count, pinned: true);

	internal static unsafe Span<T> ToSpan<T>(this UnmanagedMemoryStream stream) where T : unmanaged => new Span<T>(stream.PositionPointer, (int)((stream.Length - stream.Position) / sizeof(T)));

	internal static unsafe ReadOnlySpan<T> ToReadOnlySpan<T>(this UnmanagedMemoryStream stream) where T : unmanaged => new ReadOnlySpan<T>(stream.PositionPointer, (int)((stream.Length - stream.Position) / sizeof(T)));

	internal static Span<T> ToSpanUnsafe<T>(this ReadOnlySpan<T> span) => MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(span), span.Length);

	internal static void CopyTo<T>(this Span<T> inSpan, Span<T> outSpan, int inOffset, int outOffset, int count) =>
		inSpan.Slice(inOffset, count).CopyTo(outSpan.Slice(outOffset, count));

	internal static void CopyTo<T>(this ReadOnlySpan<T> inSpan, Span<T> outSpan, int inOffset, int outOffset, int count) =>
		inSpan.Slice(inOffset, count).CopyTo(outSpan.Slice(outOffset, count));

	internal static Span<T> Cast<T>(this Span<byte> span) where T : unmanaged => span.Cast<byte, T>();
	internal static Span<T> Cast<T>(this Span<int> span) where T : unmanaged => span.Cast<int, T>();
	internal static Span<T> Cast<T>(this Span<uint> span) where T : unmanaged => span.Cast<uint, T>();
	internal static Span<T> Cast<T>(this Span<Color8> span) where T : unmanaged => span.Cast<Color8, T>();
	internal static Span<T> Cast<T>(this Span<Color16> span) where T : unmanaged => span.Cast<Color16, T>();

	internal static ReadOnlySpan<T> Cast<T>(this ReadOnlySpan<byte> span) where T : unmanaged => span.Cast<byte, T>();
	internal static ReadOnlySpan<T> Cast<T>(this ReadOnlySpan<int> span) where T : unmanaged => span.Cast<int, T>();
	internal static ReadOnlySpan<T> Cast<T>(this ReadOnlySpan<uint> span) where T : unmanaged => span.Cast<uint, T>();
	internal static ReadOnlySpan<T> Cast<T>(this ReadOnlySpan<Color8> span) where T : unmanaged => span.Cast<Color8, T>();
	internal static ReadOnlySpan<T> Cast<T>(this ReadOnlySpan<Color16> span) where T : unmanaged => span.Cast<Color16, T>();

	internal static Span<U> AsSpan<T, U>(this T[] array) where T : unmanaged where U : unmanaged => array.AsSpan().Cast<T, U>();
	internal static Span<U> AsSpan<U>(this byte[] array) where U : unmanaged => array.AsSpan().Cast<byte, U>();
	internal static Span<U> AsSpan<U>(this int[] array) where U : unmanaged => array.AsSpan().Cast<int, U>();
	internal static Span<U> AsSpan<U>(this uint[] array) where U : unmanaged => array.AsSpan().Cast<uint, U>();
	internal static Span<U> AsSpan<U>(this Color8[] array) where U : unmanaged => array.AsSpan().Cast<Color8, U>();
	internal static Span<U> AsSpan<U>(this Color16[] array) where U : unmanaged => array.AsSpan().Cast<Color16, U>();

	internal static ReadOnlySpan<U> AsReadOnlySpan<T, U>(this T[] array) where T : unmanaged where U : unmanaged => array.AsSpan().Cast<T, U>();
	internal static ReadOnlySpan<U> AsReadOnlySpan<U>(this byte[] array) where U : unmanaged => array.AsSpan().Cast<byte, U>();
	internal static ReadOnlySpan<U> AsReadOnlySpan<U>(this int[] array) where U : unmanaged => array.AsSpan().Cast<int, U>();
	internal static ReadOnlySpan<U> AsReadOnlySpan<U>(this uint[] array) where U : unmanaged => array.AsSpan().Cast<uint, U>();
	internal static ReadOnlySpan<U> AsReadOnlySpan<U>(this Color8[] array) where U : unmanaged => array.AsSpan().Cast<Color8, U>();
	internal static ReadOnlySpan<U> AsReadOnlySpan<U>(this Color16[] array) where U : unmanaged => array.AsSpan().Cast<Color16, U>();

	internal static Span<Fixed8> Elements(this Span<Color8> span) => span.Cast<Fixed8>();
	internal static ReadOnlySpan<Fixed8> Elements(this ReadOnlySpan<Color8> span) => span.Cast<Fixed8>();

	internal static Span<Fixed16> Elements(this Span<Color16> span) => span.Cast<Fixed16>();
	internal static ReadOnlySpan<Fixed16> Elements(this ReadOnlySpan<Color16> span) => span.Cast<Fixed16>();

	internal static Span<float> Elements(this Span<Float4> span) => span.Cast<Float4, float>();
	internal static ReadOnlySpan<float> Elements(this ReadOnlySpan<Float4> span) => span.Cast<Float4, float>();
}
