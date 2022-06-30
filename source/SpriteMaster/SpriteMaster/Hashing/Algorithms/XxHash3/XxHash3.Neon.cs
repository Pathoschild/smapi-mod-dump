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
using System.Runtime.Intrinsics.Arm;
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable AccessToStaticMemberViaDerivedType

namespace SpriteMaster.Hashing.Algorithms;

internal static unsafe partial class XxHash3 {
	private static class NeonImpl {
		// xxh3_accumulate_512_neon
		[MethodImpl(Inline)]
		internal static void Accumulate512(ulong* accumulatorStore, byte* data, byte* secret) {
			ref var accumulator = ref *(CombinedVector128X512<ulong>*)accumulatorStore;

			var accumulatorVecLo0 = accumulator.Data0;
			var accumulatorVecLo1 = accumulator.Data1;
			var accumulatorVecLo2 = accumulator.Data2;
			var accumulatorVecLo3 = accumulator.Data3;

			var dataVec0 = AdvSimd.LoadVector128((ulong*)(data + 0x00u));
			var dataVec1 = AdvSimd.LoadVector128((ulong*)(data + 0x10u));
			var dataVec2 = AdvSimd.LoadVector128((ulong*)(data + 0x20u));
			var dataVec3 = AdvSimd.LoadVector128((ulong*)(data + 0x30u));
			var keyVec0 = AdvSimd.LoadVector128((ulong*)(secret + 0x00u));
			var keyVec1 = AdvSimd.LoadVector128((ulong*)(secret + 0x10u));
			var keyVec2 = AdvSimd.LoadVector128((ulong*)(secret + 0x20u));
			var keyVec3 = AdvSimd.LoadVector128((ulong*)(secret + 0x30u));

			var accumulatorVecHi0 = AdvSimd.ExtractVector128(dataVec0, dataVec0, 1);
			var accumulatorVecHi1 = AdvSimd.ExtractVector128(dataVec1, dataVec1, 1);
			var accumulatorVecHi2 = AdvSimd.ExtractVector128(dataVec2, dataVec2, 1);
			var accumulatorVecHi3 = AdvSimd.ExtractVector128(dataVec3, dataVec3, 1);

			var dataKey0 = AdvSimd.Xor(dataVec0, keyVec0);
			var dataKey1 = AdvSimd.Xor(dataVec1, keyVec1);
			var dataKey2 = AdvSimd.Xor(dataVec2, keyVec2);
			var dataKey3 = AdvSimd.Xor(dataVec3, keyVec3);

			var dataKeyLo0 = AdvSimd.ExtractNarrowingLower(dataKey0);
			var dataKeyLo1 = AdvSimd.ExtractNarrowingLower(dataKey1);
			var dataKeyLo2 = AdvSimd.ExtractNarrowingLower(dataKey2);
			var dataKeyLo3 = AdvSimd.ExtractNarrowingLower(dataKey3);
			var dataKeyHi0 = AdvSimd.ShiftRightLogicalNarrowingLower(dataKey0, 32);
			var dataKeyHi1 = AdvSimd.ShiftRightLogicalNarrowingLower(dataKey1, 32);
			var dataKeyHi2 = AdvSimd.ShiftRightLogicalNarrowingLower(dataKey2, 32);
			var dataKeyHi3 = AdvSimd.ShiftRightLogicalNarrowingLower(dataKey3, 32);

			accumulatorVecHi0 = AdvSimd.MultiplyWideningLowerAndAdd(accumulatorVecHi0, dataKeyLo0, dataKeyHi0);
			accumulatorVecHi1 = AdvSimd.MultiplyWideningLowerAndAdd(accumulatorVecHi1, dataKeyLo1, dataKeyHi1);
			accumulatorVecHi2 = AdvSimd.MultiplyWideningLowerAndAdd(accumulatorVecHi2, dataKeyLo2, dataKeyHi2);
			accumulatorVecHi3 = AdvSimd.MultiplyWideningLowerAndAdd(accumulatorVecHi3, dataKeyLo3, dataKeyHi3);

			var result0 = AdvSimd.Add(accumulatorVecLo0, accumulatorVecHi0);
			var result1 = AdvSimd.Add(accumulatorVecLo1, accumulatorVecHi1);
			var result2 = AdvSimd.Add(accumulatorVecLo2, accumulatorVecHi2);
			var result3 = AdvSimd.Add(accumulatorVecLo3, accumulatorVecHi3);

			accumulator.Data0 = result0;
			accumulator.Data1 = result1;
			accumulator.Data2 = result2;
			accumulator.Data3 = result3;
		}

		// xxh3_scramble_acc_neon
		[MethodImpl(Inline)]
		internal static void ScrambleAccumulator(ulong* accumulatorStore, byte* secret) {
			ref var accumulator = ref *(CombinedVector128X512<ulong>*)accumulatorStore;

			var primeVector = Vector64.Create(Prime32.Prime0);

			var accumulatorVec0 = accumulator.Data0;
			var accumulatorVec1 = accumulator.Data1;
			var accumulatorVec2 = accumulator.Data2;
			var accumulatorVec3 = accumulator.Data3;
			var shifted0 = AdvSimd.ShiftRightLogical(accumulatorVec0, 47);
			var shifted1 = AdvSimd.ShiftRightLogical(accumulatorVec1, 47);
			var shifted2 = AdvSimd.ShiftRightLogical(accumulatorVec2, 47);
			var shifted3 = AdvSimd.ShiftRightLogical(accumulatorVec3, 47);
			var dataVec0 = AdvSimd.Xor(accumulatorVec0, shifted0);
			var dataVec1 = AdvSimd.Xor(accumulatorVec1, shifted1);
			var dataVec2 = AdvSimd.Xor(accumulatorVec2, shifted2);
			var dataVec3 = AdvSimd.Xor(accumulatorVec3, shifted3);

			var keyVec0 = AdvSimd.LoadVector128((ulong*)(secret + 0x00u)).AsUInt64();
			var keyVec1 = AdvSimd.LoadVector128((ulong*)(secret + 0x10u)).AsUInt64();
			var keyVec2 = AdvSimd.LoadVector128((ulong*)(secret + 0x20u)).AsUInt64();
			var keyVec3 = AdvSimd.LoadVector128((ulong*)(secret + 0x30u)).AsUInt64();
			var dataKey0 = AdvSimd.Xor(dataVec0, keyVec0.AsUInt64());
			var dataKey1 = AdvSimd.Xor(dataVec1, keyVec1.AsUInt64());
			var dataKey2 = AdvSimd.Xor(dataVec2, keyVec2.AsUInt64());
			var dataKey3 = AdvSimd.Xor(dataVec3, keyVec3.AsUInt64());

			var dataKeyLo0 = AdvSimd.ExtractNarrowingLower(dataKey0);
			var dataKeyLo1 = AdvSimd.ExtractNarrowingLower(dataKey1);
			var dataKeyLo2 = AdvSimd.ExtractNarrowingLower(dataKey2);
			var dataKeyLo3 = AdvSimd.ExtractNarrowingLower(dataKey3);
			var dataKeyHi0 = AdvSimd.ShiftRightLogicalNarrowingLower(dataKey0, 32);
			var dataKeyHi1 = AdvSimd.ShiftRightLogicalNarrowingLower(dataKey1, 32);
			var dataKeyHi2 = AdvSimd.ShiftRightLogicalNarrowingLower(dataKey2, 32);
			var dataKeyHi3 = AdvSimd.ShiftRightLogicalNarrowingLower(dataKey3, 32);

			var productHi0 = AdvSimd.MultiplyWideningLower(dataKeyHi0, primeVector);
			var productHi1 = AdvSimd.MultiplyWideningLower(dataKeyHi1, primeVector);
			var productHi2 = AdvSimd.MultiplyWideningLower(dataKeyHi2, primeVector);
			var productHi3 = AdvSimd.MultiplyWideningLower(dataKeyHi3, primeVector);
			productHi0 = AdvSimd.ShiftLeftLogical(productHi0, 32);
			productHi1 = AdvSimd.ShiftLeftLogical(productHi1, 32);
			productHi2 = AdvSimd.ShiftLeftLogical(productHi2, 32);
			productHi3 = AdvSimd.ShiftLeftLogical(productHi3, 32);
			var result0 = AdvSimd.MultiplyWideningLowerAndAdd(productHi0, dataKeyLo0, primeVector);
			var result1 = AdvSimd.MultiplyWideningLowerAndAdd(productHi1, dataKeyLo1, primeVector);
			var result2 = AdvSimd.MultiplyWideningLowerAndAdd(productHi2, dataKeyLo2, primeVector);
			var result3 = AdvSimd.MultiplyWideningLowerAndAdd(productHi3, dataKeyLo3, primeVector);

			accumulator.Data0 = result0;
			accumulator.Data1 = result1;
			accumulator.Data2 = result2;
			accumulator.Data3 = result3;
		}
	}
}
