/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System.Runtime.CompilerServices;
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable AccessToStaticMemberViaDerivedType

namespace SpriteMaster.Hashing.Algorithms;

internal static unsafe partial class XxHash3 {
	private static class ScalarImpl {
		// xxh3_accumulate_512_scalar
		[MethodImpl(Inline)]
		internal static void Accumulate512(ulong* accumulator, byte* data, byte* secret) {
			PrefetchNonTemporalNext(data);
			PrefetchNonTemporalNext(secret);
			PrefetchNext(accumulator);

			AccumulatorBytes.AssertEqual(8u);

			Accumulate512Pass(ref accumulator[0 ^ 1], ref accumulator[0], data + 0x00, secret + 0x00);
			Accumulate512Pass(ref accumulator[1 ^ 1], ref accumulator[1], data + 0x08, secret + 0x08);
			Accumulate512Pass(ref accumulator[2 ^ 1], ref accumulator[2], data + 0x10, secret + 0x10);
			Accumulate512Pass(ref accumulator[3 ^ 1], ref accumulator[3], data + 0x18, secret + 0x18);
			Accumulate512Pass(ref accumulator[4 ^ 1], ref accumulator[4], data + 0x20, secret + 0x20);
			Accumulate512Pass(ref accumulator[5 ^ 1], ref accumulator[5], data + 0x28, secret + 0x28);
			Accumulate512Pass(ref accumulator[6 ^ 1], ref accumulator[6], data + 0x30, secret + 0x30);
			Accumulate512Pass(ref accumulator[7 ^ 1], ref accumulator[7], data + 0x38, secret + 0x38);
		}

		[MethodImpl(Inline)]
		private static void Accumulate512Pass(
			ref ulong accumulator0,
			ref ulong accumulator1,
			byte* data,
			byte* secret
		) {
			ulong dataVal = LoadLittle64(data);
			ulong dataKey = dataVal ^ LoadLittle64(secret);

			accumulator0 += dataVal;
			accumulator1 += (uint)dataKey * (ulong)(uint)(dataKey >> 32);
		}

		// xxh3_scramble_acc_scalar
		[MethodImpl(Inline)]
		internal static void ScrambleAccumulator(ulong* accumulator, byte* secret) {
			ScrambleAccumulatorPass(ref accumulator[0], secret + 0x00);
			ScrambleAccumulatorPass(ref accumulator[1], secret + 0x08);
			ScrambleAccumulatorPass(ref accumulator[2], secret + 0x10);
			ScrambleAccumulatorPass(ref accumulator[3], secret + 0x18);
			ScrambleAccumulatorPass(ref accumulator[4], secret + 0x20);
			ScrambleAccumulatorPass(ref accumulator[5], secret + 0x28);
			ScrambleAccumulatorPass(ref accumulator[6], secret + 0x30);
			ScrambleAccumulatorPass(ref accumulator[7], secret + 0x38);
		}

		[MethodImpl(Inline)]
		private static void ScrambleAccumulatorPass(ref ulong accumulator, byte* secret) {
			ulong key64 = LoadLittle64(secret);
			ulong acc64 = accumulator;

			acc64 ^= acc64 >> 47;
			acc64 ^= key64;
			acc64 *= Prime32.Prime0;

			accumulator = acc64;
		}
	}
}