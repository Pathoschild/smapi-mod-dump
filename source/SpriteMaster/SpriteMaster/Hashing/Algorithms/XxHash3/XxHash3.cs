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
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace SpriteMaster.Hashing.Algorithms;

// https://github.com/Crauzer/XXHash3.NET/tree/main/XXHash3.NET
internal static unsafe partial class XxHash3 {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong Hash64(string data) =>
		Hash64(data.AsSpan().AsBytes());

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong Hash64(ReadOnlySpan<byte> data) {
		uint length = (uint)data.Length;

		if (length <= 16) {
			return Hash0To16(data);
		}

		fixed (byte* dataPtr = data) {
			if (length <= 128) {
				return Hash17To128(dataPtr, (uint)length);
			}

			if (length <= 240) {
				return Hash129To240(dataPtr, (uint)length);
			}

			return HashLong(dataPtr, (uint)length);
		}
	}

	// xxh3_0to16_64
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong Hash0To16(ReadOnlySpan<byte> data) {
		uint length = (uint)data.Length;
		if (length > 8) return Hash9To16(data);
		if (length >= 4) return Hash4To8(data);
		if (length > 0) return Hash1To3(data);
		return Avalanche(SecretValues64.Secret38 ^ SecretValues64.Secret40);

		// xxh3_len_9to16_64
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ulong Hash9To16(ReadOnlySpan<byte> data) {
			const ulong bitFlip1 = SecretValues64.Secret18 ^ SecretValues64.Secret20;
			const ulong bitFlip2 = SecretValues64.Secret28 ^ SecretValues64.Secret30;
			ulong inputLow = data.Cast<byte, ulong>()[0] ^ bitFlip1;
			ulong inputHigh = data.Slice(data.Length - 8).Cast<byte, ulong>()[0] ^ bitFlip2;
			ulong accumulator =
				(ulong)data.Length +
				BinaryPrimitives.ReverseEndianness(inputLow) +
				inputHigh +
				Mul128Fold64(inputLow, inputHigh);

			return Avalanche(accumulator);
		}

		// xxh3_len_4to8_64
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ulong Hash4To8(ReadOnlySpan<byte> data) {
			uint input1 = data.Cast<byte, uint>()[0];
			uint input2 = data.Slice(data.Length - 4).Cast<byte, uint>()[0];
			const ulong bitFlip = SecretValues64.Secret08 ^ SecretValues64.Secret10;
			ulong input64 = input2 + ((ulong)input1 << 0x20);
			ulong keyed = input64 ^ bitFlip;

			return RotRotMulXorMulXor(keyed, (ulong)data.Length);
		}

		// xxh3_len_1to3_64
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ulong Hash1To3(ReadOnlySpan<byte> data) {
			byte c1 = data[0];
			byte c2 = data[data.Length >> 1];
			byte c3 = data[data.Length - 1];
			uint combined = ((uint)c1 << 0x10) | ((uint)c2 << 24) | ((uint)c3 << 0) | ((uint)data.Length << 8);
			const ulong bitFlip = SecretValues32.Secret00 ^ SecretValues32.Secret04;
			ulong keyed = combined ^ bitFlip;

			return AvalancheX64(keyed);
		}
	}

	// xxh3_17to128_64
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong Hash17To128(byte* data, uint length) {
		var accumulator = length * Prime64.Prime0;

		if (length > 0x20) {
			if (length > 0x40) {
				if (length > 0x60) {
					accumulator += Mix16(data + 0x30, SecretValues64.Secret60, SecretValues64.Secret68);
					accumulator += Mix16(data + (length - 0x40), SecretValues64.Secret70, SecretValues64.Secret78);
				}

				accumulator += Mix16(data + 0x20, SecretValues64.Secret40, SecretValues64.Secret48);
				accumulator += Mix16(data + (length - 0x30), SecretValues64.Secret50, SecretValues64.Secret58);
			}

			accumulator += Mix16(data + 0x10, SecretValues64.Secret20, SecretValues64.Secret28);
			accumulator += Mix16(data + (length - 0x20), SecretValues64.Secret30, SecretValues64.Secret38);
		}

		accumulator += Mix16(data, SecretValues64.Secret00, SecretValues64.Secret08);
		accumulator += Mix16(data + (length - 0x10), SecretValues64.Secret10, SecretValues64.Secret18);

		return Avalanche(accumulator);
	}

	// xxh3_129to240_64
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong Hash129To240(byte* data, uint length) {
		byte* secret = Secret;

		PrefetchNonTemporalNext(data);
		PrefetchNonTemporalNext(secret);

		var accumulator = length * Prime64.Prime0;

		accumulator += Mix16(data + 0x00, SecretValues64.Secret00, SecretValues64.Secret08);
		accumulator += Mix16(data + 0x10, SecretValues64.Secret10, SecretValues64.Secret18);
		accumulator += Mix16(data + 0x20, SecretValues64.Secret20, SecretValues64.Secret28);
		accumulator += Mix16(data + 0x30, SecretValues64.Secret30, SecretValues64.Secret38);
		accumulator += Mix16(data + 0x40, SecretValues64.Secret40, SecretValues64.Secret48);
		accumulator += Mix16(data + 0x50, SecretValues64.Secret50, SecretValues64.Secret58);
		accumulator += Mix16(data + 0x60, SecretValues64.Secret60, SecretValues64.Secret68);
		accumulator += Mix16(data + 0x70, SecretValues64.Secret70, SecretValues64.Secret78);

		accumulator = Avalanche(accumulator);

		uint roundCount = (length / 0x10) - 8;
		// min is 129, thus min roundCount is 8, max is 15
		// the 8 are handled above.

		var offsetData = data + 0x80;
		for (uint i = 0u; i < roundCount; ++i) {
			accumulator += Mix16(offsetData + (0x10 * i), secret + (0x10 * i) + 3);
		}

		accumulator += Mix16(data + (length - 0x10), SecretValues64.Secret77, SecretValues64.Secret7F);

		return Avalanche(accumulator);
	}

	[StructLayout(LayoutKind.Sequential, Pack = 0x10, Size = 0x40)]
	private struct CombinedVector128x512<T>  where T : unmanaged {
		internal Vector128<T> Data0;
		internal Vector128<T> Data1;
		internal Vector128<T> Data2;
		internal Vector128<T> Data3;

		internal Vector128<T>* AsPointer =>
			(Vector128<T>*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));

		internal ref Vector128<T> AtOffset(uint offset) =>
			ref AsPointer[offset / 0x10u];

		internal CombinedVector128x512<TOther> As<TOther>() where TOther : unmanaged =>
			*(CombinedVector128x512<TOther> *)Unsafe.AsPointer(ref Unsafe.AsRef(in Data0));
	}

	[StructLayout(LayoutKind.Sequential, Pack = 0x20, Size = 0x40)]
	private struct CombinedVector256x512<T> where T : unmanaged {
		internal Vector256<T> Data0;
		internal Vector256<T> Data1;

		internal Vector256<T>* AsPointer =>
			(Vector256<T>*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));

		internal ref Vector256<T> AtOffset(uint offset) =>
			ref AsPointer[offset / 0x20u];

		internal CombinedVector256x512<TOther> As<TOther>() where TOther : unmanaged =>
			*(CombinedVector256x512<TOther>*)Unsafe.AsPointer(ref Unsafe.AsRef(in Data0));
	}

	[StructLayout(LayoutKind.Sequential, Pack = 0x40, Size = 0x40)]
	private ref struct Accumulator {
		internal fixed ulong Data[8];

		internal ref CombinedVector128x512<ulong> Data128 => ref Unsafe.As<ulong, CombinedVector128x512<ulong>>(ref Unsafe.AsRef(in Data[0]));

		internal ref CombinedVector256x512<ulong> Data256 => ref Unsafe.As<ulong, CombinedVector256x512<ulong>>(ref Unsafe.AsRef(in Data[0]));

		internal ulong* Pointer => (ulong *)Unsafe.AsPointer(ref Unsafe.AsRef(in Data[0]));

		public Accumulator() {
			switch (VectorSize) {
				case 256: {
					Data256.Data0 = Vector256.Create(Prime32.Prime2, Prime64.Prime0, Prime64.Prime1, Prime64.Prime2);
					Data256.Data1 = Vector256.Create(Prime64.Prime3, Prime32.Prime1, Prime64.Prime4, Prime32.Prime0);
					break;
				}
				case 128: {
					Data128.Data0 = Vector128.Create(Prime32.Prime2, Prime64.Prime0);
					Data128.Data1 = Vector128.Create(Prime64.Prime1, Prime64.Prime2);
					Data128.Data2 = Vector128.Create(Prime64.Prime3, Prime32.Prime1);
					Data128.Data3 = Vector128.Create(Prime64.Prime4, Prime32.Prime0);
					break;
				}
				default: {
					Data[0] = Prime32.Prime2;
					Data[1] = Prime64.Prime0;
					Data[2] = Prime64.Prime1;
					Data[3] = Prime64.Prime2;
					Data[4] = Prime64.Prime3;
					Data[5] = Prime32.Prime1;
					Data[6] = Prime64.Prime4;
					Data[7] = Prime32.Prime0;
					break;
				}
			}
		}
	}

	// xxh3_hashLong_64
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong HashLong(byte* data, uint length) {
		var accumulator = new Accumulator();

		byte* secret = Secret;

		const uint stripesPerBlock = (SecretLength - StripeLength) / 8u;
		stripesPerBlock.AssertEqual(16u);
		const uint blockLength = StripeLength * stripesPerBlock;
		blockLength.AssertEqual(1024u);
		uint blockCount = (length - 1) / blockLength;

		// To get here, length must be greater than 240.
		// Thus, it's possible that we end up skipping these blocks altogether

		for (uint block = 0; block < blockCount; ++block) {
			PrefetchNonTemporalNext(data);
			Accumulate(ref accumulator, data + (block * blockLength), secret, stripesPerBlock);
			ScrambleAccumulator(ref accumulator, secret + (SecretLength - StripeLength));
		}

		uint stripeCount = (length - 1u - (blockLength * blockCount)) / StripeLength;
		Accumulate(ref accumulator, data + (blockCount * blockLength), secret, stripeCount);

		byte* p = data + (length - StripeLength);
		Accumulate512(ref accumulator, p, secret + (SecretLength - StripeLength - 7u));

		return MergeAccumulators(ref accumulator, length * Prime64.Prime0);
	}

	// xxh3_merge_accs
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong MergeAccumulators(ref Accumulator accumulator, ulong start) {
		ulong result = start;

		result += MixAccumulators(accumulator.Pointer + 0x0u, SecretValues64.Secret0B, SecretValues64.Secret13);
		result += MixAccumulators(accumulator.Pointer + 0x2u, SecretValues64.Secret1B, SecretValues64.Secret23);
		result += MixAccumulators(accumulator.Pointer + 0x4u, SecretValues64.Secret2B, SecretValues64.Secret33);
		result += MixAccumulators(accumulator.Pointer + 0x6u, SecretValues64.Secret3B, SecretValues64.Secret43);

		return Avalanche(result);
	}

	// xxh3_mix2accs
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong MixAccumulators(ulong* accumulator, ulong secretLo, ulong secretHi) {
		return Mul128Fold64(
			accumulator[0] ^ secretLo,
			accumulator[1] ^ secretHi
		);
	}

	// xxh3_avalanche
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong Avalanche(ulong hash) {
		hash ^= hash >> 37;
		hash *= 0x165667919E3779F9UL;
		hash ^= hash >> 0x20;

		return hash;
	}

	// xxh64_avalanche
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong AvalancheX64(ulong hash) {
		hash ^= hash >> 33;
		hash *= Prime64.Prime1;
		hash ^= hash >> 29;
		hash *= Prime64.Prime2;
		hash ^= hash >> 0x20;

		return hash;
	}

	// xxh3_rrmxmx
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong RotRotMulXorMulXor(ulong h64, ulong len) {
		h64 ^= BitOperations.RotateLeft(h64, 49) ^ BitOperations.RotateLeft(h64, 24);
		h64 *= 0x9FB2_1C65_1E98_DF25UL;
		h64 ^= (h64 >> 35) + len;
		h64 *= 0x9FB2_1C65_1E98_DF25UL;

		return h64 ^ (h64 >> 28);
	}

	// xxh3_mix16B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong Mix16(byte* data, byte* secret) {
		ulong inputLow = LoadLittle64(data);
		ulong inputHigh = LoadLittle64(data + 8);

		return Mul128Fold64(
			inputLow ^ LoadLittle64(secret),
			inputHigh ^ LoadLittle64(secret + 8)
		);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong Mix16(byte* data, ulong secretLo, ulong secretHi) {
		ulong inputLow = LoadLittle64(data);
		ulong inputHigh = LoadLittle64(data + 8);

		return Mul128Fold64(
			inputLow ^ secretLo,
			inputHigh ^ secretHi
		);
	}

	// xxh3_accumulate
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Accumulate(ref Accumulator accumulator, byte* data, byte* secret, uint stripeCount) {
		uint i = 0;
		if (Avx2.IsSupported && UseAVX2 && UnrollCount > 2u) {
			for (; i + 1u < stripeCount; i += 2u) {
				Accumulate1024Avx2(ref accumulator, data + (i * StripeLength), secret + (i * 8u));
			}
		}
		for (; i < stripeCount; ++i) {
			Accumulate512(ref accumulator, data + (i * StripeLength), secret + (i * 8u));
		}
	}

	// xxh3_accumulate_512
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Accumulate512(ref Accumulator accumulator, byte* data, byte* secret) {
		if (Avx2.IsSupported && UseAVX2) {
			Accumulate512Avx2(ref accumulator, data, secret);
		}
		else if (Sse2.IsSupported && UseSSE2) {
			Accumulate512Sse2(ref accumulator, data, secret);
		}
		else if (AdvSimd.IsSupported) {
			Accumulate512Neon(ref accumulator, data, secret);
		}
		else {
			Accumulate512Scalar(ref accumulator, data, secret);
		}
	}

	// xxh3_accumulate_512_scalar
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Accumulate512Scalar(ref Accumulator accumulator, byte* data, byte* secret) {
		PrefetchNonTemporalNext(data);
		PrefetchNonTemporalNext(secret);
		PrefetchNext(accumulator.Pointer);

		AccumulatorBytes.AssertEqual(8u);

		Accumulate512ScalarPass(ref accumulator.Data[0 ^ 1], ref accumulator.Data[0], data + 0x00, secret + 0x00);
		Accumulate512ScalarPass(ref accumulator.Data[1 ^ 1], ref accumulator.Data[1], data + 0x08, secret + 0x08);
		Accumulate512ScalarPass(ref accumulator.Data[2 ^ 1], ref accumulator.Data[2], data + 0x10, secret + 0x10);
		Accumulate512ScalarPass(ref accumulator.Data[3 ^ 1], ref accumulator.Data[3], data + 0x18, secret + 0x18);
		Accumulate512ScalarPass(ref accumulator.Data[4 ^ 1], ref accumulator.Data[4], data + 0x20, secret + 0x20);
		Accumulate512ScalarPass(ref accumulator.Data[5 ^ 1], ref accumulator.Data[5], data + 0x28, secret + 0x28);
		Accumulate512ScalarPass(ref accumulator.Data[6 ^ 1], ref accumulator.Data[6], data + 0x30, secret + 0x30);
		Accumulate512ScalarPass(ref accumulator.Data[7 ^ 1], ref accumulator.Data[7], data + 0x38, secret + 0x38);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Accumulate512ScalarPass(ref ulong accumulator0, ref ulong accumulator1, byte* data, byte* secret) {
		ulong dataVal = LoadLittle64(data);
		ulong dataKey = dataVal ^ LoadLittle64(secret);

		accumulator0 += dataVal;
		accumulator1 += (uint)dataKey * (ulong)(uint)(dataKey >> 0x20);
	}

	// xxh3_scramble_acc
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ScrambleAccumulator(ref Accumulator accumulator, byte* secret) {
		if (Avx2.IsSupported && UseAVX2) {
			ScrambleAccumulatorAvx2(ref accumulator, secret);
		}
		else if (Sse2.IsSupported && UseSSE2) {
			ScrambleAccumulatorSse2(ref accumulator, secret);
		}
		else if (AdvSimd.IsSupported) {
			ScrambleAccumulatorNeon(ref accumulator, secret);
		}
		else {
			ScrambleAccumulatorScalar(ref accumulator, secret);
		}
	}

	// xxh3_scramble_acc_scalar
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ScrambleAccumulatorScalar(ref Accumulator accumulator, byte* secret) {
		AccumulatorBytes.AssertEqual(8u);

		ScrambleAccumulatorScalarPass(ref accumulator.Data[0], secret + 0x00);
		ScrambleAccumulatorScalarPass(ref accumulator.Data[1], secret + 0x08);
		ScrambleAccumulatorScalarPass(ref accumulator.Data[2], secret + 0x10);
		ScrambleAccumulatorScalarPass(ref accumulator.Data[3], secret + 0x18);
		ScrambleAccumulatorScalarPass(ref accumulator.Data[4], secret + 0x20);
		ScrambleAccumulatorScalarPass(ref accumulator.Data[5], secret + 0x28);
		ScrambleAccumulatorScalarPass(ref accumulator.Data[6], secret + 0x30);
		ScrambleAccumulatorScalarPass(ref accumulator.Data[7], secret + 0x38);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ScrambleAccumulatorScalarPass(ref ulong accumulator, byte* secret) {
		ulong key64 = LoadLittle64(secret);
		ulong acc64 = accumulator;

		acc64 ^= acc64 >> 47;
		acc64 ^= key64;
		acc64 *= Prime32.Prime0;

		accumulator = acc64;
	}

	// xxh3_mul128_fold64
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong Mul128Fold64(ulong lhs, ulong rhs) {
		ulong low;
		ulong high = Bmi2.IsSupported ?
			Bmi2.X64.MultiplyNoFlags(lhs, rhs, &low) :
			Math.BigMul(lhs, rhs, out low);
		return low ^ high;
	}
}
