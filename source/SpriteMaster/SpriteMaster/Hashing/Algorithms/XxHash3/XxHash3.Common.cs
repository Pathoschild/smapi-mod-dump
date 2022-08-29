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
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace SpriteMaster.Hashing.Algorithms;

internal static unsafe partial class XxHash3 {
	private const MethodImplOptions Inline =
		MethodImplOptions.AggressiveInlining;
	private const MethodImplOptions Hot =
		(MethodImplOptions)0;

	private const bool UseAvx512 = Extensions.Simd.Support.Avx512;
	internal static readonly bool UseAvx2 = true && Extensions.Simd.Support.Avx2;
	internal static readonly bool UseSse2 = Extensions.Simd.Support.Enabled && Sse2.IsSupported;
	internal static readonly bool UseNeon = Extensions.Simd.Support.Enabled && AdvSimd.IsSupported;

	private static readonly int VectorSize =
		UseAvx512 ? 512 :
		UseAvx2 ? 256 :
		(UseSse2 || UseNeon) ? 128 :
		64;
	private const bool UsePrefetch = true;
	private const uint CacheLine = 0x40u;
	private const uint PrefetchDistance = CacheLine;

	[MethodImpl(Inline)]
	private static void PrefetchNext<T>(T* address) where T : unmanaged {
		if (!UsePrefetch) {
			return;
		}

		if (Sse.IsSupported) {
			Sse.Prefetch0(((byte*)address) + PrefetchDistance);
		}
	}

	[MethodImpl(Inline)]
	private static void PrefetchNonTemporalNext<T>(T* address) where T : unmanaged {
		if (!UsePrefetch) {
			return;
		}

		if (Sse.IsSupported) {
			Sse.PrefetchNonTemporal(((byte*)address) + PrefetchDistance);
		}
	}

	private static class Prime32 {
		internal const uint Prime0 = 0x9E3779B1U;
		internal const uint Prime1 = 0x85EBCA77U;
		internal const uint Prime2 = 0xC2B2AE3DU;
	}

	private static class Prime64 {
		internal const ulong Prime0 = 0x9E3779B185EBCA87UL;
		internal const ulong Prime1 = 0xC2B2AE3D27D4EB4FUL;
		internal const ulong Prime2 = 0x165667B19E3779F9UL;
		internal const ulong Prime3 = 0x85EBCA77C2B2AE63UL;
		internal const ulong Prime4 = 0x27D4EB2F165667C5UL;
	}

	private const uint StripeLength = 64;
	private const uint AccumulatorBytes = StripeLength / sizeof(ulong);
	private const uint StripesPerBlock = (SecretLength - StripeLength) / 8U;
	private const uint BlockLength = StripeLength * StripesPerBlock;

#if !SHIPPING
	static XxHash3() {
		StripeLength.AssertEqual(64u);
		AccumulatorBytes.AssertEqual(8u);
		StripesPerBlock.AssertEqual(16u);
		BlockLength.AssertEqual(1024u);
		SecretSpan.Length.AssertEqual((int)SecretLength);
	}
#endif

	// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
	private const uint SecretLength = 192;
	// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
	private static ReadOnlySpan<byte> SecretSpan => new byte[] {
		0xb8, 0xfe, 0x6c, 0x39, 0x23, 0xa4, 0x4b, 0xbe, 0x7c, 0x01, 0x81, 0x2c, 0xf7, 0x21, 0xad, 0x1c,
		0xde, 0xd4, 0x6d, 0xe9, 0x83, 0x90, 0x97, 0xdb, 0x72, 0x40, 0xa4, 0xa4, 0xb7, 0xb3, 0x67, 0x1f,
		0xcb, 0x79, 0xe6, 0x4e, 0xcc, 0xc0, 0xe5, 0x78, 0x82, 0x5a, 0xd0, 0x7d, 0xcc, 0xff, 0x72, 0x21,
		0xb8, 0x08, 0x46, 0x74, 0xf7, 0x43, 0x24, 0x8e, 0xe0, 0x35, 0x90, 0xe6, 0x81, 0x3a, 0x26, 0x4c,
		0x3c, 0x28, 0x52, 0xbb, 0x91, 0xc3, 0x00, 0xcb, 0x88, 0xd0, 0x65, 0x8b, 0x1b, 0x53, 0x2e, 0xa3,
		0x71, 0x64, 0x48, 0x97, 0xa2, 0x0d, 0xf9, 0x4e, 0x38, 0x19, 0xef, 0x46, 0xa9, 0xde, 0xac, 0xd8,
		0xa8, 0xfa, 0x76, 0x3f, 0xe3, 0x9c, 0x34, 0x3f, 0xf9, 0xdc, 0xbb, 0xc7, 0xc7, 0x0b, 0x4f, 0x1d,
		0x8a, 0x51, 0xe0, 0x4b, 0xcd, 0xb4, 0x59, 0x31, 0xc8, 0x9f, 0x7e, 0xc9, 0xd9, 0x78, 0x73, 0x64,
		0xea, 0xc5, 0xac, 0x83, 0x34, 0xd3, 0xeb, 0xc3, 0xc5, 0x81, 0xa0, 0xff, 0xfa, 0x13, 0x63, 0xeb,
		0x17, 0x0d, 0xdd, 0x51, 0xb7, 0xf0, 0xda, 0x49, 0xd3, 0x16, 0x55, 0x26, 0x29, 0xd4, 0x68, 0x9e,
		0x2b, 0x16, 0xbe, 0x58, 0x7d, 0x47, 0xa1, 0xfc, 0x8f, 0xf8, 0xb8, 0xd1, 0x7a, 0xd0, 0x31, 0xce,
		0x45, 0xcb, 0x3a, 0x8f, 0x95, 0x16, 0x04, 0x28, 0xaf, 0xd7, 0xfb, 0xca, 0xbb, 0x4b, 0x40, 0x7e,
	};
	private static readonly byte* Secret = (byte *)Unsafe.AsPointer(ref Unsafe.AsRef(SecretSpan.GetPinnableReference()));

	private static class SecretValues64 {
		internal const ulong Secret00 = 0xbe4ba423396cfeb8UL;
		internal const ulong Secret08 = 0x1cad21f72c81017cUL;
		internal const ulong Secret10 = 0xdb979083e96dd4deUL;
		internal const ulong Secret18 = 0x1f67b3b7a4a44072UL;
		internal const ulong Secret20 = 0x78e5c0cc4ee679cbUL;
		internal const ulong Secret28 = 0x2172ffcc7dd05a82UL;
		internal const ulong Secret30 = 0x8e2443f7744608b8UL;
		internal const ulong Secret38 = 0x4c263a81e69035e0UL;
		internal const ulong Secret40 = 0xcb00c391bb52283cUL;
		internal const ulong Secret48 = 0xa32e531b8b65d088UL;
		internal const ulong Secret50 = 0x4ef90da297486471UL;
		internal const ulong Secret58 = 0xd8acdea946ef1938UL;
		internal const ulong Secret60 = 0x3f349ce33f76faa8UL;
		internal const ulong Secret68 = 0x1d4f0bc7c7bbdcf9UL;
		internal const ulong Secret70 = 0x3159b4cd4be0518aUL;
		internal const ulong Secret78 = 0x647378d9c97e9fc8UL;

		internal const ulong Secret77 = 0x7378D9C97E9FC831UL;
		internal const ulong Secret7F = 0xEBD33483ACC5EA64UL;
		internal const ulong Secret0B = 0x6DD4DE1CAD21F72CUL;
		internal const ulong Secret1B = 0xE679CB1F67B3B7A4UL;
		internal const ulong Secret2B = 0x4608B82172FFCC7DUL;
		internal const ulong Secret3B = 0x52283C4C263A81E6UL;
		internal const ulong Secret13 = 0xA44072DB979083E9UL;
		internal const ulong Secret23 = 0xD05A8278E5C0CC4EUL;
		internal const ulong Secret33 = 0x9035E08E2443F774UL;
		internal const ulong Secret43 = 0x65D088CB00C391BBUL;
	}

	private static class SecretValues32 {
		internal const uint Secret00 = 0x396cfeb8U;
		internal const uint Secret04 = 0xbe4ba423U;
	}

	private const byte ShuffleDataKey = (0 << 6) | (3 << 4) | (0 << 2) | 1;
	private const byte ShuffleDataSwap = (1 << 6) | (0 << 4) | (3 << 2) | 2;

	[Pure]
	[MethodImpl(Inline)]
	private static ulong LoadLittle64<T>(T* data) where T : unmanaged =>
		BitConverter.IsLittleEndian ? *(ulong*)data : BinaryPrimitives.ReverseEndianness(*(ulong*)data);

	[Pure]
	[MethodImpl(Inline)]
	private static T Read<T>(this ref byte data) where T : unmanaged {
		return Unsafe.ReadUnaligned<T>(ref data);
	}

	[Pure]
	[MethodImpl(Inline)]
	private static T Read<T>(this ref byte data, uint offset) where T : unmanaged {
		return Unsafe.ReadUnaligned<T>(ref Unsafe.AddByteOffset(ref data, new(offset)));
	}

	[Pure]
	[MethodImpl(Inline)]
	private static ref byte Offset(this ref byte data, uint offset) {
		return ref Unsafe.AddByteOffset(ref data, new(offset));
	}

	[Pure]
	[MethodImpl(Inline)]
	private static ref byte AsRef(this ReadOnlySpan<byte> span) =>
		ref MemoryMarshal.GetReference(span);

	[Pure]
	[MethodImpl(Inline)]
	private static bool IsAligned(this uint value, uint alignment) =>
		(value & (alignment - 1U)) == 0U;

	[Pure]
	[MethodImpl(Inline)]
	private static T* AsPointerUnsafe<T>(this Span<T> span) where T : unmanaged =>
		(T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));

	[Pure]
	[MethodImpl(Inline)]
	private static T* AsPointerUnsafe<T>(this ReadOnlySpan<T> span) where T : unmanaged =>
		(T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));

	[Pure]
	[MethodImpl(Inline)]
	private static byte* SlicePointer(this ReadOnlySpan<byte> span, uint offset) =>
		span.Slice((int)offset).AsPointerUnsafe();

	[Pure]
	[MethodImpl(Inline)]
	private static byte* SlicePointer(this ReadOnlySpan<byte> span, uint offset, uint length) =>
		span.Slice((int)offset, (int)length).AsPointerUnsafe();
}
