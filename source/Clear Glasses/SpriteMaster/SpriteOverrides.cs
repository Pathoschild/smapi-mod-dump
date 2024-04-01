/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Metadata;
using SpriteMaster.Types;
using System.Runtime.CompilerServices;

namespace SpriteMaster;

internal static class SpriteOverrides {
	internal const int WaterBlock = 4; // Water is scaled up 4x for some reason

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static bool IsWaterBounds(Bounds bounds) =>
		bounds.Right <= 640 && bounds.Top >= 2000 && bounds.Extent.MinOf >= WaterBlock;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool IsWater(Bounds bounds, Texture2DMeta meta) =>
		IsWaterBounds(bounds) && meta.IsCursors;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool IsWater(Bounds bounds, XTexture2D texture) =>
		IsWaterBounds(bounds) && texture.NormalizedName() == @"LooseSprites\Cursors";

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool IsFont(XTexture2D texture, Vector2I spriteSize, Vector2I sheetSize) {
		switch (texture.Format) {
			case SurfaceFormat.Dxt1:
			case SurfaceFormat.Dxt1SRgb:
			case SurfaceFormat.Dxt1a:
			case SurfaceFormat.Dxt3:
			case SurfaceFormat.Dxt3SRgb:
			case SurfaceFormat.Dxt5:
			case SurfaceFormat.Dxt5SRgb: {
				//return spriteSize.X != 0 && spriteSize.Y != 0 && sheetSize.X != 0 && sheetSize.Y != 0;
				// the two vectors are packed, so if we OR them together, if the upper or lower 32-bits of the ORed value are all zero,
				// then one of them has a zero component.
				ulong packed = spriteSize.Packed | sheetSize.Packed;
				uint packedLow = (uint)packed;
				uint packedHigh = (uint)(packed >> 32);
				return packedLow != 0u && packedHigh != 0u;
			}
			default:
				return false;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool IsFont(SpriteInfo info) =>
		IsFont(info.Reference, info.Bounds.Extent, info.ReferenceSize);
}
