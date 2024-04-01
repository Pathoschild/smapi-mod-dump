/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable AccessToStaticMemberViaDerivedType

namespace SpriteMaster.Hashing.Algorithms;

internal static unsafe partial class XxHash3 {
	private static partial class Avx2Impl {
		[Pure]
		[MethodImpl(Inline)]
		internal static ulong HashLong(SegmentedSpan span, uint length) {
			if (span.Width.IsAligned(BlockLength)) {
				return HashLongPerBlock(span, length);
			}

			if (span.Width.IsAligned(StripeLength)) {
				return HashLongPerStripe(span, length);
			}

			return HashLongUnaligned(span, length);
		}

		[Pure]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static ulong HashLongUnaligned(SegmentedSpan span, uint length) {
			byte* secret = Secret;

			uint blocks = (length - 1) / BlockLength;

			Vector256<ulong> accumulator0 = Vector256.Create(Prime32.Prime2, Prime64.Prime0, Prime64.Prime1, Prime64.Prime2);
			Vector256<ulong> accumulator1 = Vector256.Create(Prime64.Prime3, Prime32.Prime1, Prime64.Prime4, Prime32.Prime0);

			var primeVector = Vector256.Create(Prime32.Prime0);

			Span<byte> localData = stackalloc byte[(int)StripeLength];

			for (uint block = 0; block < blocks; ++block) {
				{
					uint lDataOffset = block * BlockLength;

					for (uint stripe = 0; stripe < StripesPerBlock; ++stripe) {
						uint llDataOffset = lDataOffset + (stripe * StripeLength);
						byte* llSecret = secret + (stripe * 8u);

						byte* llData = span.SlicePointer(localData, llDataOffset);
						var keyVec0 = LoadVector(llSecret + 0x00u);
						var keyVec1 = LoadVector(llSecret + 0x20u);
						var dataVec0 = LoadVector(llData + 0x00u);
						var dataVec1 = LoadVector(llData + 0x20u);

						var dataKey0 = Avx2.Xor(dataVec0, keyVec0);
						var dataKey1 = Avx2.Xor(dataVec1, keyVec1);
						var dataKeyLo0 = Avx2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
						var dataKeyLo1 = Avx2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
						var product0 = Avx2.Multiply(dataKey0.AsUInt32(), dataKeyLo0);
						var product1 = Avx2.Multiply(dataKey1.AsUInt32(), dataKeyLo1);

						var dataSwap0 = Avx2.Shuffle(dataVec0.AsUInt32(), ShuffleDataSwap);
						var dataSwap1 = Avx2.Shuffle(dataVec1.AsUInt32(), ShuffleDataSwap);
						var addend0 = accumulator0;
						var addend1 = accumulator1;
						var sum0 = Avx2.Add(addend0, dataSwap0.AsUInt64());
						var sum1 = Avx2.Add(addend1, dataSwap1.AsUInt64());

						var result0 = Avx2.Add(product0, sum0);
						var result1 = Avx2.Add(product1, sum1);

						accumulator0 = result0;
						accumulator1 = result1;
					}
				}
				{
					byte* lSecret = secret + (SecretLength - StripeLength);

					var accumulatorVec0 = accumulator0;
					var accumulatorVec1 = accumulator1;
					var shifted0 = Avx2.ShiftRightLogical(accumulatorVec0, 47);
					var shifted1 = Avx2.ShiftRightLogical(accumulatorVec1, 47);
					var dataVec0 = Avx2.Xor(accumulatorVec0, shifted0);
					var dataVec1 = Avx2.Xor(accumulatorVec1, shifted1);

					var keyVec0 = LoadVector(lSecret + 0x00u);
					var keyVec1 = LoadVector(lSecret + 0x20u);
					var dataKey0 = Avx2.Xor(dataVec0, keyVec0);
					var dataKey1 = Avx2.Xor(dataVec1, keyVec1);

					var dataKeyHi0 = Avx2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
					var dataKeyHi1 = Avx2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
					var productLo0 = Avx2.Multiply(dataKey0.AsUInt32(), primeVector);
					var productLo1 = Avx2.Multiply(dataKey1.AsUInt32(), primeVector);
					var productHi0 = Avx2.Multiply(dataKeyHi0.AsUInt32(), primeVector);
					var productHi1 = Avx2.Multiply(dataKeyHi1.AsUInt32(), primeVector);

					productHi0 = Avx2.ShiftLeftLogical(productHi0, 32);
					productHi1 = Avx2.ShiftLeftLogical(productHi1, 32);

					var sum0 = Avx2.Add(productLo0, productHi0);
					var sum1 = Avx2.Add(productLo1, productHi1);

					accumulator0 = sum0;
					accumulator1 = sum1;
				}
			}

			uint stripeCount = (length - 1u - (BlockLength * blocks)) / StripeLength;
			{
				uint lDataOffset = blocks * BlockLength;

				uint stripe = 0;

				for (; stripe < stripeCount; ++stripe) {
					uint llDataOffset = lDataOffset + (stripe * StripeLength);
					byte* llSecret = secret + (stripe * 8u);

					byte* llData = span.SlicePointer(localData, llDataOffset);
					var keyVec0 = LoadVector(llSecret + 0x00u);
					var keyVec1 = LoadVector(llSecret + 0x20u);
					var dataVec0 = LoadVector(llData + 0x00u);
					var dataVec1 = LoadVector(llData + 0x20u);

					var dataKey0 = Avx2.Xor(dataVec0, keyVec0);
					var dataKey1 = Avx2.Xor(dataVec1, keyVec1);
					var dataKeyLo0 = Avx2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
					var dataKeyLo1 = Avx2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
					var product0 = Avx2.Multiply(dataKey0.AsUInt32(), dataKeyLo0);
					var product1 = Avx2.Multiply(dataKey1.AsUInt32(), dataKeyLo1);

					var dataSwap0 = Avx2.Shuffle(dataVec0.AsUInt32(), ShuffleDataSwap);
					var dataSwap1 = Avx2.Shuffle(dataVec1.AsUInt32(), ShuffleDataSwap);
					var addend0 = accumulator0;
					var addend1 = accumulator1;
					var sum0 = Avx2.Add(addend0, dataSwap0.AsUInt64());
					var sum1 = Avx2.Add(addend1, dataSwap1.AsUInt64());

					var result0 = Avx2.Add(product0, sum0);
					var result1 = Avx2.Add(product1, sum1);

					accumulator0 = result0;
					accumulator1 = result1;
				}
			}

			{
				uint lDataOffset = length - StripeLength;
				byte* lSecret = secret + (SecretLength - StripeLength - 7u);

				byte* lData = span.SlicePointer(localData, lDataOffset);
				{
					var keyVec0 = LoadVector(lSecret + 0x00u);
					var keyVec1 = LoadVector(lSecret + 0x20u);
					var dataVec0 = LoadVector(lData + 0x00u);
					var dataVec1 = LoadVector(lData + 0x20u);

					var dataKey0 = Avx2.Xor(dataVec0, keyVec0);
					var dataKey1 = Avx2.Xor(dataVec1, keyVec1);
					var dataKeyLo0 = Avx2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
					var dataKeyLo1 = Avx2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
					var product0 = Avx2.Multiply(dataKey0.AsUInt32(), dataKeyLo0);
					var product1 = Avx2.Multiply(dataKey1.AsUInt32(), dataKeyLo1);

					var dataSwap0 = Avx2.Shuffle(dataVec0.AsUInt32(), ShuffleDataSwap);
					var dataSwap1 = Avx2.Shuffle(dataVec1.AsUInt32(), ShuffleDataSwap);
					var addend0 = accumulator0;
					var addend1 = accumulator1;
					var sum0 = Avx2.Add(addend0, dataSwap0.AsUInt64());
					var sum1 = Avx2.Add(addend1, dataSwap1.AsUInt64());

					var result0 = Avx2.Add(product0, sum0);
					var result1 = Avx2.Add(product1, sum1);

					accumulator0 = result0;
					accumulator1 = result1;
				}
			}

			ulong result = unchecked(length * Prime64.Prime0);

			var data0 = Avx2.Xor(
				accumulator0,
				Vector256.Create(
					SecretValues64.Secret0B, SecretValues64.Secret13, SecretValues64.Secret1B, SecretValues64.Secret23
				)
			);
			var data1 = Avx2.Xor(
				accumulator1,
				Vector256.Create(
					SecretValues64.Secret2B, SecretValues64.Secret33, SecretValues64.Secret3B, SecretValues64.Secret43
				)
			);

			result += MixAccumulators(data0.GetElement(0), data0.GetElement(1));
			result += MixAccumulators(data0.GetElement(2), data0.GetElement(3));
			result += MixAccumulators(data1.GetElement(0), data1.GetElement(1));
			result += MixAccumulators(data1.GetElement(2), data1.GetElement(3));

			return Avalanche(result);
		}

		[Pure]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static ulong HashLongPerBlock(SegmentedSpan span, uint length) {
			// Span rows are aligned per-block.
			uint rowsPerBlock = span.Width / BlockLength;

			return HashLongPerBlock(span, length, rowsPerBlock);
		}

		[Pure]
		[MethodImpl(Inline)]
		private static ulong HashLongPerBlock(SegmentedSpan span, uint length, uint rowsPerBlock) {
			byte* secret = Secret;

			uint blocks = (length - 1) / BlockLength;

			Vector256<ulong> accumulator0 = Vector256.Create(Prime32.Prime2, Prime64.Prime0, Prime64.Prime1, Prime64.Prime2);
			Vector256<ulong> accumulator1 = Vector256.Create(Prime64.Prime3, Prime32.Prime1, Prime64.Prime4, Prime32.Prime0);

			var primeVector = Vector256.Create(Prime32.Prime0);

			[MethodImpl(Inline)]
			ReadOnlySpan<byte> GetBlock(SegmentedSpan span, uint block) {
				uint row = block / rowsPerBlock;
				uint spanOffset = (block % rowsPerBlock) * BlockLength;

				return span.GetAtOffset(row, spanOffset, BlockLength);
			}

			for (uint block = 0; block < blocks; ++block) {
				byte* blockData = GetBlock(span, block).AsPointerUnsafe();

				for (uint stripe = 0; stripe < StripesPerBlock; ++stripe) {
					byte* llData = blockData + (stripe * StripeLength);
					byte* llSecret = secret + (stripe * 8u);

					var dataVec0 = LoadVector(llData + 0x00u);
					var dataVec1 = LoadVector(llData + 0x20u);
					var keyVec0 = LoadVector(llSecret + 0x00u);
					var keyVec1 = LoadVector(llSecret + 0x20u);

					var dataKey0 = Avx2.Xor(dataVec0, keyVec0);
					var dataKey1 = Avx2.Xor(dataVec1, keyVec1);
					var dataKeyLo0 = Avx2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
					var dataKeyLo1 = Avx2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
					var product0 = Avx2.Multiply(dataKey0.AsUInt32(), dataKeyLo0);
					var product1 = Avx2.Multiply(dataKey1.AsUInt32(), dataKeyLo1);

					var dataSwap0 = Avx2.Shuffle(dataVec0.AsUInt32(), ShuffleDataSwap);
					var dataSwap1 = Avx2.Shuffle(dataVec1.AsUInt32(), ShuffleDataSwap);
					var addend0 = accumulator0;
					var addend1 = accumulator1;
					var sum0 = Avx2.Add(addend0, dataSwap0.AsUInt64());
					var sum1 = Avx2.Add(addend1, dataSwap1.AsUInt64());

					var result0 = Avx2.Add(product0, sum0);
					var result1 = Avx2.Add(product1, sum1);

					accumulator0 = result0;
					accumulator1 = result1;
				}

				{
					byte* lSecret = secret + (SecretLength - StripeLength);

					var accumulatorVec0 = accumulator0;
					var accumulatorVec1 = accumulator1;
					var shifted0 = Avx2.ShiftRightLogical(accumulatorVec0, 47);
					var shifted1 = Avx2.ShiftRightLogical(accumulatorVec1, 47);
					var dataVec0 = Avx2.Xor(accumulatorVec0, shifted0);
					var dataVec1 = Avx2.Xor(accumulatorVec1, shifted1);

					var keyVec0 = LoadVector(lSecret + 0x00u);
					var keyVec1 = LoadVector(lSecret + 0x20u);
					var dataKey0 = Avx2.Xor(dataVec0, keyVec0);
					var dataKey1 = Avx2.Xor(dataVec1, keyVec1);

					var dataKeyHi0 = Avx2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
					var dataKeyHi1 = Avx2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
					var productLo0 = Avx2.Multiply(dataKey0.AsUInt32(), primeVector);
					var productLo1 = Avx2.Multiply(dataKey1.AsUInt32(), primeVector);
					var productHi0 = Avx2.Multiply(dataKeyHi0.AsUInt32(), primeVector);
					var productHi1 = Avx2.Multiply(dataKeyHi1.AsUInt32(), primeVector);

					productHi0 = Avx2.ShiftLeftLogical(productHi0, 32);
					productHi1 = Avx2.ShiftLeftLogical(productHi1, 32);

					var sum0 = Avx2.Add(productLo0, productHi0);
					var sum1 = Avx2.Add(productLo1, productHi1);

					accumulator0 = sum0;
					accumulator1 = sum1;
				}
			}

			uint stripeCount = (length - 1u - (BlockLength * blocks)) / StripeLength;
			{
				byte* blockData = GetBlock(span, blocks).AsPointerUnsafe();

				for (uint stripe = 0; stripe < stripeCount; ++stripe) {
					byte* llData = blockData + (stripe * StripeLength);
					byte* llSecret = secret + (stripe * 8u);

					var dataVec0 = LoadVector(llData + 0x00u);
					var dataVec1 = LoadVector(llData + 0x20u);
					var keyVec0 = LoadVector(llSecret + 0x00u);
					var keyVec1 = LoadVector(llSecret + 0x20u);

					var dataKey0 = Avx2.Xor(dataVec0, keyVec0);
					var dataKey1 = Avx2.Xor(dataVec1, keyVec1);
					var dataKeyLo0 = Avx2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
					var dataKeyLo1 = Avx2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
					var product0 = Avx2.Multiply(dataKey0.AsUInt32(), dataKeyLo0);
					var product1 = Avx2.Multiply(dataKey1.AsUInt32(), dataKeyLo1);

					var dataSwap0 = Avx2.Shuffle(dataVec0.AsUInt32(), ShuffleDataSwap);
					var dataSwap1 = Avx2.Shuffle(dataVec1.AsUInt32(), ShuffleDataSwap);
					var addend0 = accumulator0;
					var addend1 = accumulator1;
					var sum0 = Avx2.Add(addend0, dataSwap0.AsUInt64());
					var sum1 = Avx2.Add(addend1, dataSwap1.AsUInt64());

					var result0 = Avx2.Add(product0, sum0);
					var result1 = Avx2.Add(product1, sum1);

					accumulator0 = result0;
					accumulator1 = result1;
				}
			}

			{
				byte* lData = span.SlicePointer(stackalloc byte[(int)StripeLength], length - StripeLength);
				byte* lSecret = secret + (SecretLength - StripeLength - 7u);

				var dataVec0 = LoadVector(lData + 0x00u);
				var dataVec1 = LoadVector(lData + 0x20u);
				var keyVec0 = LoadVector(lSecret + 0x00u);
				var keyVec1 = LoadVector(lSecret + 0x20u);

				var dataKey0 = Avx2.Xor(dataVec0, keyVec0);
				var dataKey1 = Avx2.Xor(dataVec1, keyVec1);
				var dataKeyLo0 = Avx2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
				var dataKeyLo1 = Avx2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
				var product0 = Avx2.Multiply(dataKey0.AsUInt32(), dataKeyLo0);
				var product1 = Avx2.Multiply(dataKey1.AsUInt32(), dataKeyLo1);

				var dataSwap0 = Avx2.Shuffle(dataVec0.AsUInt32(), ShuffleDataSwap);
				var dataSwap1 = Avx2.Shuffle(dataVec1.AsUInt32(), ShuffleDataSwap);
				var addend0 = accumulator0;
				var addend1 = accumulator1;
				var sum0 = Avx2.Add(addend0, dataSwap0.AsUInt64());
				var sum1 = Avx2.Add(addend1, dataSwap1.AsUInt64());

				var result0 = Avx2.Add(product0, sum0);
				var result1 = Avx2.Add(product1, sum1);

				accumulator0 = result0;
				accumulator1 = result1;
			}

			ulong result = unchecked(length * Prime64.Prime0);

			var data0 = Avx2.Xor(
				accumulator0,
				Vector256.Create(
					SecretValues64.Secret0B, SecretValues64.Secret13, SecretValues64.Secret1B, SecretValues64.Secret23
				)
			);
			var data1 = Avx2.Xor(
				accumulator1,
				Vector256.Create(
					SecretValues64.Secret2B, SecretValues64.Secret33, SecretValues64.Secret3B, SecretValues64.Secret43
				)
			);

			result += MixAccumulators(data0.GetElement(0), data0.GetElement(1));
			result += MixAccumulators(data0.GetElement(2), data0.GetElement(3));
			result += MixAccumulators(data1.GetElement(0), data1.GetElement(1));
			result += MixAccumulators(data1.GetElement(2), data1.GetElement(3));

			return Avalanche(result);
		}

		[Pure]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static ulong HashLongPerStripe(SegmentedSpan span, uint length) {
			// Span rows are aligned per-stripe.
			uint rowsPerStripe = span.Width / StripeLength;

			return HashLongPerStripe(span, length, rowsPerStripe);
		}

		[Pure]
		[MethodImpl(Inline)]
		private static ulong HashLongPerStripe(SegmentedSpan span, uint length, uint rowsPerStripe) {
			byte* secret = Secret;

			uint blocks = (length - 1) / BlockLength;

			Vector256<ulong> accumulator0 = Vector256.Create(Prime32.Prime2, Prime64.Prime0, Prime64.Prime1, Prime64.Prime2);
			Vector256<ulong> accumulator1 = Vector256.Create(Prime64.Prime3, Prime32.Prime1, Prime64.Prime4, Prime32.Prime0);

			var primeVector = Vector256.Create(Prime32.Prime0);

			[MethodImpl(Inline)]
			ReadOnlySpan<byte> GetStripe(SegmentedSpan span, uint stripe) {
				uint row = stripe / rowsPerStripe;
				uint spanOffset = (stripe % rowsPerStripe) * StripeLength;

				return span.GetAtOffset(row, spanOffset, StripeLength);
			}

			for (uint block = 0, baseStripe = 0; block < blocks; ++block) {
				for (uint stripe = 0; stripe < StripesPerBlock; ++stripe, ++baseStripe) {
					byte* llData = GetStripe(span, baseStripe).AsPointerUnsafe();
					byte* llSecret = secret + (stripe * 8u);

					var dataVec0 = LoadVector(llData + 0x00u);
					var dataVec1 = LoadVector(llData + 0x20u);
					var keyVec0 = LoadVector(llSecret + 0x00u);
					var keyVec1 = LoadVector(llSecret + 0x20u);

					var dataKey0 = Avx2.Xor(dataVec0, keyVec0);
					var dataKey1 = Avx2.Xor(dataVec1, keyVec1);
					var dataKeyLo0 = Avx2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
					var dataKeyLo1 = Avx2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
					var product0 = Avx2.Multiply(dataKey0.AsUInt32(), dataKeyLo0);
					var product1 = Avx2.Multiply(dataKey1.AsUInt32(), dataKeyLo1);

					var dataSwap0 = Avx2.Shuffle(dataVec0.AsUInt32(), ShuffleDataSwap);
					var dataSwap1 = Avx2.Shuffle(dataVec1.AsUInt32(), ShuffleDataSwap);
					var addend0 = accumulator0;
					var addend1 = accumulator1;
					var sum0 = Avx2.Add(addend0, dataSwap0.AsUInt64());
					var sum1 = Avx2.Add(addend1, dataSwap1.AsUInt64());

					var result0 = Avx2.Add(product0, sum0);
					var result1 = Avx2.Add(product1, sum1);

					accumulator0 = result0;
					accumulator1 = result1;
				}

				{
					byte* lSecret = secret + (SecretLength - StripeLength);

					var accumulatorVec0 = accumulator0;
					var accumulatorVec1 = accumulator1;
					var shifted0 = Avx2.ShiftRightLogical(accumulatorVec0, 47);
					var shifted1 = Avx2.ShiftRightLogical(accumulatorVec1, 47);
					var dataVec0 = Avx2.Xor(accumulatorVec0, shifted0);
					var dataVec1 = Avx2.Xor(accumulatorVec1, shifted1);

					var keyVec0 = LoadVector(lSecret + 0x00u);
					var keyVec1 = LoadVector(lSecret + 0x20u);
					var dataKey0 = Avx2.Xor(dataVec0, keyVec0);
					var dataKey1 = Avx2.Xor(dataVec1, keyVec1);

					var dataKeyHi0 = Avx2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
					var dataKeyHi1 = Avx2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
					var productLo0 = Avx2.Multiply(dataKey0.AsUInt32(), primeVector);
					var productLo1 = Avx2.Multiply(dataKey1.AsUInt32(), primeVector);
					var productHi0 = Avx2.Multiply(dataKeyHi0.AsUInt32(), primeVector);
					var productHi1 = Avx2.Multiply(dataKeyHi1.AsUInt32(), primeVector);

					productHi0 = Avx2.ShiftLeftLogical(productHi0, 32);
					productHi1 = Avx2.ShiftLeftLogical(productHi1, 32);

					var sum0 = Avx2.Add(productLo0, productHi0);
					var sum1 = Avx2.Add(productLo1, productHi1);

					accumulator0 = sum0;
					accumulator1 = sum1;
				}
			}

			uint stripeCount = (length - 1u - (BlockLength * blocks)) / StripeLength;
			{
				for (uint stripe = 0, baseStripe = blocks * StripesPerBlock; stripe < stripeCount; ++stripe) {
					byte* llData = GetStripe(span, baseStripe + stripe).AsPointerUnsafe();
					byte* llSecret = secret + (stripe * 8u);

					var dataVec0 = LoadVector(llData + 0x00u);
					var dataVec1 = LoadVector(llData + 0x20u);
					var keyVec0 = LoadVector(llSecret + 0x00u);
					var keyVec1 = LoadVector(llSecret + 0x20u);

					var dataKey0 = Avx2.Xor(dataVec0, keyVec0);
					var dataKey1 = Avx2.Xor(dataVec1, keyVec1);
					var dataKeyLo0 = Avx2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
					var dataKeyLo1 = Avx2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
					var product0 = Avx2.Multiply(dataKey0.AsUInt32(), dataKeyLo0);
					var product1 = Avx2.Multiply(dataKey1.AsUInt32(), dataKeyLo1);

					var dataSwap0 = Avx2.Shuffle(dataVec0.AsUInt32(), ShuffleDataSwap);
					var dataSwap1 = Avx2.Shuffle(dataVec1.AsUInt32(), ShuffleDataSwap);
					var addend0 = accumulator0;
					var addend1 = accumulator1;
					var sum0 = Avx2.Add(addend0, dataSwap0.AsUInt64());
					var sum1 = Avx2.Add(addend1, dataSwap1.AsUInt64());

					var result0 = Avx2.Add(product0, sum0);
					var result1 = Avx2.Add(product1, sum1);

					accumulator0 = result0;
					accumulator1 = result1;
				}
			}

			{
				byte* lData = span.SlicePointer(stackalloc byte[(int)StripeLength], length - StripeLength);
				byte* lSecret = secret + (SecretLength - StripeLength - 7u);

				var dataVec0 = LoadVector(lData + 0x00u);
				var dataVec1 = LoadVector(lData + 0x20u);
				var keyVec0 = LoadVector(lSecret + 0x00u);
				var keyVec1 = LoadVector(lSecret + 0x20u);

				var dataKey0 = Avx2.Xor(dataVec0, keyVec0);
				var dataKey1 = Avx2.Xor(dataVec1, keyVec1);
				var dataKeyLo0 = Avx2.Shuffle(dataKey0.AsUInt32(), ShuffleDataKey);
				var dataKeyLo1 = Avx2.Shuffle(dataKey1.AsUInt32(), ShuffleDataKey);
				var product0 = Avx2.Multiply(dataKey0.AsUInt32(), dataKeyLo0);
				var product1 = Avx2.Multiply(dataKey1.AsUInt32(), dataKeyLo1);

				var dataSwap0 = Avx2.Shuffle(dataVec0.AsUInt32(), ShuffleDataSwap);
				var dataSwap1 = Avx2.Shuffle(dataVec1.AsUInt32(), ShuffleDataSwap);
				var addend0 = accumulator0;
				var addend1 = accumulator1;
				var sum0 = Avx2.Add(addend0, dataSwap0.AsUInt64());
				var sum1 = Avx2.Add(addend1, dataSwap1.AsUInt64());

				var result0 = Avx2.Add(product0, sum0);
				var result1 = Avx2.Add(product1, sum1);

				accumulator0 = result0;
				accumulator1 = result1;
			}

			ulong result = unchecked(length * Prime64.Prime0);

			var data0 = Avx2.Xor(
				accumulator0,
				Vector256.Create(
					SecretValues64.Secret0B, SecretValues64.Secret13, SecretValues64.Secret1B, SecretValues64.Secret23
				)
			);
			var data1 = Avx2.Xor(
				accumulator1,
				Vector256.Create(
					SecretValues64.Secret2B, SecretValues64.Secret33, SecretValues64.Secret3B, SecretValues64.Secret43
				)
			);

			result += MixAccumulators(data0.GetElement(0), data0.GetElement(1));
			result += MixAccumulators(data0.GetElement(2), data0.GetElement(3));
			result += MixAccumulators(data1.GetElement(0), data1.GetElement(1));
			result += MixAccumulators(data1.GetElement(2), data1.GetElement(3));

			return Avalanche(result);
		}
	}
}
