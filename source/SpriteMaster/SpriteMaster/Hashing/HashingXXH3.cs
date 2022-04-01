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
using SpriteMaster.Extensions;
using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;

namespace SpriteMaster;

static partial class Hashing {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong HashXX3(this byte[] data) => XXHash3.Hash64(data);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong HashXX3(this ReadOnlySequence<byte> data) {
		// HasherXX.ComputeHash(new SequenceReader<T>(data)).Hash.HashXXCompute();
		ulong currentHash = Default;
		foreach (var seq in data) {
			currentHash = Combine(currentHash, seq.Span.HashXX3());
		}
		return currentHash;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong HashXX3(this Span2D<byte> data) {
		// HasherXX.ComputeHash(new SequenceReader<T>(data)).Hash.HashXXCompute();
		ulong currentHash = Default;
		for(int i = 0; i < data.Height; ++i) {
			currentHash = Combine(currentHash, data.GetRowSpan(i).HashXX3());
		}
		return currentHash;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong HashXX3(this ReadOnlySpan<byte> data) => XXHash3.Hash64(data);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong HashXX3(this Span<byte> data) => XXHash3.Hash64(data);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe ulong HashXX3(byte* data, int length) => XXHash3.Hash64(new ReadOnlySpan<byte>(data, length));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong HashXX3(this byte[] data, int start, int length) => XXHash3.Hash64(new ReadOnlySpan<byte>(data, start, length));

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong HashXX3(this Stream stream) {
		using var mStream = new MemoryStream();
		stream.CopyTo(mStream);
		return XXHash3.Hash64(mStream.ToArray());
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong HashXX3(this MemoryStream stream) => XXHash3.Hash64(stream.ToArray());

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong HashXX3(this UnmanagedMemoryStream stream) => stream.ToReadOnlySpan<byte>().HashXX3();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong HashXX3<T>(this T[] data) where T : unmanaged => new Span<T>(data).Cast<T, byte>().HashXX3();
}
