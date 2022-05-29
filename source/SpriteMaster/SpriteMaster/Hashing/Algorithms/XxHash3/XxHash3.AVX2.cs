/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SpriteMaster.Hashing.Algorithms;

internal static unsafe partial class XxHash3 {
	private static class Avx2Implementation {
		[FixedAddressValueType]
		internal static readonly Vector256<uint> PrimeVector = Vector256.Create(Prime32.Prime0);
	}

	// xxh3_accumulate_512_avx2
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Accumulate512Avx2(ref Accumulator accumulator, byte* data, byte* secret) {
		PrefetchNonTemporalNext(data);
		PrefetchNonTemporalNext(secret);

		if (UnrollCount >= 2u) {
			var dataVec0 = Avx2.LoadVector256(data + 0x00u).AsUInt64();
			var dataVec1 = Avx2.LoadVector256(data + 0x20u).AsUInt64();
			var keyVec0 = Avx2.LoadVector256(secret + 0x00u).AsUInt64();
			var keyVec1 = Avx2.LoadVector256(secret + 0x20u).AsUInt64();

			var dataKey0 = Avx2.Xor(dataVec0, keyVec0);
			var dataKey1 = Avx2.Xor(dataVec1, keyVec1);
			var dataKeyLo0 = Avx2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
			var dataKeyLo1 = Avx2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
			var product0 = Avx2.Multiply(dataKey0.AsUInt32(), dataKeyLo0);
			var product1 = Avx2.Multiply(dataKey1.AsUInt32(), dataKeyLo1);

			var dataSwap0 = Avx2.Shuffle(dataVec0.AsUInt32(), ShuffleDataSwap);
			var dataSwap1 = Avx2.Shuffle(dataVec1.AsUInt32(), ShuffleDataSwap);
			var addend0 = accumulator.Data256.Data0;
			var addend1 = accumulator.Data256.Data1;
			var sum0 = Avx2.Add(addend0, dataSwap0.AsUInt64());
			var sum1 = Avx2.Add(addend1, dataSwap1.AsUInt64());

			var result0 = Avx2.Add(product0, sum0);
			var result1 = Avx2.Add(product1, sum1);

			accumulator.Data256.Data0 = result0;
			accumulator.Data256.Data1 = result1;
		}
		else {
			for (uint i = 0u; i < StripeLength; i += 0x20u) {
				var dataVec = Avx2.LoadVector256(data + i).AsUInt64();
				var keyVec = Avx2.LoadVector256(secret + i).AsUInt64();

				var dataKey = Avx2.Xor(dataVec, keyVec);
				var dataKeyLo = Avx2.Shuffle(dataKey.AsUInt32(), ShuffleDataKey);
				var product = Avx2.Multiply(dataKey.AsUInt32(), dataKeyLo);

				var dataSwap = Avx2.Shuffle(dataVec.AsUInt32(), ShuffleDataSwap);
				var addend = accumulator.Data256.AtOffset(i);
				var sum = Avx2.Add(addend, dataSwap.AsUInt64());

				var result = Avx2.Add(product, sum);

				accumulator.Data256.AtOffset(i) = result;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Accumulate1024Avx2(ref Accumulator accumulator, byte* data, byte* secret) {
		PrefetchNonTemporalNext(data);
		PrefetchNonTemporalNext(data + 0x40);

		var dataVec0 = Avx2.LoadVector256(data + 0x00u).AsUInt64();
		var dataVec1 = Avx2.LoadVector256(data + 0x20u).AsUInt64();
		var keyVec0 = Avx2.LoadVector256(secret + 0x00u).AsUInt64();
		var keyVec1 = Avx2.LoadVector256(secret + 0x20u).AsUInt64();

		var dataKey0 = Avx2.Xor(dataVec0, keyVec0);
		var dataKey1 = Avx2.Xor(dataVec1, keyVec1);
		var dataKeyLo0 = Avx2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
		var dataKeyLo1 = Avx2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
		var product0 = Avx2.Multiply(dataKey0.AsUInt32(), dataKeyLo0);
		var product1 = Avx2.Multiply(dataKey1.AsUInt32(), dataKeyLo1);

		var dataSwap0 = Avx2.Shuffle(dataVec0.AsUInt32(), ShuffleDataSwap);
		var dataSwap1 = Avx2.Shuffle(dataVec1.AsUInt32(), ShuffleDataSwap);
		var addend0 = accumulator.Data256.Data0;
		var addend1 = accumulator.Data256.Data1;
		var sum0 = Avx2.Add(addend0, dataSwap0.AsUInt64());
		var sum1 = Avx2.Add(addend1, dataSwap1.AsUInt64());

		var result0 = Avx2.Add(product0, sum0);
		var result1 = Avx2.Add(product1, sum1);

		addend0 = result0;
		addend1 = result1;

		var dataVec2 = Avx2.LoadVector256(data + 0x40u).AsUInt64();
		var dataVec3 = Avx2.LoadVector256(data + 0x60u).AsUInt64();
		var keyVec2 = Avx2.LoadVector256(secret + 0x08u).AsUInt64();
		var keyVec3 = Avx2.LoadVector256(secret + 0x28u).AsUInt64();

		var dataKey2 = Avx2.Xor(dataVec2, keyVec2);
		var dataKey3 = Avx2.Xor(dataVec3, keyVec3);
		var dataKeyLo2 = Avx2.Shuffle(dataKey2.AsUInt32(), ShuffleDataKey);
		var dataKeyLo3 = Avx2.Shuffle(dataKey3.AsUInt32(), ShuffleDataKey);
		var product2 = Avx2.Multiply(dataKey2.AsUInt32(), dataKeyLo2);
		var product3 = Avx2.Multiply(dataKey3.AsUInt32(), dataKeyLo3);

		var dataSwap2 = Avx2.Shuffle(dataVec2.AsUInt32(), ShuffleDataSwap);
		var dataSwap3 = Avx2.Shuffle(dataVec3.AsUInt32(), ShuffleDataSwap);
		var sum2 = Avx2.Add(addend0, dataSwap2.AsUInt64());
		var sum3 = Avx2.Add(addend1, dataSwap3.AsUInt64());

		var result2 = Avx2.Add(product2, sum2);
		var result3 = Avx2.Add(product3, sum3);

		accumulator.Data256.Data0 = result2;
		accumulator.Data256.Data1 = result3;
	}

	// xxh3_scramble_acc_avx2
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ScrambleAccumulatorAvx2(ref Accumulator accumulator, byte* secret) {
		if (UnrollCount >= 2u) {
			var accumulatorVec0 = accumulator.Data256.Data0;
			var accumulatorVec1 = accumulator.Data256.Data1;
			var shifted0 = Avx2.ShiftRightLogical(accumulatorVec0, 47);
			var shifted1 = Avx2.ShiftRightLogical(accumulatorVec1, 47);
			var dataVec0 = Avx2.Xor(accumulatorVec0, shifted0);
			var dataVec1 = Avx2.Xor(accumulatorVec1, shifted1);

			var keyVec0 = Avx2.LoadVector256(secret + 0x00u).AsUInt64();
			var keyVec1 = Avx2.LoadVector256(secret + 0x20u).AsUInt64();
			var dataKey0 = Avx2.Xor(dataVec0, keyVec0.AsUInt64());
			var dataKey1 = Avx2.Xor(dataVec1, keyVec1.AsUInt64());

			var dataKeyHi0 = Avx2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
			var dataKeyHi1 = Avx2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
			var productLo0 = Avx2.Multiply(dataKey0.AsUInt32(), Avx2Implementation.PrimeVector);
			var productLo1 = Avx2.Multiply(dataKey1.AsUInt32(), Avx2Implementation.PrimeVector);
			var productHi0 = Avx2.Multiply(dataKeyHi0.AsUInt32(), Avx2Implementation.PrimeVector);
			var productHi1 = Avx2.Multiply(dataKeyHi1.AsUInt32(), Avx2Implementation.PrimeVector);

			productHi0 = Avx2.ShiftLeftLogical(productHi0, 32);
			productHi1 = Avx2.ShiftLeftLogical(productHi1, 32);

			var sum0 = Avx2.Add(productLo0, productHi0);
			var sum1 = Avx2.Add(productLo1, productHi1);

			accumulator.Data256.Data0 = sum0;
			accumulator.Data256.Data1 = sum1;
		}
		else {
			for (uint i = 0u; i < StripeLength; i += 0x20u) {
				var accumulatorVec = accumulator.Data256.AtOffset(i);
				var shifted = Avx2.ShiftRightLogical(accumulatorVec, 47);
				var dataVec = Avx2.Xor(accumulatorVec, shifted);

				var keyVec = Avx2.LoadVector256(secret + i).AsUInt64();
				var dataKey = Avx2.Xor(dataVec, keyVec.AsUInt64());

				var dataKeyHi = Avx2.Shuffle(dataKey.AsUInt32(), ShuffleDataKey);
				var productLo = Avx2.Multiply(dataKey.AsUInt32(), Avx2Implementation.PrimeVector);
				var productHi = Avx2.Multiply(dataKeyHi.AsUInt32(), Avx2Implementation.PrimeVector);

				productHi = Avx2.ShiftLeftLogical(productHi, 32);

				var sum = Avx2.Add(productLo, productHi);

				accumulator.Data256.AtOffset(i) = sum;
			}
		}
	}
}
