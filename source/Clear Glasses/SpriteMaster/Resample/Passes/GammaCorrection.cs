/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Colors;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Resample.Passes;

internal static class GammaCorrection {
	private static readonly ColorSpace ColorSpace = ColorSpace.sRGB_Precise;
	private static readonly ConverterRef ColorConverter = ColorSpace.GetConverterRef();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe void Delinearize(Span<Color16> data, Vector2I size) {
		var pTable = ColorConverter.DelinearizeTable16Ptr;

		fixed (Color16* pRefData = data) {
			ulong* pData = (ulong*)pRefData;
			for (int i = 0; i < data.Length; ++i, ++pData) {
				ulong item = *pData;
				ulong res0 = pTable[(ushort)item];
				ulong res1 = pTable[(ushort)(item >> 16)];
				ulong res2 = pTable[(ushort)(item >> 32)];
				item = (item & 0xFFFF_0000_0000_0000UL) | res0 | (res1 << 16) | (res2 << 32);
				*pData = item;
			}
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static unsafe void Linearize(Span<Color16> data, Vector2I size) {
		var pTable = ColorConverter.LinearizeTable16Ptr;

		fixed (Color16* pRefData = data) {
			ulong* pData = (ulong*)pRefData;
			for (int i = 0; i < data.Length; ++i, ++pData) {
				ulong item = *pData;
				ulong res0 = pTable[(ushort)item];
				ulong res1 = pTable[(ushort)(item >> 16)];
				ulong res2 = pTable[(ushort)(item >> 32)];
				item = (item & 0xFFFF_0000_0000_0000UL) | res0 | (res1 << 16) | (res2 << 32);
				*pData = item;
			}
		}
	}
}
