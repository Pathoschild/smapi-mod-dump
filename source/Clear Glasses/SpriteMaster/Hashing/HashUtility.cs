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
using MonoGame.Framework.Utilities;
using SpriteMaster.Extensions;
using SpriteMaster.Types;

using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpriteMaster.Hashing;

internal static partial class HashUtility {
	internal static class Constants {
		internal static class Bits64 {
			internal const ulong GoldenRatio = 0x9e3779b97f4a7c15UL; // ⌊2^64 / Φ⌋

			internal const ulong Default = GoldenRatio;
			internal const ulong Null = ~Default;
		}

		internal static class Bits32 {
			internal const uint GoldenRatioU = 0x9e3779b9U; // ⌊2^64 / Φ⌋
			internal const int GoldenRatio = unchecked((int)GoldenRatioU); // ⌊2^64 / Φ⌋

			internal const int Default = GoldenRatio;
			internal const int Null = ~Default;
		}
	}

	private static class SplitMix {
		// https://stackoverflow.com/a/12996028
		// https://github.com/bryc/code/blob/516c6942da7a48963d81bda5428d13380ef8da5a/jshash/PRNGs.md#SplitMix32
		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal static ulong Common(ulong value) {
			value += Constants.Bits64.GoldenRatio;
			value = (value ^ value >> 30) * 0xbf58476d1ce4e5b9UL; // Closest prime is 0xbf58476d1ce4e57d
			value = (value ^ value >> 27) * 0x94d049bb133111ebUL; // Closest prime is 0x94d049bb133111e7
			value ^= value >> 31;
			return value;
		}

		// https://github.com/bryc/code/blob/516c6942da7a48963d81bda5428d13380ef8da5a/jshash/PRNGs.md#SplitMix32
		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal static int Common(int value) {
			uint uValue = (uint)value;

			uValue += Constants.Bits32.GoldenRatioU;
			uValue = (uValue ^ uValue >> 15) * 0x85ebca6bU; // Closest prime is 0x85ebca77
			uValue = (uValue ^ uValue >> 13) * 0xc2b2ae35U; // Closest prime is 0xc2b2ae3d
			uValue ^= uValue >> 16;
			return (int)uValue;
		}
	}

	// https://stackoverflow.com/a/12996028
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Rehash(ulong value) =>
		SplitMix.Common(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Rehash(int value) =>
		SplitMix.Common(value);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Accumulate(ulong hash, ulong hashend) =>
		hash ^ hashend + Constants.Bits64.Default + (hash << 6) + (hash >> 2);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Accumulate(int hash, int hashend) =>
		hash ^ (int)((uint)hashend + Constants.Bits32.Default) + (hash << 6) + (int)((uint)hash >> 2);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Accumulate(ulong hash, int hashend) =>
		Accumulate(hash, (ulong)hashend);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong SubHash64(object? subHash) => subHash switch {
		int i => (ulong)i,
		uint i => i,
		long i => (ulong)i,
		ulong i => i,
		string s => s.GetSafeHash64(),
		StringBuilder s => s.GetSafeHash64(),
		null => Constants.Bits64.Null,
		ILongHash lh => lh.GetLongHashCode(),
		_ => (ulong)subHash.GetHashCode()
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int SubHash32(object? subHash) => subHash switch {
		int i => i,
		uint i => (int)i,
		long i => i.GetHashCode(),
		ulong i => i.GetHashCode(),
		string s => s.GetSafeHash(),
		StringBuilder s => s.GetSafeHash(),
		null => Constants.Bits32.Null,
		_ => subHash.GetHashCode()
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Combine(params ulong[] hashes) {
		if (hashes.Length == 0) {
			return Constants.Bits64.Default;
		}

		ulong hash = hashes[0];
		for (int i = 1; i < hashes.Length; ++i) {
			hash = Accumulate(hash, hashes[i]);
		}
		return hash;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Combine(ulong? hash0, params ulong[] hashes) {
		if (hashes.Length == 0) {
			return Constants.Bits64.Default;
		}

		ulong hash = hash0.HasValue ? SubHash64(hash0.Value) : Constants.Bits64.Null;
		for (int i = 0; i < hashes.Length; ++i) {
			hash = Accumulate(hash, hashes[i]);
		}
		return hash;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Combine(params int[] hashes) {
		if (hashes.Length == 0) {
			return Constants.Bits32.Default;
		}

		int hash = hashes[0];
		for (int i = 1; i < hashes.Length; ++i) {
			hash = Accumulate(hash, hashes[i]);
		}
		return hash;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Combine(int? hash0, params int[] hashes) {
		if (hashes.Length == 0) {
			return Constants.Bits32.Default;
		}

		int hash = hash0.HasValue ? SubHash32(hash0.Value) : Constants.Bits32.Null;
		for (int i = 0; i < hashes.Length; ++i) {
			hash = Accumulate(hash, hashes[i]);
		}
		return hash;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Combine(params object?[] hashes) {
		if (hashes.Length == 0) {
			return Constants.Bits64.Default;
		}

		ulong hash = SubHash64(hashes[0]);

		for (int index = 1; index < hashes.Length; ++index) {
			hash = Accumulate(hash, SubHash64(hashes[index]));
		}
		return hash;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Combine(ulong hash0, ulong hash1) =>
		Accumulate(hash0, hash1);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Combine(ulong hash0, ulong hash1, ulong hash2) =>
		Accumulate(Accumulate(hash0, hash1), hash2);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Combine(string hash0, string hash1, int hash2) =>
		Accumulate(Accumulate(SubHash64(hash0), SubHash64(hash1)), hash2);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Combine(object? hashable0, object? hashable1) =>
		Combine(SubHash64(hashable0), SubHash64(hashable1));

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Combine32(params object?[] hashes) {
		if (hashes.Length == 0) {
			return Constants.Bits32.Default;
		}

		int hash = SubHash32(hashes[0]);

		for (int index = 1; index < hashes.Length; ++index) {
			hash = Accumulate(hash, SubHash32(hashes[index]));
		}
		return hash;
	}



	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Combine32(int hash0, int hash1) =>
		Accumulate(hash0, hash1);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Combine32(int hash0, int hash1, int hash2) =>
		Accumulate(Accumulate(hash0, hash1), hash2);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Combine32(string hash0, string hash1, int hash2) =>
		Accumulate(Accumulate(SubHash32(hash0), SubHash32(hash1)), hash2);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Combine32(object? hashable0, object? hashable1) =>
		Combine(SubHash32(hashable0), SubHash32(hashable1));

	#region 64-bit

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash(this byte[] data) => data.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash(this ReadOnlySequence<byte> data) => data.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash(this Span2D<byte> data) => data.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash(this ReadOnlySpan2D<byte> data) => data.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash(this Span<byte> data) => data.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash(this ReadOnlySpan<byte> data) => data.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash<T>(this Span<T> data) where T : unmanaged => data.Cast<T, byte>().Hash();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash<T>(this ReadOnlySpan<T> data) where T : unmanaged => data.Cast<T, byte>().Hash();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash(this in ReadOnlyMemory<byte> data) => data.Span.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe ulong Hash(byte* data, int length) => HashXx3(data, length);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash(this byte[] data, int start, int length) => data.HashXx3(start, length);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash<T>(this T[] data) where T : unmanaged => data.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash(this Stream stream) => stream.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash(this MemoryStream stream) => stream.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash(this UnmanagedMemoryStream stream) => stream.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash(this DrawingRectangle rectangle) =>
		((Bounds)rectangle).GetLongHashCode();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash(this XRectangle rectangle) =>
		((Bounds)rectangle).GetLongHashCode();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static ulong Hash(this Bounds rectangle) =>
		rectangle.GetLongHashCode();

	#endregion

	#region 32-bit

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32(this byte[] data) => (int)data.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32(this ReadOnlySequence<byte> data) => (int)data.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32(this Span2D<byte> data) => (int)data.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32(this Span<byte> data) => (int)data.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32(this ReadOnlySpan<byte> data) => (int)data.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32<T>(this Span<T> data) where T : unmanaged => data.Cast<T, byte>().Hash32();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32<T>(this ReadOnlySpan<T> data) where T : unmanaged => data.Cast<T, byte>().Hash32();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32(this in ReadOnlyMemory<byte> data) => (int)data.Span.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe int Hash32(byte* data, int length) => (int)HashXx3(data, length);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32(this byte[] data, int start, int length) => (int)data.HashXx3(start, length);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32<T>(this T[] data) where T : unmanaged => (int)data.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32(this Stream stream) => (int)stream.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32(this MemoryStream stream) => (int)stream.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32(this UnmanagedMemoryStream stream) => (int)stream.HashXx3();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32(this DrawingRectangle rectangle) =>
		((Bounds)rectangle).GetHashCode();

			[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32(this XRectangle rectangle) =>
		((Bounds)rectangle).GetHashCode();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Hash32(this Bounds rectangle) =>
		rectangle.GetHashCode();

	#endregion
}
