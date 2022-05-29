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
	private static class Sse2Implementation {
		[FixedAddressValueType]
		internal static readonly Vector128<uint> PrimeVector = Vector128.Create(Prime32.Prime0);
	}

	// xxh3_accumulate_512_sse2
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Accumulate512Sse2(ref Accumulator accumulator, byte* data, byte* secret) {
		PrefetchNonTemporalNext(data);
		PrefetchNonTemporalNext(secret);

		if (UnrollCount > 2u) {
			var dataVec0 = Sse2.LoadVector128(data + 0x00u).AsUInt64();
			var dataVec1 = Sse2.LoadVector128(data + 0x10u).AsUInt64();
			var dataVec2 = Sse2.LoadVector128(data + 0x20u).AsUInt64();
			var dataVec3 = Sse2.LoadVector128(data + 0x30u).AsUInt64();
			var keyVec0 = Sse2.LoadVector128(secret + 0x00u).AsUInt64();
			var keyVec1 = Sse2.LoadVector128(secret + 0x10u).AsUInt64();
			var keyVec2 = Sse2.LoadVector128(secret + 0x20u).AsUInt64();
			var keyVec3 = Sse2.LoadVector128(secret + 0x30u).AsUInt64();

			var dataKey0 = Sse2.Xor(dataVec0, keyVec0);
			var dataKey1 = Sse2.Xor(dataVec1, keyVec1);
			var dataKey2 = Sse2.Xor(dataVec2, keyVec2);
			var dataKey3 = Sse2.Xor(dataVec3, keyVec3);
			var dataKeyLo0 = Sse2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
			var dataKeyLo1 = Sse2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
			var dataKeyLo2 = Sse2.Shuffle(dataKey2.AsUInt32(), ShuffleDataKey);
			var dataKeyLo3 = Sse2.Shuffle(dataKey3.AsUInt32(), ShuffleDataKey);
			var product0 = Sse2.Multiply(dataKey0.AsUInt32(), dataKeyLo0);
			var product1 = Sse2.Multiply(dataKey1.AsUInt32(), dataKeyLo1);
			var product2 = Sse2.Multiply(dataKey2.AsUInt32(), dataKeyLo2);
			var product3 = Sse2.Multiply(dataKey3.AsUInt32(), dataKeyLo3);

			var dataSwap0 = Sse2.Shuffle(dataVec0.AsUInt32(), ShuffleDataSwap);
			var dataSwap1 = Sse2.Shuffle(dataVec1.AsUInt32(), ShuffleDataSwap);
			var dataSwap2 = Sse2.Shuffle(dataVec2.AsUInt32(), ShuffleDataSwap);
			var dataSwap3 = Sse2.Shuffle(dataVec3.AsUInt32(), ShuffleDataSwap);
			var addend0 = accumulator.Data128.Data0;
			var addend1 = accumulator.Data128.Data1;
			var addend2 = accumulator.Data128.Data2;
			var addend3 = accumulator.Data128.Data3;

			var sum0 = Sse2.Add(addend0, dataSwap0.AsUInt64());
			var sum1 = Sse2.Add(addend1, dataSwap1.AsUInt64());
			var sum2 = Sse2.Add(addend2, dataSwap2.AsUInt64());
			var sum3 = Sse2.Add(addend3, dataSwap3.AsUInt64());

			var result0 = Sse2.Add(product0, sum0);
			var result1 = Sse2.Add(product1, sum1);
			var result2 = Sse2.Add(product2, sum2);
			var result3 = Sse2.Add(product3, sum3);

			accumulator.Data128.Data0 = result0;
			accumulator.Data128.Data1 = result1;
			accumulator.Data128.Data2 = result2;
			accumulator.Data128.Data3 = result3;
		}
		else if (UnrollCount == 2u) {
			for (uint i = 0u; i < StripeLength; i += 0x20u) {
				var dataVec0 = Sse2.LoadVector128(data + i + 0x00u).AsUInt64();
				var dataVec1 = Sse2.LoadVector128(data + i + 0x10u).AsUInt64();
				var keyVec0 = Sse2.LoadVector128(secret + i + 0x00u).AsUInt64();
				var keyVec1 = Sse2.LoadVector128(secret + i + 0x10u).AsUInt64();

				var dataKey0 = Sse2.Xor(dataVec0, keyVec0);
				var dataKey1 = Sse2.Xor(dataVec1, keyVec1);
				var dataKeyLo0 = Sse2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
				var dataKeyLo1 = Sse2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
				var product0 = Sse2.Multiply(dataKey0.AsUInt32(), dataKeyLo0);
				var product1 = Sse2.Multiply(dataKey1.AsUInt32(), dataKeyLo1);

				var dataSwap0 = Sse2.Shuffle(dataVec0.AsUInt32(), ShuffleDataSwap);
				var dataSwap1 = Sse2.Shuffle(dataVec1.AsUInt32(), ShuffleDataSwap);
				var addend0 = accumulator.Data128.AtOffset(i + 0x00u);
				var addend1 = accumulator.Data128.AtOffset(i + 0x10u);

				var sum0 = Sse2.Add(addend0, dataSwap0.AsUInt64());
				var sum1 = Sse2.Add(addend1, dataSwap1.AsUInt64());

				var result0 = Sse2.Add(product0, sum0);
				var result1 = Sse2.Add(product1, sum1);

				accumulator.Data128.AtOffset(i + 0x00u) = result0;
				accumulator.Data128.AtOffset(i + 0x10u) = result1;
			}
		}
		else {
			for (uint i = 0u; i < StripeLength; i += 0x10u) {
				var dataVec = Sse2.LoadVector128(data + i).AsUInt64();
				var keyVec = Sse2.LoadVector128(secret + i).AsUInt64();

				var dataKey = Sse2.Xor(dataVec, keyVec);
				var dataKeyLo = Sse2.Shuffle(dataKey.AsUInt32(), ShuffleDataKey);
				var product = Sse2.Multiply(dataKey.AsUInt32(), dataKeyLo);

				var dataSwap = Sse2.Shuffle(dataVec.AsUInt32(), ShuffleDataSwap);
				var addend = accumulator.Data128.AtOffset(i);
				var sum = Sse2.Add(addend, dataSwap.AsUInt64());

				var result = Sse2.Add(product, sum);

				accumulator.Data128.AtOffset(i) = result;
			}
		}
	}

	// xxh3_scramble_acc_sse2
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ScrambleAccumulatorSse2(ref Accumulator accumulator, byte* secret) {
		PrefetchNonTemporalNext(secret);

		if (UnrollCount > 2u) {
			var accumulatorVec0 = accumulator.Data128.Data0;
			var accumulatorVec1 = accumulator.Data128.Data1;
			var accumulatorVec2 = accumulator.Data128.Data2;
			var accumulatorVec3 = accumulator.Data128.Data3;
			var shifted0 = Sse2.ShiftRightLogical(accumulatorVec0, 47);
			var shifted1 = Sse2.ShiftRightLogical(accumulatorVec1, 47);
			var shifted2 = Sse2.ShiftRightLogical(accumulatorVec2, 47);
			var shifted3 = Sse2.ShiftRightLogical(accumulatorVec3, 47);
			var dataVec0 = Sse2.Xor(accumulatorVec0, shifted0);
			var dataVec1 = Sse2.Xor(accumulatorVec1, shifted1);
			var dataVec2 = Sse2.Xor(accumulatorVec2, shifted2);
			var dataVec3 = Sse2.Xor(accumulatorVec3, shifted3);

			var keyVec0 = Sse2.LoadVector128(secret + 0x00u).AsUInt64();
			var keyVec1 = Sse2.LoadVector128(secret + 0x10u).AsUInt64();
			var keyVec2 = Sse2.LoadVector128(secret + 0x20u).AsUInt64();
			var keyVec3 = Sse2.LoadVector128(secret + 0x30u).AsUInt64();
			var dataKey0 = Sse2.Xor(dataVec0, keyVec0.AsUInt64());
			var dataKey1 = Sse2.Xor(dataVec1, keyVec1.AsUInt64());
			var dataKey2 = Sse2.Xor(dataVec2, keyVec2.AsUInt64());
			var dataKey3 = Sse2.Xor(dataVec3, keyVec3.AsUInt64());

			var dataKeyHi0 = Sse2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
			var dataKeyHi1 = Sse2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
			var dataKeyHi2 = Sse2.Shuffle(dataKey2.AsUInt32(), ShuffleDataKey);
			var dataKeyHi3 = Sse2.Shuffle(dataKey3.AsUInt32(), ShuffleDataKey);
			var productLo0 = Sse2.Multiply(dataKey0.AsUInt32(), Sse2Implementation.PrimeVector);
			var productLo1 = Sse2.Multiply(dataKey1.AsUInt32(), Sse2Implementation.PrimeVector);
			var productLo2 = Sse2.Multiply(dataKey2.AsUInt32(), Sse2Implementation.PrimeVector);
			var productLo3 = Sse2.Multiply(dataKey3.AsUInt32(), Sse2Implementation.PrimeVector);
			var productHi0 = Sse2.Multiply(dataKeyHi0.AsUInt32(), Sse2Implementation.PrimeVector);
			var productHi1 = Sse2.Multiply(dataKeyHi1.AsUInt32(), Sse2Implementation.PrimeVector);
			var productHi2 = Sse2.Multiply(dataKeyHi2.AsUInt32(), Sse2Implementation.PrimeVector);
			var productHi3 = Sse2.Multiply(dataKeyHi3.AsUInt32(), Sse2Implementation.PrimeVector);

			productHi0 = Sse2.ShiftLeftLogical(productHi0, 32);
			productHi1 = Sse2.ShiftLeftLogical(productHi1, 32);
			productHi2 = Sse2.ShiftLeftLogical(productHi2, 32);
			productHi3 = Sse2.ShiftLeftLogical(productHi3, 32);

			var sum0 = Sse2.Add(productLo0, productHi0);
			var sum1 = Sse2.Add(productLo1, productHi1);
			var sum2 = Sse2.Add(productLo2, productHi2);
			var sum3 = Sse2.Add(productLo3, productHi3);

			accumulator.Data128.Data0 = sum0;
			accumulator.Data128.Data1 = sum1;
			accumulator.Data128.Data2 = sum2;
			accumulator.Data128.Data3 = sum3;
		}
		else if (UnrollCount == 2u) {
			for (uint i = 0u; i < StripeLength; i += 0x20u) {
				var accumulatorVec0 = accumulator.Data128.AtOffset(i + 0x00u);
				var accumulatorVec1 = accumulator.Data128.AtOffset(i + 0x10u);
				var shifted0 = Sse2.ShiftRightLogical(accumulatorVec0, 47);
				var shifted1 = Sse2.ShiftRightLogical(accumulatorVec1, 47);
				var dataVec0 = Sse2.Xor(accumulatorVec0, shifted0);
				var dataVec1 = Sse2.Xor(accumulatorVec1, shifted1);

				var keyVec0 = Sse2.LoadVector128(secret + i + 0x00u).AsUInt64();
				var keyVec1 = Sse2.LoadVector128(secret + i + 0x10u).AsUInt64();
				var dataKey0 = Sse2.Xor(dataVec0, keyVec0.AsUInt64());
				var dataKey1 = Sse2.Xor(dataVec1, keyVec1.AsUInt64());

				var dataKeyHi0 = Sse2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
				var dataKeyHi1 = Sse2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
				var productLo0 = Sse2.Multiply(dataKey0.AsUInt32(), Sse2Implementation.PrimeVector);
				var productLo1 = Sse2.Multiply(dataKey1.AsUInt32(), Sse2Implementation.PrimeVector);
				var productHi0 = Sse2.Multiply(dataKeyHi0.AsUInt32(), Sse2Implementation.PrimeVector);
				var productHi1 = Sse2.Multiply(dataKeyHi1.AsUInt32(), Sse2Implementation.PrimeVector);

				productHi0 = Sse2.ShiftLeftLogical(productHi0, 32);
				productHi1 = Sse2.ShiftLeftLogical(productHi1, 32);

				var sum0 = Sse2.Add(productLo0, productHi0);
				var sum1 = Sse2.Add(productLo1, productHi1);

				accumulator.Data128.AtOffset(i + 0x00u) = sum0;
				accumulator.Data128.AtOffset(i + 0x10u) = sum1;
			}
		}
		else {
			for (uint i = 0u; i < StripeLength; i += 0x10u) {
				var accumulatorVec = accumulator.Data128.AtOffset(i);
				var shifted = Sse2.ShiftRightLogical(accumulatorVec, 47);
				var dataVec = Sse2.Xor(accumulatorVec, shifted);

				var keyVec = Sse2.LoadVector128(secret + i).AsUInt64();
				var dataKey = Sse2.Xor(dataVec, keyVec.AsUInt64());

				var dataKeyHi = Sse2.Shuffle(dataKey.AsUInt32(), ShuffleDataKey);
				var productLo = Sse2.Multiply(dataKey.AsUInt32(), Sse2Implementation.PrimeVector);
				var productHi = Sse2.Multiply(dataKeyHi.AsUInt32(), Sse2Implementation.PrimeVector);

				productHi = Sse2.ShiftLeftLogical(productHi, 32);

				var sum = Sse2.Add(productLo, productHi);

				accumulator.Data128.AtOffset(i) = sum;
			}
		}
	}
}
