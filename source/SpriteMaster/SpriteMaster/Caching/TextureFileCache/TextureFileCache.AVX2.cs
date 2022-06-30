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
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;

namespace SpriteMaster.Caching;

internal static partial class TextureFileCache {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe void ProcessTextureAvx2Unrolled(Span<Color8> data) {
		uint registerElements = (uint)Vector256<uint>.Count * 4;
		registerElements.AssertEqual((uint)(sizeof(Vector256<uint>) / sizeof(Color8)));

		uint offset;
		fixed (Color8* dataPtr8 = data) {
			uint* dataPtr = (uint*)dataPtr8;

			for (offset = 0; offset + (registerElements - 1U) < data.Length; offset += registerElements) {
				Vector256<uint> rawColor0 = Avx2.LoadVector256(dataPtr + offset + 0x00);
				Vector256<uint> rawColor1 = Avx2.LoadVector256(dataPtr + offset + 0x08);
				Vector256<uint> rawColor2 = Avx2.LoadVector256(dataPtr + offset + 0x10);
				Vector256<uint> rawColor3 = Avx2.LoadVector256(dataPtr + offset + 0x18);

				Vector256<uint> alphaMask = Vector256.Create(0xFF000000U);
				Vector256<uint> alpha0 = Avx2.And(rawColor0, alphaMask);
				Vector256<uint> alpha1 = Avx2.And(rawColor1, alphaMask);
				Vector256<uint> alpha2 = Avx2.And(rawColor2, alphaMask);
				Vector256<uint> alpha3 = Avx2.And(rawColor3, alphaMask);

				Vector256<ushort> lo0 = Avx2.UnpackLow(rawColor0.AsByte(), Vector256<byte>.Zero).AsUInt16();
				Vector256<ushort> lo1 = Avx2.UnpackLow(rawColor1.AsByte(), Vector256<byte>.Zero).AsUInt16();
				Vector256<ushort> lo2 = Avx2.UnpackLow(rawColor2.AsByte(), Vector256<byte>.Zero).AsUInt16();
				Vector256<ushort> lo3 = Avx2.UnpackLow(rawColor3.AsByte(), Vector256<byte>.Zero).AsUInt16();
				Vector256<ushort> hi0 = Avx2.UnpackHigh(rawColor0.AsByte(), Vector256<byte>.Zero).AsUInt16();
				Vector256<ushort> hi1 = Avx2.UnpackHigh(rawColor1.AsByte(), Vector256<byte>.Zero).AsUInt16();
				Vector256<ushort> hi2 = Avx2.UnpackHigh(rawColor2.AsByte(), Vector256<byte>.Zero).AsUInt16();
				Vector256<ushort> hi3 = Avx2.UnpackHigh(rawColor3.AsByte(), Vector256<byte>.Zero).AsUInt16();

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

				Vector256<uint> alphaLo0 = Avx2.Shuffle(lo0.AsByte(), alphaShuffle).AsUInt32();
				Vector256<uint> alphaLo1 = Avx2.Shuffle(lo1.AsByte(), alphaShuffle).AsUInt32();
				Vector256<uint> alphaLo2 = Avx2.Shuffle(lo2.AsByte(), alphaShuffle).AsUInt32();
				Vector256<uint> alphaLo3 = Avx2.Shuffle(lo3.AsByte(), alphaShuffle).AsUInt32();
				Vector256<uint> alphaHi0 = Avx2.Shuffle(hi0.AsByte(), alphaShuffle).AsUInt32();
				Vector256<uint> alphaHi1 = Avx2.Shuffle(hi1.AsByte(), alphaShuffle).AsUInt32();
				Vector256<uint> alphaHi2 = Avx2.Shuffle(hi2.AsByte(), alphaShuffle).AsUInt32();
				Vector256<uint> alphaHi3 = Avx2.Shuffle(hi3.AsByte(), alphaShuffle).AsUInt32();

				Vector256<ushort> prodLo0 = Avx2.MultiplyLow(lo0, alphaLo0.AsUInt16());
				Vector256<ushort> prodLo1 = Avx2.MultiplyLow(lo1, alphaLo1.AsUInt16());
				Vector256<ushort> prodLo2 = Avx2.MultiplyLow(lo2, alphaLo2.AsUInt16());
				Vector256<ushort> prodLo3 = Avx2.MultiplyLow(lo3, alphaLo3.AsUInt16());
				Vector256<ushort> prodHi0 = Avx2.MultiplyLow(hi0, alphaHi0.AsUInt16());
				Vector256<ushort> prodHi1 = Avx2.MultiplyLow(hi1, alphaHi1.AsUInt16());
				Vector256<ushort> prodHi2 = Avx2.MultiplyLow(hi2, alphaHi2.AsUInt16());
				Vector256<ushort> prodHi3 = Avx2.MultiplyLow(hi3, alphaHi3.AsUInt16());

				Vector256<ushort> addend = Vector256.Create((ushort)0x00FFU);

				var sumLo0 = Avx2.Add(prodLo0, addend);
				var sumLo1 = Avx2.Add(prodLo1, addend);
				var sumLo2 = Avx2.Add(prodLo2, addend);
				var sumLo3 = Avx2.Add(prodLo3, addend);
				var sumHi0 = Avx2.Add(prodHi0, addend);
				var sumHi1 = Avx2.Add(prodHi1, addend);
				var sumHi2 = Avx2.Add(prodHi2, addend);
				var sumHi3 = Avx2.Add(prodHi3, addend);

				var shiftLo0 = Avx2.ShiftRightLogical(sumLo0, 8);
				var shiftLo1 = Avx2.ShiftRightLogical(sumLo1, 8);
				var shiftLo2 = Avx2.ShiftRightLogical(sumLo2, 8);
				var shiftLo3 = Avx2.ShiftRightLogical(sumLo3, 8);
				var shiftHi0 = Avx2.ShiftRightLogical(sumHi0, 8);
				var shiftHi1 = Avx2.ShiftRightLogical(sumHi1, 8);
				var shiftHi2 = Avx2.ShiftRightLogical(sumHi2, 8);
				var shiftHi3 = Avx2.ShiftRightLogical(sumHi3, 8);

				var packed0 = Avx2.PackUnsignedSaturate(shiftLo0.AsInt16(), shiftHi0.AsInt16()).AsUInt32();
				var packed1 = Avx2.PackUnsignedSaturate(shiftLo1.AsInt16(), shiftHi1.AsInt16()).AsUInt32();
				var packed2 = Avx2.PackUnsignedSaturate(shiftLo2.AsInt16(), shiftHi2.AsInt16()).AsUInt32();
				var packed3 = Avx2.PackUnsignedSaturate(shiftLo3.AsInt16(), shiftHi3.AsInt16()).AsUInt32();

				var mask = Vector256.Create(0x00FFFFFFU);
				packed0 = Avx2.And(packed0, mask);
				packed1 = Avx2.And(packed1, mask);
				packed2 = Avx2.And(packed2, mask);
				packed3 = Avx2.And(packed3, mask);
				packed0 = Avx2.Or(packed0, alpha0);
				packed1 = Avx2.Or(packed1, alpha1);
				packed2 = Avx2.Or(packed2, alpha2);
				packed3 = Avx2.Or(packed3, alpha3);

				Avx2.Store(dataPtr + offset + 0x00, packed0);
				Avx2.Store(dataPtr + offset + 0x08, packed1);
				Avx2.Store(dataPtr + offset + 0x10, packed2);
				Avx2.Store(dataPtr + offset + 0x18, packed3);
			}
		}

		// This is unlikely to happen, but handle when there are still elements left (the texture size isn't aligned to 4)
		if (offset < data.Length) {
			ProcessTextureScalar(data.SliceUnsafe(offset));
		}
	}

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
			ProcessTextureScalar(data.SliceUnsafe(offset));
		}
	}
}
