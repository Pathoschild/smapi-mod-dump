/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;

namespace SpriteMaster.Caching;

internal static partial class TextureFileCache {
	// https://godbolt.org/z/rdq3xP15E

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe void ProcessTextureAvx2(Span<Color8> data) {
		uint registerElements = (uint)Vector256<uint>.Count;
		registerElements.AssertEqual((uint)(sizeof(Vector256<uint>) / sizeof(Color8)));

		uint offset;
		fixed (Color8* dataPtr8 = data) {
			uint* dataPtr = (uint*)dataPtr8;

			for (offset = 0; offset + (registerElements - 1U) < data.Length; offset += registerElements) {
				Vector256<uint> rawColor = Avx2.LoadVector256(dataPtr + offset);

				Vector256<uint> alphaMask = Vector256.Create(0xFF000000U);
				Vector256<uint> alpha = Avx2.And(rawColor, alphaMask);

				Vector256<ushort> lo = Avx2.UnpackLow(rawColor.AsByte(), Vector256<byte>.Zero).AsUInt16();
				Vector256<ushort> hi = Avx2.UnpackHigh(rawColor.AsByte(), Vector256<byte>.Zero).AsUInt16();

				const byte offset0 = 6;
				const byte offset1 = offset0 + 8;
				const byte offset2 = offset1 + 8;
				const byte offset3 = offset2 + 8;
				Vector256<byte> alphaShuffle = Vector256.Create(
					offset0, 0xFF, offset0, 0xFF, offset0, 0xFF, offset0, 0xFF,
					offset1, 0xFF, offset1, 0xFF, offset1, 0xFF, offset1, 0xFF,
					offset2, 0xFF, offset2, 0xFF, offset2, 0xFF, offset2, 0xFF,
					offset3, 0xFF, offset3, 0xFF, offset3, 0xFF, offset3, 0xFF
				);

				Vector256<uint> alphaLo = Avx2.Shuffle(lo.AsByte(), alphaShuffle).AsUInt32();
				Vector256<uint> alphaHi = Avx2.Shuffle(hi.AsByte(), alphaShuffle).AsUInt32();

				Vector256<ushort> prodLo = Avx2.MultiplyLow(lo, alphaLo.AsUInt16());
				Vector256<ushort> prodHi = Avx2.MultiplyLow(hi, alphaHi.AsUInt16());

				Vector256<ushort> addend = Vector256.Create((ushort)0x00FFU);

				var sumLo = Avx2.Add(prodLo, addend);
				var sumHi = Avx2.Add(prodHi, addend);

				var shiftLo = Avx2.ShiftRightLogical(sumLo, 8);
				var shiftHi = Avx2.ShiftRightLogical(sumHi, 8);

				var packed = Avx2.PackUnsignedSaturate(shiftLo.AsInt16(), shiftHi.AsInt16()).AsUInt32();

				var mask = Vector256.Create(0x00FFFFFFU);
				packed = Avx2.And(packed, mask);
				packed = Avx2.Or(packed, alpha);

				Avx2.Store(dataPtr + offset, packed);
			}
		}

		// This is unlikely to happen, but handle when there are still elements left (the texture size isn't aligned to 4)
		if (offset < data.Length) {
			ProcessTextureScalar(data.Slice((int)offset));
		}
	}
}
