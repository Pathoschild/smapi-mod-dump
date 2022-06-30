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
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable AccessToStaticMemberViaDerivedType

namespace SpriteMaster.Hashing.Algorithms;

internal static unsafe partial class XxHash3 {
	private static partial class Sse2Impl {
		[Pure]
		[MethodImpl(Inline)]
		private static Vector128<T> LoadVector<T>(byte* data) where T : unmanaged =>
			Sse2.LoadVector128(data).As<byte, T>();

		[Pure]
		[MethodImpl(Inline)]
		private static Vector128<ulong> LoadVector(byte* data) =>
			LoadVector<ulong>(data);

		// xxh3_accumulate_512_sse2
		[MethodImpl(Inline)]
		internal static void Accumulate512(ulong* accumulatorStore, byte* data, byte* secret) {
			PrefetchNonTemporalNext(data);
			PrefetchNonTemporalNext(secret);

			ref var accumulator = ref *(CombinedVector128X512<ulong>*)accumulatorStore;

			var dataVec0 = LoadVector(data + 0x00u);
			var dataVec1 = LoadVector(data + 0x10u);
			var dataVec2 = LoadVector(data + 0x20u);
			var dataVec3 = LoadVector(data + 0x30u);
			var keyVec0 = LoadVector(secret + 0x00u);
			var keyVec1 = LoadVector(secret + 0x10u);
			var keyVec2 = LoadVector(secret + 0x20u);
			var keyVec3 = LoadVector(secret + 0x30u);

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
			var addend0 = accumulator.Data0;
			var addend1 = accumulator.Data1;
			var addend2 = accumulator.Data2;
			var addend3 = accumulator.Data3;

			var sum0 = Sse2.Add(addend0, dataSwap0.AsUInt64());
			var sum1 = Sse2.Add(addend1, dataSwap1.AsUInt64());
			var sum2 = Sse2.Add(addend2, dataSwap2.AsUInt64());
			var sum3 = Sse2.Add(addend3, dataSwap3.AsUInt64());

			var result0 = Sse2.Add(product0, sum0);
			var result1 = Sse2.Add(product1, sum1);
			var result2 = Sse2.Add(product2, sum2);
			var result3 = Sse2.Add(product3, sum3);

			accumulator.Data0 = result0;
			accumulator.Data1 = result1;
			accumulator.Data2 = result2;
			accumulator.Data3 = result3;
		}

		// xxh3_scramble_acc_sse2
		[MethodImpl(Inline)]
		internal static void ScrambleAccumulator(ulong* accumulatorStore, byte* secret) {
			PrefetchNonTemporalNext(secret);

			ref var accumulator = ref *(CombinedVector128X512<ulong>*)accumulatorStore;

			var primeVector = Vector128.Create(Prime32.Prime0);

			var accumulatorVec0 = accumulator.Data0;
			var accumulatorVec1 = accumulator.Data1;
			var accumulatorVec2 = accumulator.Data2;
			var accumulatorVec3 = accumulator.Data3;
			var shifted0 = Sse2.ShiftRightLogical(accumulatorVec0, 47);
			var shifted1 = Sse2.ShiftRightLogical(accumulatorVec1, 47);
			var shifted2 = Sse2.ShiftRightLogical(accumulatorVec2, 47);
			var shifted3 = Sse2.ShiftRightLogical(accumulatorVec3, 47);
			var dataVec0 = Sse2.Xor(accumulatorVec0, shifted0);
			var dataVec1 = Sse2.Xor(accumulatorVec1, shifted1);
			var dataVec2 = Sse2.Xor(accumulatorVec2, shifted2);
			var dataVec3 = Sse2.Xor(accumulatorVec3, shifted3);

			var keyVec0 = LoadVector(secret + 0x00u);
			var keyVec1 = LoadVector(secret + 0x10u);
			var keyVec2 = LoadVector(secret + 0x20u);
			var keyVec3 = LoadVector(secret + 0x30u);
			var dataKey0 = Sse2.Xor(dataVec0, keyVec0);
			var dataKey1 = Sse2.Xor(dataVec1, keyVec1);
			var dataKey2 = Sse2.Xor(dataVec2, keyVec2);
			var dataKey3 = Sse2.Xor(dataVec3, keyVec3);

			var dataKeyHi0 = Sse2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
			var dataKeyHi1 = Sse2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
			var dataKeyHi2 = Sse2.Shuffle(dataKey2.AsUInt32(), ShuffleDataKey);
			var dataKeyHi3 = Sse2.Shuffle(dataKey3.AsUInt32(), ShuffleDataKey);
			var productLo0 = Sse2.Multiply(dataKey0.AsUInt32(), primeVector);
			var productLo1 = Sse2.Multiply(dataKey1.AsUInt32(), primeVector);
			var productLo2 = Sse2.Multiply(dataKey2.AsUInt32(), primeVector);
			var productLo3 = Sse2.Multiply(dataKey3.AsUInt32(), primeVector);
			var productHi0 = Sse2.Multiply(dataKeyHi0.AsUInt32(), primeVector);
			var productHi1 = Sse2.Multiply(dataKeyHi1.AsUInt32(), primeVector);
			var productHi2 = Sse2.Multiply(dataKeyHi2.AsUInt32(), primeVector);
			var productHi3 = Sse2.Multiply(dataKeyHi3.AsUInt32(), primeVector);

			productHi0 = Sse2.ShiftLeftLogical(productHi0, 32);
			productHi1 = Sse2.ShiftLeftLogical(productHi1, 32);
			productHi2 = Sse2.ShiftLeftLogical(productHi2, 32);
			productHi3 = Sse2.ShiftLeftLogical(productHi3, 32);

			var sum0 = Sse2.Add(productLo0, productHi0);
			var sum1 = Sse2.Add(productLo1, productHi1);
			var sum2 = Sse2.Add(productLo2, productHi2);
			var sum3 = Sse2.Add(productLo3, productHi3);

			accumulator.Data0 = sum0;
			accumulator.Data1 = sum1;
			accumulator.Data2 = sum2;
			accumulator.Data3 = sum3;
		}

		[Pure]
		[MethodImpl(Inline)]
		internal static ulong HashLong(byte* data, uint length) {
			byte* secret = Secret;

			uint blocks = (length - 1) / BlockLength;

			Vector128<ulong> accumulator0 = Vector128.Create(Prime32.Prime2, Prime64.Prime0);
			Vector128<ulong> accumulator1 = Vector128.Create(Prime64.Prime1, Prime64.Prime2);
			Vector128<ulong> accumulator2 = Vector128.Create(Prime64.Prime3, Prime32.Prime1);
			Vector128<ulong> accumulator3 = Vector128.Create(Prime64.Prime4, Prime32.Prime0);

			var primeVector = Vector128.Create(Prime32.Prime0);

			uint block = 0;

			for (; block < blocks; ++block) {
				{
					byte* lData = data + (block * BlockLength);

					uint stripe = 0;

					for (; stripe + 1 < StripesPerBlock; stripe += 2) {
						PrefetchNonTemporalNext(lData + (stripe * StripeLength) + 0x040);
						PrefetchNonTemporalNext(lData + (stripe * StripeLength) + 0x080);
						{
							byte* llData = lData + (stripe * StripeLength);
							byte* llSecret = secret + (stripe * 8u);

							Vector128<ulong> result0, result1, result2, result3;

							{
								var dataVec0 = LoadVector(llData + 0x00u);
								var dataVec1 = LoadVector(llData + 0x10u);
								var dataVec2 = LoadVector(llData + 0x20u);
								var dataVec3 = LoadVector(llData + 0x30u);
								var keyVec0 = LoadVector(llSecret + 0x00u);
								var keyVec1 = LoadVector(llSecret + 0x10u);
								var keyVec2 = LoadVector(llSecret + 0x20u);
								var keyVec3 = LoadVector(llSecret + 0x30u);

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
								var addend0 = accumulator0;
								var addend1 = accumulator1;
								var addend2 = accumulator2;
								var addend3 = accumulator3;

								var sum0 = Sse2.Add(addend0, dataSwap0.AsUInt64());
								var sum1 = Sse2.Add(addend1, dataSwap1.AsUInt64());
								var sum2 = Sse2.Add(addend2, dataSwap2.AsUInt64());
								var sum3 = Sse2.Add(addend3, dataSwap3.AsUInt64());

								result0 = Sse2.Add(product0, sum0);
								result1 = Sse2.Add(product1, sum1);
								result2 = Sse2.Add(product2, sum2);
								result3 = Sse2.Add(product3, sum3);
							}

							{
								var dataVec0 = LoadVector(llData + 0x40u);
								var dataVec1 = LoadVector(llData + 0x50u);
								var dataVec2 = LoadVector(llData + 0x60u);
								var dataVec3 = LoadVector(llData + 0x70u);
								var keyVec0 = LoadVector(llSecret + 0x08u);
								var keyVec1 = LoadVector(llSecret + 0x18u);
								var keyVec2 = LoadVector(llSecret + 0x28u);
								var keyVec3 = LoadVector(llSecret + 0x38u);

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
								var addend0 = result0;
								var addend1 = result1;
								var addend2 = result2;
								var addend3 = result3;

								var sum0 = Sse2.Add(addend0, dataSwap0.AsUInt64());
								var sum1 = Sse2.Add(addend1, dataSwap1.AsUInt64());
								var sum2 = Sse2.Add(addend2, dataSwap2.AsUInt64());
								var sum3 = Sse2.Add(addend3, dataSwap3.AsUInt64());

								accumulator0 = Sse2.Add(product0, sum0);
								accumulator1 = Sse2.Add(product1, sum1);
								accumulator2 = Sse2.Add(product2, sum2);
								accumulator3 = Sse2.Add(product3, sum3);
							}
						}
					}

					if (stripe < StripesPerBlock) {
						PrefetchNonTemporalNext(lData + (stripe * StripeLength) + 0x040);
						{
							byte* llData = lData + (stripe * StripeLength);
							byte* llSecret = secret + (stripe * 8u);

							var dataVec0 = LoadVector(llData + 0x00u);
							var dataVec1 = LoadVector(llData + 0x10u);
							var dataVec2 = LoadVector(llData + 0x20u);
							var dataVec3 = LoadVector(llData + 0x30u);
							var keyVec0 = LoadVector(llSecret + 0x00u);
							var keyVec1 = LoadVector(llSecret + 0x10u);
							var keyVec2 = LoadVector(llSecret + 0x20u);
							var keyVec3 = LoadVector(llSecret + 0x30u);

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
							var addend0 = accumulator0;
							var addend1 = accumulator1;
							var addend2 = accumulator2;
							var addend3 = accumulator3;

							var sum0 = Sse2.Add(addend0, dataSwap0.AsUInt64());
							var sum1 = Sse2.Add(addend1, dataSwap1.AsUInt64());
							var sum2 = Sse2.Add(addend2, dataSwap2.AsUInt64());
							var sum3 = Sse2.Add(addend3, dataSwap3.AsUInt64());

							var result0 = Sse2.Add(product0, sum0);
							var result1 = Sse2.Add(product1, sum1);
							var result2 = Sse2.Add(product2, sum2);
							var result3 = Sse2.Add(product3, sum3);

							accumulator0 = result0;
							accumulator1 = result1;
							accumulator2 = result2;
							accumulator3 = result3;
						}
					}
				}

				{
					byte* lSecret = secret + (SecretLength - StripeLength);

					var accumulatorVec0 = accumulator0;
					var accumulatorVec1 = accumulator1;
					var accumulatorVec2 = accumulator2;
					var accumulatorVec3 = accumulator3;
					var shifted0 = Sse2.ShiftRightLogical(accumulatorVec0, 47);
					var shifted1 = Sse2.ShiftRightLogical(accumulatorVec1, 47);
					var shifted2 = Sse2.ShiftRightLogical(accumulatorVec2, 47);
					var shifted3 = Sse2.ShiftRightLogical(accumulatorVec3, 47);
					var dataVec0 = Sse2.Xor(accumulatorVec0, shifted0);
					var dataVec1 = Sse2.Xor(accumulatorVec1, shifted1);
					var dataVec2 = Sse2.Xor(accumulatorVec2, shifted2);
					var dataVec3 = Sse2.Xor(accumulatorVec3, shifted3);

					var keyVec0 = LoadVector(lSecret + 0x00u);
					var keyVec1 = LoadVector(lSecret + 0x10u);
					var keyVec2 = LoadVector(lSecret + 0x20u);
					var keyVec3 = LoadVector(lSecret + 0x30u);
					var dataKey0 = Sse2.Xor(dataVec0, keyVec0);
					var dataKey1 = Sse2.Xor(dataVec1, keyVec1);
					var dataKey2 = Sse2.Xor(dataVec2, keyVec2);
					var dataKey3 = Sse2.Xor(dataVec3, keyVec3);

					var dataKeyHi0 = Sse2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
					var dataKeyHi1 = Sse2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
					var dataKeyHi2 = Sse2.Shuffle(dataKey2.AsUInt32(), ShuffleDataKey);
					var dataKeyHi3 = Sse2.Shuffle(dataKey3.AsUInt32(), ShuffleDataKey);
					var productLo0 = Sse2.Multiply(dataKey0.AsUInt32(), primeVector);
					var productLo1 = Sse2.Multiply(dataKey1.AsUInt32(), primeVector);
					var productLo2 = Sse2.Multiply(dataKey2.AsUInt32(), primeVector);
					var productLo3 = Sse2.Multiply(dataKey3.AsUInt32(), primeVector);
					var productHi0 = Sse2.Multiply(dataKeyHi0.AsUInt32(), primeVector);
					var productHi1 = Sse2.Multiply(dataKeyHi1.AsUInt32(), primeVector);
					var productHi2 = Sse2.Multiply(dataKeyHi2.AsUInt32(), primeVector);
					var productHi3 = Sse2.Multiply(dataKeyHi3.AsUInt32(), primeVector);

					productHi0 = Sse2.ShiftLeftLogical(productHi0, 32);
					productHi1 = Sse2.ShiftLeftLogical(productHi1, 32);
					productHi2 = Sse2.ShiftLeftLogical(productHi2, 32);
					productHi3 = Sse2.ShiftLeftLogical(productHi3, 32);

					var sum0 = Sse2.Add(productLo0, productHi0);
					var sum1 = Sse2.Add(productLo1, productHi1);
					var sum2 = Sse2.Add(productLo2, productHi2);
					var sum3 = Sse2.Add(productLo3, productHi3);

					accumulator0 = sum0;
					accumulator1 = sum1;
					accumulator2 = sum2;
					accumulator3 = sum3;
				}
			}

			uint stripeCount = (length - 1u - (BlockLength * blocks)) / StripeLength;
			{
				byte* lData = data + (blocks * BlockLength);
				uint stripe = 0;

				for (; stripe + 1 < stripeCount; stripe += 2) {
					PrefetchNonTemporalNext(lData + (stripe * StripeLength) + 0x040);
					PrefetchNonTemporalNext(lData + (stripe * StripeLength) + 0x080);
					{
						byte* llData = lData + (stripe * StripeLength);
						byte* llSecret = secret + (stripe * 8u);

						Vector128<ulong> result0, result1, result2, result3;

						{
							var dataVec0 = LoadVector(llData + 0x00u);
							var dataVec1 = LoadVector(llData + 0x10u);
							var dataVec2 = LoadVector(llData + 0x20u);
							var dataVec3 = LoadVector(llData + 0x30u);
							var keyVec0 = LoadVector(llSecret + 0x00u);
							var keyVec1 = LoadVector(llSecret + 0x10u);
							var keyVec2 = LoadVector(llSecret + 0x20u);
							var keyVec3 = LoadVector(llSecret + 0x30u);

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
							var addend0 = accumulator0;
							var addend1 = accumulator1;
							var addend2 = accumulator2;
							var addend3 = accumulator3;

							var sum0 = Sse2.Add(addend0, dataSwap0.AsUInt64());
							var sum1 = Sse2.Add(addend1, dataSwap1.AsUInt64());
							var sum2 = Sse2.Add(addend2, dataSwap2.AsUInt64());
							var sum3 = Sse2.Add(addend3, dataSwap3.AsUInt64());

							result0 = Sse2.Add(product0, sum0);
							result1 = Sse2.Add(product1, sum1);
							result2 = Sse2.Add(product2, sum2);
							result3 = Sse2.Add(product3, sum3);
						}

						{
							var dataVec0 = LoadVector(llData + 0x40u);
							var dataVec1 = LoadVector(llData + 0x50u);
							var dataVec2 = LoadVector(llData + 0x60u);
							var dataVec3 = LoadVector(llData + 0x70u);
							var keyVec0 = LoadVector(llSecret + 0x08u);
							var keyVec1 = LoadVector(llSecret + 0x18u);
							var keyVec2 = LoadVector(llSecret + 0x28u);
							var keyVec3 = LoadVector(llSecret + 0x38u);

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
							var addend0 = result0;
							var addend1 = result1;
							var addend2 = result2;
							var addend3 = result3;

							var sum0 = Sse2.Add(addend0, dataSwap0.AsUInt64());
							var sum1 = Sse2.Add(addend1, dataSwap1.AsUInt64());
							var sum2 = Sse2.Add(addend2, dataSwap2.AsUInt64());
							var sum3 = Sse2.Add(addend3, dataSwap3.AsUInt64());

							accumulator0 = Sse2.Add(product0, sum0);
							accumulator1 = Sse2.Add(product1, sum1);
							accumulator2 = Sse2.Add(product2, sum2);
							accumulator3 = Sse2.Add(product3, sum3);
						}
					}
				}

				if (stripe < stripeCount) {
					PrefetchNonTemporalNext(lData + (stripe * StripeLength) + 0x040);
					{
						byte* llData = lData + (stripe * StripeLength);
						byte* llSecret = secret + (stripe * 8u);

						var dataVec0 = LoadVector(llData + 0x00u);
						var dataVec1 = LoadVector(llData + 0x10u);
						var dataVec2 = LoadVector(llData + 0x20u);
						var dataVec3 = LoadVector(llData + 0x30u);
						var keyVec0 = LoadVector(llSecret + 0x00u);
						var keyVec1 = LoadVector(llSecret + 0x10u);
						var keyVec2 = LoadVector(llSecret + 0x20u);
						var keyVec3 = LoadVector(llSecret + 0x30u);

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
						var addend0 = accumulator0;
						var addend1 = accumulator1;
						var addend2 = accumulator2;
						var addend3 = accumulator3;

						var sum0 = Sse2.Add(addend0, dataSwap0.AsUInt64());
						var sum1 = Sse2.Add(addend1, dataSwap1.AsUInt64());
						var sum2 = Sse2.Add(addend2, dataSwap2.AsUInt64());
						var sum3 = Sse2.Add(addend3, dataSwap3.AsUInt64());

						var result0 = Sse2.Add(product0, sum0);
						var result1 = Sse2.Add(product1, sum1);
						var result2 = Sse2.Add(product2, sum2);
						var result3 = Sse2.Add(product3, sum3);

						accumulator0 = result0;
						accumulator1 = result1;
						accumulator2 = result2;
						accumulator3 = result3;
					}
				}
			}

			{
				byte* lData = data + (length - StripeLength);
				byte* lSecret = secret + (SecretLength - StripeLength - 7u);

				var dataVec0 = LoadVector(lData + 0x00u);
				var dataVec1 = LoadVector(lData + 0x10u);
				var dataVec2 = LoadVector(lData + 0x20u);
				var dataVec3 = LoadVector(lData + 0x30u);
				var keyVec0 = LoadVector(lSecret + 0x00u);
				var keyVec1 = LoadVector(lSecret + 0x10u);
				var keyVec2 = LoadVector(lSecret + 0x20u);
				var keyVec3 = LoadVector(lSecret + 0x30u);

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
				var addend0 = accumulator0;
				var addend1 = accumulator1;
				var addend2 = accumulator2;
				var addend3 = accumulator3;

				var sum0 = Sse2.Add(addend0, dataSwap0.AsUInt64());
				var sum1 = Sse2.Add(addend1, dataSwap1.AsUInt64());
				var sum2 = Sse2.Add(addend2, dataSwap2.AsUInt64());
				var sum3 = Sse2.Add(addend3, dataSwap3.AsUInt64());

				var result0 = Sse2.Add(product0, sum0);
				var result1 = Sse2.Add(product1, sum1);
				var result2 = Sse2.Add(product2, sum2);
				var result3 = Sse2.Add(product3, sum3);

				accumulator0 = result0;
				accumulator1 = result1;
				accumulator2 = result2;
				accumulator3 = result3;
			}

			ulong result = unchecked(length * Prime64.Prime0);

			var data0 = Sse2.Xor(accumulator0, Vector128.Create(SecretValues64.Secret0B, SecretValues64.Secret13));
			var data1 = Sse2.Xor(accumulator1, Vector128.Create(SecretValues64.Secret1B, SecretValues64.Secret23));
			var data2 = Sse2.Xor(accumulator2, Vector128.Create(SecretValues64.Secret2B, SecretValues64.Secret33));
			var data3 = Sse2.Xor(accumulator3, Vector128.Create(SecretValues64.Secret3B, SecretValues64.Secret43));

			result += MixAccumulators(data0.GetElement(0), data0.GetElement(1));
			result += MixAccumulators(data1.GetElement(0), data1.GetElement(1));
			result += MixAccumulators(data2.GetElement(0), data2.GetElement(1));
			result += MixAccumulators(data3.GetElement(0), data3.GetElement(1));

			return Avalanche(result);
		}
	}
}
