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
using SpriteMaster.Types;

using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpriteMaster;

static partial class Hashing {
	internal const ulong Default = 0x9e3779b97f4a7c15UL;
	internal const int Default32 = unchecked((int)Default);
	internal const ulong Null = ~Default;
	internal const int Null32 = unchecked((int)Null);

	// https://stackoverflow.com/a/12996028
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Rehash(ulong value) {
		if (value == 0) {
			value = 0x9e3779b97f4a7c15UL; // ⌊2^64 / Φ⌋
		}
		value = (value ^ value >> 30) * 0xbf58476d1ce4e5b9UL;
		value = (value ^ value >> 27) * 0x94d049bb133111ebUL;
		value ^= value >> 31;
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Accumulate(ulong hash, ulong hashend) => hash ^ hashend + Default + (hash << 6) + (hash >> 2);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Accumulate(int hash, int hashend) => hash ^ (int)((uint)hashend + (uint)Default32) + (hash << 6) + (int)((uint)hash >> 2);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Accumulate(ulong hash, int hashend) => Accumulate(hash, (ulong)hashend);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Combine(params ulong[] hashes) {
		ulong hash = Default;
		foreach (var subHash in hashes) {
			hash = Accumulate(hash, subHash);
		}
		return hash;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Combine(params int[] hashes) {
		int hash = Default32;
		foreach (var subHash in hashes) {
			hash = Accumulate(hash, subHash);
		}
		return hash;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Combine(params object?[] hashes) {
		ulong hash = Default;

		foreach (var subHash in hashes) {
			hash = subHash switch {
				int i => Accumulate(hash, i),
				uint i => Accumulate(hash, (int)i),
				long i => Accumulate(hash, (ulong)i),
				ulong i => Accumulate(hash, i),
				string s => Accumulate(hash, s.GetSafeHash()),
				StringBuilder s => Accumulate(hash, s.GetSafeHash()),
				null => Accumulate(hash, Null),
				_ => Accumulate(hash, subHash.GetHashCode()),
			};
		}
		return hash;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Combine32(params object?[] hashes) {
		int hash = Default32;

		foreach (var subHash in hashes) {
			hash = subHash switch {
				int i => Accumulate(hash, i),
				uint i => Accumulate(hash, (int)i),
				long i => Accumulate(hash, i.GetSafeHash()),
				ulong i => Accumulate(hash, i.GetSafeHash()),
				string s => Accumulate(hash, s.GetSafeHash()),
				StringBuilder s => Accumulate(hash, s.GetSafeHash()),
				null => Accumulate(hash, Null32),
				_ => Accumulate(hash, subHash.GetHashCode()),
			};
		}
		return hash;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Hash(this byte[] data) => data.HashXX3();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Hash(this ReadOnlySequence<byte> data) => data.HashXX3();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Hash(this Span2D<byte> data) => data.HashXX3();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Hash(this Span<byte> data) => data.HashXX3();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Hash(this ReadOnlySpan<byte> data) => data.HashXX3();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Hash(this in ReadOnlyMemory<byte> data) => data.Span.HashXX3();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static unsafe ulong Hash(byte* data, int length) => HashXX3(data, length);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Hash(this byte[] data, int start, int length) => data.HashXX3(start, length);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Hash<T>(this T[] data) where T : unmanaged => data.HashXX3();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Hash(this Stream stream) => stream.HashXX3();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Hash(this MemoryStream stream) => stream.HashXX3();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Hash(this UnmanagedMemoryStream stream) => stream.HashXX3();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Hash(this in DrawingRectangle rectangle) =>
		(ulong)rectangle.X & 0xFFFF |
		((ulong)rectangle.Y & 0xFFFF) << 16 |
		((ulong)rectangle.Width & 0xFFFF) << 32 |
		((ulong)rectangle.Height & 0xFFFF) << 48;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Hash(this in XNA.Rectangle rectangle) =>
		(ulong)rectangle.X & 0xFFFF |
		((ulong)rectangle.Y & 0xFFFF) << 16 |
		((ulong)rectangle.Width & 0xFFFF) << 32 |
		((ulong)rectangle.Height & 0xFFFF) << 48;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static ulong Hash(this in Bounds rectangle) =>
		(ulong)rectangle.X & 0xFFFF |
		((ulong)rectangle.Y & 0xFFFF) << 16 |
		((ulong)rectangle.Width & 0xFFFF) << 32 |
		((ulong)rectangle.Height & 0xFFFF) << 48;
}
