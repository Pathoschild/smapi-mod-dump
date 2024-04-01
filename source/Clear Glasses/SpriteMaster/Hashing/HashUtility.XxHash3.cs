/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using Microsoft.Toolkit.HighPerformance;
using SpriteMaster.Extensions;
using SpriteMaster.Hashing.Algorithms;
using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Hashing;

internal static partial class HashUtility {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong HashXx3(this byte[] data) =>
		XxHash3.Hash64(data);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong HashXx3(this ReadOnlySequence<byte> data) {
		if (data.Length == 0) {
			return Constants.Bits64.Default;
		}

		var firstSpan = data.FirstSpan;
		ulong currentHash = firstSpan.HashXx3();
		foreach (var seq in data.Slice(firstSpan.Length, data.Length - firstSpan.Length)) {
			currentHash = Combine(currentHash, seq.Span.HashXx3());
		}
		return currentHash;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong HashXx3(this Span2D<byte> data) =>
		XxHash3.Hash64(data);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong HashXx3(this ReadOnlySpan2D<byte> data) =>
		XxHash3.Hash64(data);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong HashXx3(this ReadOnlySpan<byte> data) =>
		XxHash3.Hash64(data);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong HashXx3(this Span<byte> data) =>
		XxHash3.Hash64(data);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe ulong HashXx3(byte* data, int length) =>
		XxHash3.Hash64(data, length);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong HashXx3(this byte[] data, int start, int length) =>
		XxHash3.Hash64(new ReadOnlySpan<byte>(data, start, length));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong HashXx3(this Stream stream) {
		using var mStream = new MemoryStream();
		stream.CopyTo(mStream);
		return XxHash3.Hash64(mStream.ToArray());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong HashXx3(this MemoryStream stream) =>
		XxHash3.Hash64(stream.GetArray());

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong HashXx3(this UnmanagedMemoryStream stream) =>
		stream.ToReadOnlySpan<byte>().HashXx3();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong HashXx3<T>(this T[] data) where T : unmanaged =>
		new Span<T>(data).Cast<T, byte>().HashXx3();
}
