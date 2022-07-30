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
using SpriteMaster.Extensions.Reflection;
using SpriteMaster.Types.Reflection;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Harmonize.Patches.Game;

internal static class ColorChanger {
	private static readonly VariableStaticAccessor<XColor[]?>? ColorChangerBuffer = ReflectionExt.GetTypeExt("StardewValley.ColorChanger")?.GetStaticVariable("_buffer")?.GetStaticAccessor<XColor[]?>();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XColor[] GetBuffer(int length) {
		Contract.Assert(ColorChangerBuffer is not null);

		var staticBuffer = ColorChangerBuffer.Value;
		if (staticBuffer is null || staticBuffer.Length < length) {
			ColorChangerBuffer.Value = staticBuffer = GC.AllocateUninitializedArray<XColor>(length);
		}

		return staticBuffer;
	}

	[Harmonize(
		typeof(StardewValley.Game1),
		"StardewValley.ColorChanger",
		"swapColor",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool SwapColor(ref XTexture2D __result, XTexture2D texture, int targetColorIndex1, int r1, int g1, int b1, int startPixelIndex, int endPixelIndex) {
		if (ColorChangerBuffer is null) {
			return true;
		}

		if (endPixelIndex < startPixelIndex) {
			return true;
		}

		r1 = Math.Clamp(r1, 1, 255);
		g1 = Math.Clamp(g1, 1, 255);
		b1 = Math.Clamp(b1, 1, 255);

		uint dstColorPacked = (uint)(r1 | (g1 << 8) | (b1 << 16));

		var length = texture.Area();

		endPixelIndex = Math.Min(endPixelIndex, length);

		var data = GetBuffer(length);

		texture.GetData(data);

		ref XColor srcDataRef = ref MemoryMarshal.GetArrayDataReference(data);

		uint srcColorPacked = Unsafe.Add(ref srcDataRef, targetColorIndex1).PackedValue;

		srcDataRef = ref Unsafe.Add(ref srcDataRef, startPixelIndex);

		int count = (endPixelIndex - startPixelIndex) + 1;

		if (startPixelIndex + count > length) {
			--count;
		}

		for (int i = 0; i < count; ++i) {
			ref XColor current = ref Unsafe.Add(ref srcDataRef, i);
			if (current.PackedValue == srcColorPacked) {
				current.PackedValue = dstColorPacked;
			}
		}

		texture.SetData(data);
		__result = texture;

		return false;
	}

	[Harmonize(
		typeof(StardewValley.Game1),
		"StardewValley.ColorChanger",
		"swapColors",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool SwapColor(XTexture2D texture, int targetColorIndex1, byte r1, byte g1, byte b1, int targetColorIndex2, byte r2, byte g2, byte b2) {
		if (ColorChangerBuffer is null) {
			return true;
		}

		r1 = Math.Max(r1, (byte)1);
		g1 = Math.Max(g1, (byte)1);
		b1 = Math.Max(b1, (byte)1);
		r2 = Math.Max(r2, (byte)1);
		g2 = Math.Max(g2, (byte)1);
		b2 = Math.Max(b2, (byte)1);

		uint dstColor1Packed = (uint)(r1 | (g1 << 8) | (b1 << 16));
		uint dstColor2Packed = (uint)(r2 | (g2 << 8) | (b2 << 16));

		var length = texture.Area();

		var data = GetBuffer(length);

		texture.GetData(data);

		ref XColor srcDataRef = ref MemoryMarshal.GetArrayDataReference(data);

		uint srcColor1Packed = Unsafe.Add(ref srcDataRef, targetColorIndex1).PackedValue;
		uint srcColor2Packed = Unsafe.Add(ref srcDataRef, targetColorIndex2).PackedValue;

		for (int i = 0; i < length; ++i) {
			ref XColor current = ref Unsafe.Add(ref srcDataRef, i);
			uint currentPacked = current.PackedValue;
			if (currentPacked == srcColor1Packed) {
				current.PackedValue = dstColor1Packed;
			}
			else if (currentPacked == srcColor2Packed) {
				current.PackedValue = dstColor2Packed;
			}
		}

		texture.SetData(data);

		return false;
	}
}
