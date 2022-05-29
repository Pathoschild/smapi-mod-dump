/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster;

internal static class SpriteOverrides {
	internal const int WaterBlock = 4; // Water is scaled up 4x for some reason

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool IsWater(Bounds bounds, XTexture2D texture) {
		return bounds.Right <= 640 && bounds.Top >= 2000 && bounds.Extent.MinOf >= WaterBlock && texture.NormalizedName() == @"LooseSprites\Cursors";
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool IsFont(XTexture2D texture, Vector2I spriteSize, Vector2I sheetSize) {
		switch (texture.Format) {
			case SurfaceFormat.Dxt1:
			case SurfaceFormat.Dxt1SRgb:
			case SurfaceFormat.Dxt1a:
			case SurfaceFormat.Dxt3:
			case SurfaceFormat.Dxt3SRgb:
			case SurfaceFormat.Dxt5:
			case SurfaceFormat.Dxt5SRgb:
				return Math.Min(spriteSize.MinOf, sheetSize.MinOf) >= 1;
			default:
				return false;
		}
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool IsFont(SpriteInfo info) => IsFont(info.Reference, info.Bounds.Extent, info.ReferenceSize);
}
